using System.Text.Json.Serialization;

namespace OddbitAi.Whisper.Dto
{
    public enum ResponseFormat
    { 
        Whisper = 0,
        NeMo,
        Exception
    } 

    public class TextDto
    {
        public ResponseFormat Format { get; set; } 
            = ResponseFormat.Whisper;
        public string? Text { get; set; }
        public SegmentDto[]? Segments { get; set; }
    }
}
