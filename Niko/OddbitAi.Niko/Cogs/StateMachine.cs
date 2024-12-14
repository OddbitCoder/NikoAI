using OddbitAi.Models.Dto;
using OddbitAi.Niko.Models;

namespace OddbitAi.Niko.Cogs
{
    public enum State
    {
        Idle,
        LookAndListen,
        Greet
        // ...
    }

    public class StateMachine
    {
        private readonly Queue<(DateTime timestamp, List<DetectedObjectDto> objects)> detectedFaces
            = new();
        private readonly Queue<(DateTime timestamp, List<DetectedObjectDto> objects)> detectedObjects
            = new();
        private readonly List<ChatItem> chatContext
            = new();

        public State State { get; set; }
            = State.Idle;

        public TimeSpan ContextQueueSpan { get; set; }
            = TimeSpan.FromSeconds(3);

        public void SetChatContext(IEnumerable<ChatItem> context)
        {
            lock (chatContext)
            {
                chatContext.Clear();
                chatContext.AddRange(context);
            }
        }

        public void AddDetectedFaces(IEnumerable<DetectedObjectDto> items, DateTime? timestamp = null)
        {
            lock (detectedFaces)
            {
                var now = DateTime.UtcNow;
                detectedFaces.Enqueue(((timestamp ?? now), new List<DetectedObjectDto>(items)));
                while (detectedFaces.Any() && (timestamp ?? now) - detectedFaces.Peek().timestamp > ContextQueueSpan)
                {
                    detectedFaces.Dequeue();
                }
            }
        }

        public void AddDetectedObjects(IEnumerable<DetectedObjectDto> items, DateTime? timestamp = null)
        {
            lock (detectedObjects)
            {
                var now = DateTime.UtcNow;
                detectedObjects.Enqueue(((timestamp ?? now), new List<DetectedObjectDto>(items)));
                while (detectedObjects.Any() && (timestamp ?? now) - detectedObjects.Peek().timestamp > ContextQueueSpan)
                {
                    detectedObjects.Dequeue();
                }
            }
        }

        private readonly string[] heyNicoStrList = { 
            "heynico", 
            "heyniko" 
        };

        private bool IsHeyNiko(Word w1, Word w2)
        {
            var str = new string((w1.String + w2.String).ToLower().Where(ch => char.IsLetter(ch)).ToArray());
            return heyNicoStrList.Contains(str);
        }

        public void Transition()
        {
            if (State == State.Idle)
            {
                Console.WriteLine("Idle");
                // check for "hey, Niko"
                lock (chatContext)
                {
                    for (int i = chatContext.Count - 1; i >= 0; i--)
                    { 
                        var item = chatContext[i];
                        for (int j = item.Words.Count - 2; j >= 0; j--)
                        { 
                            var w1 = item.Words[j];
                            var w2 = item.Words[j + 1];
                            if (IsHeyNiko(w1, w2))
                            {
                                // TODO: note where this phrase was observed
                                State = State.LookAndListen;
                                return;
                            }
                        }
                    }
                }
            }
            else if (State == State.LookAndListen)
            {

            }
        }
    }
}
