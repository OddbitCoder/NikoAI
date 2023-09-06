namespace OddbitAi.AudioRecorder
{
    internal static class TextBufferExtensions
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

        // finds all words in 'wordList' that overlap with 'word' and have the same token
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

        private static void ResolveSequenceRecurs(List<int> list, List<List<int>> sequence, int seqItemIdx, List<List<int>> expSeqs, int idxLimit)
        {
            foreach (var idx in sequence[seqItemIdx].Where(x => x > idxLimit).DefaultIfEmpty(-1))
            {
                list.Add(idx);
                if (seqItemIdx == sequence.Count - 1)
                {
                    expSeqs.Add(new(list));
                }
                else
                {
                    ResolveSequenceRecurs(list, sequence, seqItemIdx + 1, expSeqs, idx);
                }
                list.RemoveAt(list.Count - 1);
            }
        }

        // creates all possible variants of 'sequence'
        public static List<List<int>> ResolveSequence(List<List<int>> sequence)
        {
            var expSeqs = new List<List<int>>();
            ResolveSequenceRecurs(new(), sequence, 0, expSeqs, -1);
            return expSeqs;
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

        public static void TrimEndAt(this List<Word> words, DateTime targetTime, string? anchorToken = null)
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
                    if (anchorToken != null)
                    {
                        // try to find the "anchor" token
                        if (words[i].Token() == anchorToken)
                        {
                            words.RemoveRange(i, words.Count - i);
                            return;
                        }
                        else if (i + 1 < words.Count && words[i + 1].Token() == anchorToken)
                        {
                            words.RemoveRange(i + 1, words.Count - (i + 1));
                            return;
                        }
                    }
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
        private readonly List<Word> buffer
            = new();
        private readonly TimeSpan trimLen;

        private DateTime? textBufferEndTime;

        public TextBuffer(TimeSpan trimLen)
        {
            this.trimLen = trimLen;
        }

        public void AddWords(List<Word> words, DateTime wordsStartTime, DateTime wordsEndTime)
        {
            Console.WriteLine("textBuffer:");
            DebugOut(buffer);
            Console.WriteLine("words:");
            DebugOut(words);
            Console.WriteLine("--");

            // we experiment here with sequence matching algorithm
            var matches = new List<List<int>>();
            foreach (Word word in words)
            {
                var m = word.FindOverlappingWords(buffer);
                matches.Add(m);
                Console.Write("(");
                foreach (var item in m) { Console.Write(item + " "); }
                Console.Write(")");
            }
            Console.WriteLine();


            //var matches_test = new List<List<int>>
            //{
            //    new List<int>() { 1 },
            //    new List<int>() { 2 },
            //    new List<int>() { 4, 3 },
            //    new List<int>() { 5, 4 },
            //    new List<int>() { 6 }
            //};
            // here we need to produce all possible variants
            var seqs = TextBufferExtensions.ResolveSequence(matches);

            foreach (var seq in seqs)
            {
                foreach (var item in seq)
                {
                    Console.Write($"{item} ");
                }
                Console.WriteLine();
            }


            var overlap = TextBufferExtensions.FindOverlapIndex(seqs);
            if (overlap.HasValue)
            {
                // we found overlap, let's stitch there
                buffer.RemoveRange(overlap.Value.oldChunkIdx, buffer.Count - overlap.Value.oldChunkIdx);
                words.RemoveRange(0, overlap.Value.newChunkIdx);
                buffer.AddRange(words);
                return;
            }

            Console.WriteLine("OVERLAP NOT FOUND!");




            // else do whatever

            Console.WriteLine("--");
            if (textBufferEndTime == null || wordsStartTime > textBufferEndTime) // text buffer is empty or there is no overlap
            {
                buffer.AddRange(words);
            }
            else // text buffer is not empty and there is overlap
            {
                // check if there is enough overlap for trimming (we need at least 2 x trimLen)
                if (textBufferEndTime - wordsStartTime >= 2 * trimLen)
                {
                    words.TrimStartAt(wordsStartTime + trimLen);
                    if (words.Any())
                    {
                        buffer.TrimEndAt(wordsStartTime + trimLen, words.First().Token());
                        buffer.AddRange(words);
                    }
                }
                else // not enough overlap
                {
                    words.RemoveAt(0);
                    if (words.Any())
                    {
                        buffer.TrimEndAt(words.First().StartTime, words.First().Token());
                        buffer.AddRange(words);
                    }
                }
            }
            // update buffer end time
            textBufferEndTime = wordsEndTime; 
        }

        public void Print()
        { 
            foreach (Word word in buffer)
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
