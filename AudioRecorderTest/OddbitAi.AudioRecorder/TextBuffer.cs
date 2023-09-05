namespace OddbitAi.AudioRecorder
{
    internal static class TextBufferExtensions
    {
        public static string? Token(this Word word)
        {
            return word.Word?
                .Select(ch => char.ToLower(ch))
                .Where(ch => char.IsLetterOrDigit(ch))
                .Select(ch => $"{ch}")
                .DefaultIfEmpty("")
                .Aggregate((x, y) => x + y);
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
