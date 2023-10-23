using OddbitAi.Niko.Models;
using OddbitAi.Models.VisionDto;

namespace OddbitAi.Niko.Cogs
{
    public enum State
    {
        Idle,
        LookAround,
        Greet
        // ...
    }

    public class StateMachine
    {
        private class Context
        {
            public DateTime Timestamp { get; set; }
            public List<DetectedObjectDto>? DetectedObjects { get; set; }
            public List<DetectedObjectDto>? DetectedFaces { get; set; }
            public List<ChatItem>? ChatHistory { get; set; }
        }

        private readonly Queue<Context> contextQueue
            = new();

        public State State { get; set; }
            = State.Idle;

        public TimeSpan ContextQueueSpan { get; set; }
            = TimeSpan.FromSeconds(5);

        private void TrimQueue()
        {
            while (contextQueue.Any() && DateTime.UtcNow - contextQueue.Peek().Timestamp > ContextQueueSpan)
            {
                contextQueue.Dequeue();
            }
        }

        public void AddContext(
            IEnumerable<DetectedObjectDto>? detectedObjects,
            IEnumerable<DetectedObjectDto>? detectedFaces,
            IEnumerable<ChatItem>? chatHistory
        )
        {
            var ctx = new Context
            {
                Timestamp = DateTime.UtcNow,
                DetectedObjects = detectedObjects == null
                    ? null
                    : new List<DetectedObjectDto>(detectedObjects),
                DetectedFaces = detectedFaces == null
                    ? null
                    : new List<DetectedObjectDto>(detectedFaces),
                ChatHistory = chatHistory == null
                    ? null
                    : new List<ChatItem>(chatHistory)
            };
            contextQueue.Enqueue(ctx);
            TrimQueue();
        }
    }
}
