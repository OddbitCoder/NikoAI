using OddbitAi.Whisper.Dto;

namespace OddbitAi.AudioRecorder
{
    internal class Word : WordDto
    {
        public int SegmentId { get; set; }
        public DateTime StartTimeUtc { get; set; }
        public DateTime EndTimeUtc { get; set; }
    }
}
