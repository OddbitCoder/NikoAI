namespace OddbitAi.AudioRecorder
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
        public State State { get; set; }
            = State.Idle;
    }
}
