using OddbitAi.Whisper.Dto;

namespace OddbitAi.AudioRecorder
{
    internal class Word : WordDto
    {
        public int SegmentId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
