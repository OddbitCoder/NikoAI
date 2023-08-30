using NAudio.Wave;

namespace OddbitAi.AudioRecorder
{
    internal class AudioBuffer
    {
        private readonly int bufferSize; // in bytes
        private DateTime? writeTimestamp 
            = null;
        private DateTime? startTimestamp 
            = null;
        private readonly WaveFormat wavInFmt;
        private readonly int bytesPerSecond;

        private readonly Queue<byte> rawData
            = new();

        public byte[] RawData 
            => rawData.ToArray();
        public int RawByteCount
            => rawData.Count;
        public DateTime? EndTimestampUtc
            => writeTimestamp;
        public DateTime? StartTimestampUtc
            => startTimestamp;
        public string SnapshotId
            => string.Format($"{StartTimestampUtc:yyyyMMddHHmmss.fff}-{EndTimestampUtc:yyyyMMddHHmmss.fff}");

        public void Clear()
        {
            rawData.Clear();
        }

        public AudioBuffer(int bufferSize, WaveFormat wavInFmt)
        {
            this.wavInFmt = wavInFmt;
            this.bufferSize = bufferSize;
            this.bytesPerSecond = wavInFmt.SampleRate * wavInFmt.Channels * (wavInFmt.BitsPerSample / 8);
        }

        public void WriteData(byte[] buffer, int bufferLen)
        {
            writeTimestamp = DateTime.UtcNow;
            for (int i = 0; i < bufferLen; i++) 
            {
                rawData.Enqueue(buffer[i]);
                while (rawData.Count > bufferSize) 
                {
                    rawData.Dequeue();
                }
            }
            double seconds = (double)rawData.Count / bytesPerSecond;
            startTimestamp = writeTimestamp - TimeSpan.FromSeconds(seconds);
        }

        public void WriteToFile(string fileName, WaveFormat? wavFormat = null)
        {
            wavFormat ??= wavInFmt;
            using (var writer = new WaveFileWriter(fileName, wavFormat))
            {
                var rawData = RawData; // make snapshot
                writer.Write(rawData, 0, rawData.Length);
                writer.Flush();
            }
        }

        public byte[] GetWavBytes(WaveFormat? wavFormat = null)
        {
            wavFormat ??= wavInFmt;
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
