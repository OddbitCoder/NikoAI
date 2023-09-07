namespace OddbitAi.AudioRecorder
{
    internal static class TextBufferUtils
    {
        // normalizes word string
        public static string? Token(this Word word)
        {
            return word.Word?
                .Select(ch => char.ToLower(ch))
                .Where(ch => char.IsLetterOrDigit(ch))
                .Select(ch => $"{ch}")
                .DefaultIfEmpty("")
                .Aggregate((x, y) => x + y);
        }

        // finds all words in 'wordList' that overlap with 'word' and have the same normalized form (token)
        public static List<int> FindOverlappingWords(this Word word, List<Word> wordList) 
        {
            var matchIdxList = new List<int>();
            for (int i = 0; i < wordList.Count; i++)
            {
                if (word.Token() == wordList[i].Token())
                {
                    if (word.StartTime < wordList[i].EndTime && word.EndTime > wordList[i].StartTime)
                    { 
                        matchIdxList.Add(i);
                    }
                }
            }
            return matchIdxList;
        }

        // finds all words in 'wordList' that overlap with 'word' (token is not checked)
        public static List<int> FindOverlappingWordsNoTokenCheck(this Word word, List<Word> wordList, int threshMs)
        {
            var matchIdxList = new List<int>();
            for (int i = 0; i < wordList.Count; i++)
            {
                if ((word.EndTime - wordList[i].EndTime).Duration().TotalMilliseconds < threshMs) 
                {
                    matchIdxList.Add(i);
                }
            }
            return matchIdxList;
        }

        private static void GetValidSequencesRecurs(List<int> list, List<List<int>> seqData, int seqItemIdx, List<List<int>> seqs, int idxLimit)
        {
            foreach (var idx in seqData[seqItemIdx].Where(x => x > idxLimit).DefaultIfEmpty(-1))
            {
                list.Add(idx);
                if (seqItemIdx == seqData.Count - 1)
                {
                    seqs.Add(new(list));
                }
                else
                {
                    GetValidSequencesRecurs(list, seqData, seqItemIdx + 1, seqs, idx);
                }
                list.RemoveAt(list.Count - 1);
            }
        }

        // creates all valid sequences
        public static List<List<int>> GetValidSequences(List<List<int>> seqData)
        {
            var seqs = new List<List<int>>();
            GetValidSequencesRecurs(new(), seqData, 0, seqs, -1);
            return seqs;
        }

        public static (int oldChunkIdx, int newChunkIdx)? FindOverlapIndex(List<List<int>> seqs)
        {
            // look for seq of length 3
            foreach (var seq in seqs)
            {
                for (int i = 2; i < seq.Count; i++)
                {
                    if (seq[i] - 1 == seq[i - 1] && seq[i - 1] - 1 == seq[i - 2] && seq[i - 2] != -1)
                    {
                        return (seq[i - 2], i - 2);
                    }
                }
            }
            // look for seq of length 2
            foreach (var seq in seqs)
            {
                for (int i = 1; i < seq.Count; i++)
                {
                    if (seq[i] - 1 == seq[i - 1] && seq[i - 1] != -1)
                    {
                        return (seq[i - 1], i - 1);
                    }
                }
            }
            // look for single-element match
            foreach (var seq in seqs)
            {
                for (int i = 0; i < seq.Count; i++)
                {
                    if (seq[i] != -1)
                    {
                        return (seq[i], i);
                    }
                }
            }
            return null;
        }

        public static bool Stitch(this List<Word> wordBuffer, List<Word> words, Func<Word, List<Word>, List<int>> matcher)
        {
            // map each word into list of indices into wordBuffer (possible matches)
            var seqData = new List<List<int>>();
            foreach (Word word in words)
            {
                seqData.Add(matcher(word, wordBuffer));
            }

            // produce all valid sequences
            var seqs = TextBufferUtils.GetValidSequences(seqData);

            // [try to] find overlap index
            var overlap = TextBufferUtils.FindOverlapIndex(seqs);

            if (overlap.HasValue) // we found overlap, let's stitch there
            {
                wordBuffer.RemoveRange(overlap.Value.oldChunkIdx, wordBuffer.Count - overlap.Value.oldChunkIdx);
                words.RemoveRange(0, overlap.Value.newChunkIdx);
                wordBuffer.AddRange(words);
                return true;
            }

            return false;
        }
    }

    internal class TextBuffer
    {
        private readonly List<Word> wordBuffer
            = new();
        private readonly TimeSpan trimLen; 

        private DateTime? LastWordEndTime
            => wordBuffer.Any() ? wordBuffer.Last().EndTime : null;
        //private DateTime? audioBufferEndTime;

        public TextBuffer(TimeSpan trimLen)
        {
            this.trimLen = trimLen;
        }

        public void AddWords(List<Word> words, DateTime audioBufferStartTime, DateTime audioBufferEndTime)
        {
            // trim
            words = words.Where(x => !((x.EndTime - audioBufferEndTime).Duration() < trimLen)
                && !((x.EndTime/*safer!*/ - audioBufferStartTime).Duration() < trimLen)).ToList();

            if (words.Any()) // anything left?
            {
                var wordsStartTime = words[0].StartTime;
                var wordsEndTime = words[^1].EndTime;

                if (!wordBuffer.Any() || LastWordEndTime <= wordsStartTime) // no overlap? // TODO: different check here?
                {
                    wordBuffer.AddRange(words);
                }
                else
                {
                    if (!wordBuffer.Stitch(words, (Word word, List<Word> buffer) => TextBufferUtils.FindOverlappingWords(word, buffer)))
                    {
                        if (!wordBuffer.Stitch(words, (Word word, List<Word> buffer) => TextBufferUtils.FindOverlappingWordsNoTokenCheck(word, buffer, /*threshMs=*/200)))
                        {
                            // none of the heuristics work, do whatever
                            wordBuffer.Add(new Word
                            {
                                Word = " (...)",
                                StartTime = LastWordEndTime!.Value,
                                EndTime = LastWordEndTime!.Value,
                                Probability = 0,
                                SegmentId = -1
                            });
                            wordBuffer.AddRange(words);
                        }
                    }
                }
            }

            //this.audioBufferEndTime = audioBufferEndTime;
        }

        public void Print()
        { 
            foreach (Word word in wordBuffer)
            {
                Console.Write(word.Word);
            }
            Console.WriteLine();
        }
    }
}
