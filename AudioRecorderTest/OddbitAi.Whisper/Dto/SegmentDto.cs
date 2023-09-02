using System.Text.Json.Serialization;

namespace OddbitAi.Whisper.Dto
{
    public class SegmentDto
    {
        public double Start { get; set; }
        public double End { get; set; }
        [JsonPropertyName("no_speech_prob")]
        public double NoSpeechProb { get; set; }
        public WordDto[]? Words { get; set; }
    }
}
