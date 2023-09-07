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

        public static List<int> FindOverlappingWordsNoTokenCheck(this Word word, List<Word> wordList)
        {
            var matchIdxList = new List<int>();
            for (int i = 0; i < wordList.Count; i++)
            {
                if ((word.EndTime - wordList[i].EndTime).Duration().TotalMilliseconds < 500) // WARNME: hardcoded threshold
                {
                    matchIdxList.Add(i);
                }
            }
            return matchIdxList;
        }

        private static void FindValidSequencesRecurs(List<int> list, List<List<int>> seqData, int seqItemIdx, List<List<int>> seqs, int idxLimit)
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
                    FindValidSequencesRecurs(list, seqData, seqItemIdx + 1, seqs, idx);
                }
                list.RemoveAt(list.Count - 1);
            }
        }

        public static void StitchWords(this List<Word> text, List<Word> words)
        {
            foreach (var word in words)
            {
                double minDiff = text.Select(x => (x.EndTime - word.EndTime).Duration().TotalMilliseconds).Min();
                Console.WriteLine($"{word} {minDiff}");
            }
        }

        // creates all valid sequences
        public static List<List<int>> FindValidSequences(List<List<int>> seqData)
        {
            var seqs = new List<List<int>>();
            FindValidSequencesRecurs(new(), seqData, 0, seqs, -1);
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

        public static DateTime? StartTime(this List<Word> words)
        {
            return words.Any() ? words[0].StartTime : null;
        }

        public static DateTime? EndTime(this List<Word> words)
        {
            return words.Any() ? words[^1].EndTime : null;
        }

        public static void TrimStartAt(this List<Word> words, DateTime targetTime)
        {
            for (int i = 0; i < words.Count; i++)
            {
                if (targetTime <= words[i].StartTime)
                {
                    words.RemoveRange(0, i);
                    return;
                }
                else if (targetTime <= words[i].EndTime)
                {
                    if ((targetTime - words[i].StartTime).Duration() < (targetTime - words[i].EndTime).Duration())
                    {
                        words.RemoveRange(0, i);
                    }
                    else
                    {
                        words.RemoveRange(0, i + 1);
                    }
                    return;
                }
            }
            words.Clear(); // empty list
        }

        public static void TrimEndAt(this List<Word> words, DateTime targetTime)
        {
            for (int i = 0; i < words.Count; i++)
            {
                if (targetTime <= words[i].StartTime)
                {
                    words.RemoveRange(i, words.Count - i);
                    return;
                }
                else if (targetTime <= words[i].EndTime)
                {
                    if ((targetTime - words[i].StartTime).Duration() < (targetTime - words[i].EndTime).Duration())
                    {
                        words.RemoveRange(i, words.Count - i);
                    }
                    else if (i + 1 < words.Count)
                    {
                        words.RemoveRange(i + 1, words.Count - (i + 1));
                    }
                    return;
                }
            }
        }
    }

    internal class TextBuffer
    {
        private readonly List<Word> wordBuffer
            = new();
        private readonly TimeSpan trimLen; 

        private DateTime? LastWordEndTime
            => wordBuffer.Any() ? wordBuffer.Last().EndTime : null;
        private DateTime? audioBufferEndTime;

        public TextBuffer(TimeSpan trimLen)
        {
            this.trimLen = trimLen;
        }

        public void AddWords(List<Word> words, DateTime audioBufferStartTime, DateTime audioBufferEndTime)
        {
            words = words.Where(x => !((x.EndTime - audioBufferEndTime).Duration() < trimLen)
                && !((x.EndTime/*safer!*/ - audioBufferStartTime).Duration() < trimLen)).ToList();
          
            if (!words.Any())
            {
                Console.WriteLine("Buffer empty");
                this.audioBufferEndTime = audioBufferEndTime;
                return;
            }

            var wordsStartTime = words.StartTime()!.Value;
            var wordsEndTime = words.EndTime()!.Value;



            Console.WriteLine("textBuffer:");
            DebugOut(wordBuffer);
            Console.WriteLine("words:");
            DebugOut(words);
            Console.WriteLine("--");

            // check if the two sequences overlap at all

            if (!wordBuffer.Any() || LastWordEndTime <= wordsStartTime)
            {
                Console.WriteLine("The two sequences do not overlap.");
                wordBuffer.AddRange(words);
                this.audioBufferEndTime = audioBufferEndTime;
                return;
            }

            var seqData = new List<List<int>>();
            foreach (Word word in words)
            {
                seqData.Add(word.FindOverlappingWords(wordBuffer));
            }

            // produce all possible sequences
            var seqs = TextBufferUtils.FindValidSequences(seqData);

            foreach (var seq in seqs)
            {
                foreach (var item in seq)
                {
                    Console.Write($"{item} ");
                }
                Console.WriteLine();
            }

            // find overlap
            var overlap = TextBufferUtils.FindOverlapIndex(seqs);
            
            if (overlap.HasValue) // we found overlap, let's stitch there
            {
                wordBuffer.RemoveRange(overlap.Value.oldChunkIdx, wordBuffer.Count - overlap.Value.oldChunkIdx);
                words.RemoveRange(0, overlap.Value.newChunkIdx);
                wordBuffer.AddRange(words);
                this.audioBufferEndTime = audioBufferEndTime;
                return;
            }

            Console.WriteLine("OVERLAP NOT FOUND!");


            
            var seqData2 = new List<List<int>>();
            foreach (Word word in words)
            {
                seqData2.Add(word.FindOverlappingWordsNoTokenCheck(wordBuffer));
            }
            Console.WriteLine("seqData2.count " + seqData2.Count);
            var seqs2 = TextBufferUtils.FindValidSequences(seqData2);
            var overlap2 = TextBufferUtils.FindOverlapIndex(seqs2);
            if (overlap2.HasValue) // we found overlap, let's stitch there
            {
                Console.WriteLine("FOUND OVERLAP ON END-MATCHING HEURISTICS");
                wordBuffer.RemoveRange(overlap2.Value.oldChunkIdx, wordBuffer.Count - overlap2.Value.oldChunkIdx);
                words.RemoveRange(0, overlap2.Value.newChunkIdx);
                wordBuffer.AddRange(words);
                this.audioBufferEndTime = audioBufferEndTime;
                return;
            }

            // do something silly, since you don't know what to do

            wordBuffer.Add(new Word
            {
                Word = " (...)",
                StartTime = LastWordEndTime!.Value,
                EndTime = LastWordEndTime!.Value,
                Probability = 0,
                SegmentId = -1
            });
            wordBuffer.AddRange(words);
            this.audioBufferEndTime = audioBufferEndTime;
            return;




            // else do whatever

            //Console.WriteLine("--");
            //if (lastWordEndTime == null || audioBufferStartTime > lastWordEndTime) // text buffer is empty or there is no overlap
            //{
            //    wordBuffer.AddRange(words);
            //}
            //else // text buffer is not empty and there is overlap
            //{
            //    // check if there is enough overlap for trimming (we need at least 2 x trimLen)
            //    if (lastWordEndTime - audioBufferStartTime >= 2 * trimLen)
            //    {
            //        words.TrimStartAt(audioBufferStartTime + trimLen);
            //        if (words.Any())
            //        {
            //            wordBuffer.TrimEndAt(audioBufferStartTime + trimLen, words.First().Token());
            //            wordBuffer.AddRange(words);
            //        }
            //    }
            //    else // not enough overlap
            //    {
            //        words.RemoveAt(0);
            //        if (words.Any())
            //        {
            //            wordBuffer.TrimEndAt(words.First().StartTime, words.First().Token());
            //            wordBuffer.AddRange(words);
            //        }
            //    }
            //}
            //// update buffer end time
            //lastWordEndTime = audioBufferEndTime; 
        }

        public void Print()
        { 
            foreach (Word word in wordBuffer)
            {
                Console.Write(word.Word);
            }
            Console.WriteLine();
        }

        private static void DebugOut(List<Word> words)
        {
            foreach (var word in words)
            {
                Console.Write($"<{word.StartTime:mm.ss.fff} {word.Word}({word.Probability}) {word.EndTime:mm.ss.fff}> ");
            }
            Console.WriteLine();    
        }
    }
}
