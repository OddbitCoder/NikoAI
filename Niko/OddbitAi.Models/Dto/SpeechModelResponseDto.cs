namespace OddbitAi.Models.Dto
{
    public class SpeechModelResponseDto
    {
        public string? Text { get; set; }
        public SpeechSegmentDto[]? Segments { get; set; }
    }
}
