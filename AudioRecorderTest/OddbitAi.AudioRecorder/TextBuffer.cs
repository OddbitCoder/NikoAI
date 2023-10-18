namespace OddbitAi.AudioRecorder
{
    internal class TextBuffer
    {
        private readonly List<Word> text
            = new();
        private readonly TimeSpan trimLen;
        //private DateTime? audioBufferEndTime;

        private DateTime? LastWordEndTime
            => text.Any() ? text.Last().EndTime : null;
        private int LastWordSegmentId
            => text.Any() ? text.Last().SegmentId : 0;

        private static string? Normalize(Word word)
        {
            return word.String?
                .Select(ch => char.ToLower(ch))
                .Where(ch => char.IsLetterOrDigit(ch))
                .Select(ch => $"{ch}")
                .DefaultIfEmpty("")
                .Aggregate((x, y) => x + y);
        }

        // finds all words in 'text' that overlap with 'word' and have the same normalized form
        private static List<int> FindOverlappingWords(Word word, List<Word> text)
        {
            var matches = new List<int>();
            for (int i = 0; i < text.Count; i++)
            {
                if (Normalize(word) == Normalize(text[i]))
                {
                    if (word.StartTime < text[i].EndTime && word.EndTime > text[i].StartTime)
                    {
                        matches.Add(i);
                    }
                }
            }
            return matches;
        }

        // finds all words in 'text' that overlap with 'word' (ignores content)
        public static List<int> FindOverlappingWordsNoTokenCheck(Word word, List<Word> text, TimeSpan thresh) 
        {
            var matches = new List<int>();
            for (int i = 0; i < text.Count; i++)
            {
                if ((word.EndTime - text[i].EndTime).Duration() < thresh)
                {
                    matches.Add(i);
                }
            }
            return matches;
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
        private static List<List<int>> GetValidSequences(List<List<int>> seqData)
        {
            var seqs = new List<List<int>>();
            GetValidSequencesRecurs(new(), seqData, 0, seqs, -1);
            return seqs;
        }

        private static (int textIdx, int snippetIdx)? FindOverlapIndex(List<List<int>> seqs)
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

        private static bool Append(List<Word> text, List<Word> snippet, Func<Word, List<Word>, List<int>> matcher)
        {
            // map each word into list of indices into wordBuffer (possible matches)
            var seqData = new List<List<int>>();
            foreach (Word word in snippet)
            {
                seqData.Add(matcher(word, text));
            }

            // produce all valid sequences
            var seqs = GetValidSequences(seqData);

            // [try to] find overlap index
            var overlap = FindOverlapIndex(seqs);

            if (overlap.HasValue) // we found overlap, let's stitch there
            {
                int idOfs = text[overlap.Value.textIdx].SegmentId - snippet[overlap.Value.snippetIdx].SegmentId;
                foreach (var item in snippet) { item.SegmentId += idOfs; }

                text.RemoveRange(overlap.Value.textIdx, text.Count - overlap.Value.textIdx);
                snippet.RemoveRange(0, overlap.Value.snippetIdx);
                text.AddRange(snippet);
                return true;
            }

            return false;
        }

        public TextBuffer(TimeSpan trimLen)
        {
            this.trimLen = trimLen;
        }

        public void AddWords(List<Word> snippet, DateTime audioBufferStartTime, DateTime audioBufferEndTime)
        {
            // trim
            snippet = snippet.Where(x => !((x.EndTime - audioBufferEndTime).Duration() < trimLen)
                && !((x.EndTime/*safer!*/ - audioBufferStartTime).Duration() < trimLen)).ToList();

            if (snippet.Any()) // anything left?
            {
                var snippetStartTime = snippet[0].StartTime;
                var snippetEndTime = snippet[^1].EndTime;

                if (!text.Any() || LastWordEndTime <= snippetStartTime) // no overlap? // TODO: different condition here?
                {
                    int idOfs = LastWordSegmentId - snippet[0].SegmentId + 1;
                    foreach (var item in snippet) { item.SegmentId += idOfs; }
                    text.AddRange(snippet);
                }
                else
                {
                    if (!Append(text, snippet, (Word word, List<Word> text) => FindOverlappingWords(word, text)))
                    {
                        if (!Append(text, snippet, (Word word, List<Word> text) => FindOverlappingWordsNoTokenCheck(word, text, TimeSpan.FromMilliseconds(200)))) // WARNME: hardcoded threshold
                        {
                             // none of the heuristics work, do whatever
                            text.Add(new Word
                            {
                                String = " (...)",
                                StartTime = LastWordEndTime!.Value,
                                EndTime = LastWordEndTime!.Value,
                                Probability = 0,
                                SegmentId = LastWordSegmentId + 1
                            });
                            int idOfs = LastWordSegmentId - snippet[0].SegmentId + 1;
                            foreach (var item in snippet) { item.SegmentId += idOfs; }
                            text.AddRange(snippet);
                        }
                    }
                }
            }

            //this.audioBufferEndTime = audioBufferEndTime;
        }

        public void Print()
        { 
            foreach (var word in text)
            {
                //Console.Write(word.String + "/" + word.SegmentId);
                Console.Write(word.String);
            }
            Console.WriteLine();
        }

        public List<Snippet> GetTextSnippets()
        {
            var snippets = new List<Snippet>();
            int id = -1;
            foreach (var word in text)
            {
                if (word.SegmentId != id)
                {
                    snippets.Add(new Snippet() { Text = "" });
                    id = word.SegmentId;
                }
                snippets.Last().Text += word.String;
            }
            foreach (var snippet in snippets)
            {
                snippet.Text = snippet.Text!.Trim();
            }
            return snippets;
        }

        public void WriteToFile(string filename, IEnumerable<Word>? snippet = null)
        { 
            using (var sw = new StreamWriter(filename, append: true)) 
            {
                foreach (var word in snippet ?? text)
                {
                    sw.Write($"<{word.StartTime:ss.fff} {word.String}/{word.SegmentId} {word.EndTime:ss.fff}> ");
                }
                sw.WriteLine();
            }
        }
    }
}
