namespace OddbitAi.Models.AsrDto
{
    public class WordDto
    {
        public string? Word { get; set; }
        public double Start { get; set; }
        public double End { get; set; }
        public double Probability { get; set; }
    }
}
