using OddbitAi.Models.VisionDto;

namespace OddbitAi.AudioRecorder
{
    public class State
    {
        public List<DetectedObjectDto> DetectedObjects { get; } // WARNME: these should not be DTOs
            = new();
        public List<DetectedObjectDto> DetectedFaces { get; }
            = new();

        public void AddDetectedObjects(VisionModelResponseDto objectDetectionResponse)
        {
            // TODO: put into queue, determine which objects are "visible"
            lock (DetectedObjects)
            {
                DetectedObjects.Clear();
                DetectedObjects.AddRange(objectDetectionResponse?.Objects ?? Array.Empty<DetectedObjectDto>());
            }
        }

        public void AddDetectedFaces(VisionModelResponseDto faceDetectionResponse)
        {
            // TODO: put into queue, determine which faces are "visible"
            lock (DetectedFaces)
            {
                DetectedFaces.Clear();
                DetectedFaces.AddRange(faceDetectionResponse?.Objects ?? Array.Empty<DetectedObjectDto>());
            }
        }
    }
}
