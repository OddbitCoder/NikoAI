using NAudio.Wave;

namespace OddbitAi.AudioRecorder
{
    internal class AudioBuffer
    {
        private readonly int bufferSize; // in bytes
        private DateTime? startTimestamp
            = null;
        private DateTime? endTimestamp 
            = null;
        private readonly int bytesPerSecond;

        private readonly Queue<byte> rawData
            = new();

        public byte[] RawData 
            => rawData.ToArray();
        public int RawByteCount
            => rawData.Count;
        public DateTime? StartTimestampUtc
            => startTimestamp;
        public DateTime? EndTimestampUtc
            => endTimestamp;
        public string SnapshotId
            => string.Format($"{StartTimestampUtc:yyyyMMddHHmmss.fff}-{EndTimestampUtc:yyyyMMddHHmmss.fff}");

        public void Clear()
        {
            rawData.Clear();
        }

        public AudioBuffer(int bufferSize, WaveFormat wavInFmt)
        {
            this.bufferSize = bufferSize;
            this.bytesPerSecond = wavInFmt.SampleRate * wavInFmt.Channels * (wavInFmt.BitsPerSample / 8);
        }

        public void WriteData(byte[] buffer, int bufferLen)
        {
            endTimestamp = DateTime.UtcNow;
            for (int i = 0; i < bufferLen; i++) 
            {
                rawData.Enqueue(buffer[i]);
                while (rawData.Count > bufferSize) 
                {
                    rawData.Dequeue();
                }
            }
            double seconds = (double)rawData.Count / bytesPerSecond;
            startTimestamp = endTimestamp - TimeSpan.FromSeconds(seconds);
        }

        public void WriteToFile(string fileName, WaveFormat wavFormat)
        {
            using (var writer = new WaveFileWriter(fileName, wavFormat))
            {
                var rawData = RawData; // make snapshot
                writer.Write(rawData, 0, rawData.Length);
                writer.Flush();
            }
        }

        public byte[] GetWavBytes(WaveFormat wavFormat)
        {
            var ms = new MemoryStream();
            using (var writer = new WaveFileWriter(ms, wavFormat))
            {
                var rawData = RawData; // make snapshot
                writer.Write(rawData, 0, rawData.Length);
                writer.Flush();
                var msBuffer = ms.GetBuffer();
                var destBuffer = new byte[msBuffer.Length];
                Array.Copy(msBuffer, destBuffer, msBuffer.Length);
                return destBuffer;
            }
        }
    }
}
