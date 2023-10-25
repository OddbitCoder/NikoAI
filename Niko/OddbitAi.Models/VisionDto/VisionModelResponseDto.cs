namespace OddbitAi.Models.VisionDto
{
    public class VisionModelResponseDto
    {
        public string? Summary { get; set; }
        public DetectedObjectDto[]? Objects { get; set; }
    }
}
