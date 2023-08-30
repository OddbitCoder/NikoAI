using NAudio.Wave;

namespace OddbitAi.AudioRecorder
{
    internal class AudioBuffer
    {
        private readonly int bufferSize; // in bytes
        private long writeTimestamp = -1;
        private long startTimestamp = -1;
        private readonly WaveFormat wavInFmt;
        private readonly int bytesPerSecond;

        private readonly Queue<byte> rawData
            = new();

        public byte[] RawData 
            => rawData.ToArray();
        public int RawByteCount
            => rawData.Count;
        public long EndTimestampTicks
            => writeTimestamp;
        public long StartTimestampTicks
            => startTimestamp;
        public DateTime? EndTimestampUtc
            => writeTimestamp >= 0 ? new(writeTimestamp, DateTimeKind.Utc) : null;
        public DateTime? StartTimestampUtc
            => startTimestamp >= 0 ? new(startTimestamp, DateTimeKind.Utc) : null;

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
            writeTimestamp = DateTime.UtcNow.Ticks;
            for (int i = 0; i < bufferLen; i++) 
            {
                rawData.Enqueue(buffer[i]);
                while (rawData.Count > bufferSize) 
                {
                    rawData.Dequeue();
                }
            }
            double seconds = (double)rawData.Count / bytesPerSecond;
            long ticks = (long)(seconds * TimeSpan.TicksPerSecond);
            startTimestamp = writeTimestamp - ticks;
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
