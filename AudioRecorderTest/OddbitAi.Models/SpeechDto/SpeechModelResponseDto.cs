namespace OddbitAi.Models.SpeechDto
{
    public enum SpeechModelResponseType
    { 
        Whisper = 0,
        NeMo,
        Exception
    } 

    public class SpeechModelResponseDto
    {
        public SpeechModelResponseType Format { get; set; } 
            = SpeechModelResponseType.Whisper;
        public string? Text { get; set; }
        public SpeechSegmentDto[]? Segments { get; set; }
    }
}
