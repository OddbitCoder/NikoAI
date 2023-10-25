using System.Text.Json.Serialization;

namespace OddbitAi.Models.SpeechDto
{
    public class SpeechSegmentDto
    {
        public int Id { get; set; }
        public double Start { get; set; }
        public double End { get; set; }
        [JsonPropertyName("no_speech_prob")]
        public double NoSpeechProb { get; set; }
        public WordDto[]? Words { get; set; }
    }
}
