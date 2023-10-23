namespace OddbitAi.Niko.Models
{
    public class TranscriptItem : ChatItem
    {
        public Color Color { get; set; }

        public TranscriptItem(DateTime timestamp, string speaker, string text, Color color) : base(timestamp, speaker, text)
        {
            Color = color;
        }
    }
}
