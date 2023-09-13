namespace OddbitAi.Models.AsrDto
{
    public enum ResponseType
    { 
        Whisper = 0,
        NeMo,
        Exception
    } 

    public class TextDto
    {
        public ResponseType Format { get; set; } 
            = ResponseType.Whisper;
        public string? Text { get; set; }
        public SegmentDto[]? Segments { get; set; }
    }
}
