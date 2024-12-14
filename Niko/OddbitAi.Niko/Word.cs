namespace OddbitAi.Niko
{
    public class Word 
    {
        public string? String { get; set; }
        public double Probability { get; set; }
        public int SegmentId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
