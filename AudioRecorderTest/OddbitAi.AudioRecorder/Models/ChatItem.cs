namespace OddbitAi.Niko.Models
{
    public class ChatItem
    {
        public DateTime Timestamp { get; }
        public string Text { get; }
        public string Speaker { get; }

        public ChatItem(DateTime timestamp, string speaker, string text)
        { 
            Timestamp = timestamp;
            Speaker = speaker;
            Text = text;
        }
    }
}
