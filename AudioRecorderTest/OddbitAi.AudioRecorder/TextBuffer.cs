namespace OddbitAi.AudioRecorder
{
    internal static class TextBufferExtensions
    {
        public static void TrimStartAt(this List<Word> words, DateTime targetTime)
        {
            for (int i = 0; i < words.Count; i++)
            {
                if (targetTime <= words[i].StartTimeUtc)
                {
                    words.RemoveRange(0, i);
                    return;
                }
                else if (targetTime <= words[i].EndTimeUtc)
                {
                    if ((targetTime - words[i].StartTimeUtc).Duration() < (targetTime - words[i].EndTimeUtc).Duration())
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
                if (targetTime <= words[i].StartTimeUtc)
                {
                    words.RemoveRange(i, words.Count - i);
                    return;
                }
                else if (targetTime <= words[i].EndTimeUtc)
                {
                    if ((targetTime - words[i].StartTimeUtc).Duration() < (targetTime - words[i].EndTimeUtc).Duration())
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

        private DateTime? endTime;

        public TextBuffer(TimeSpan trimLen)
        {
            this.trimLen = trimLen;
        }

        public void AddWords(List<Word> words, DateTime bufferStartTime, DateTime bufferEndTime)
        {
            Console.WriteLine("textBuffer:");
            DebugOut(buffer);
            Console.WriteLine("words:");
            DebugOut(words);
            Console.WriteLine("--");

            if (endTime == null || bufferStartTime > endTime) // text buffer is empty or there is no overlap
            {
                buffer.AddRange(words);
            }
            else // text buffer is not empty and there is overlap
            {
                // check if there is enough overlap for trimming (we need at least 2 x trimLen)
                if (endTime - bufferStartTime >= 2 * trimLen)
                {
                    words.TrimStartAt(bufferStartTime + trimLen);
                    if (words.Any())
                    {
                        buffer.TrimEndAt(bufferStartTime + trimLen);
                        buffer.AddRange(words);
                    }
                }
                else // not enough overlap
                {
                    words.RemoveAt(0);
                    if (words.Any())
                    {
                        buffer.TrimEndAt(words.First().StartTimeUtc);
                        buffer.AddRange(words);
                    }
                }
            }
            // update buffer end time
            endTime = bufferEndTime; 
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
                Console.Write($"<{word.StartTimeUtc:mm.ss.fff} {word.Word}({word.Probability}) {word.EndTimeUtc:mm.ss.fff}> ");
            }
            Console.WriteLine();    
        }
    }
}
