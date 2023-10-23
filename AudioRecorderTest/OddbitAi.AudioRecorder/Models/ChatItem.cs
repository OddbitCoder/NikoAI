namespace OddbitAi.Niko.Models
{
    public class ChatItem
    {
        public DateTime Timestamp { get; set; }
        public string? Text { get; set; }
        public string? Speaker { get; set; }
        public Color Color { get; set; }
            = Color.Black;
    }
}
