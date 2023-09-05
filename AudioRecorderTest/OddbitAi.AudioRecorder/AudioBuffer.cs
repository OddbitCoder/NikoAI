using NAudio.Wave;

namespace OddbitAi.AudioRecorder
{
    internal class AudioBuffer
    {
        private class QueueItem
        {
            public byte DataByte { get; set; }
            public DateTime EndTime { get; set; }
        }

        private readonly int bufferSize; // in bytes
        private DateTime? endTime 
            = null;
        private readonly int bytesPerSecond;

        private readonly Queue<QueueItem> rawData
            = new();

        public byte[] RawData 
            => rawData.Select(x => x.DataByte).ToArray();
        public int RawByteCount
            => rawData.Count;
        public DateTime? StartTime
            => rawData.Count > 0 ? rawData.First().EndTime : null;
        public DateTime? EndTime
            => endTime;
        public string SnapshotId
            => string.Format($"{StartTime:yyyyMMddHHmmss.fff}-{EndTime:yyyyMMddHHmmss.fff}");

        public void Clear()
        {
            rawData.Clear();
            endTime = null;
        }

        public AudioBuffer(int bufferSize, WaveFormat wavInFmt)
        {
            this.bufferSize = bufferSize;
            this.bytesPerSecond = wavInFmt.SampleRate * wavInFmt.Channels * (wavInFmt.BitsPerSample / 8);
        }

        public void WriteData(byte[] buffer, int bufferLen)
        {
            endTime = DateTime.UtcNow;
            for (int i = 0; i < bufferLen; i++) 
            {
                rawData.Enqueue(new QueueItem {
                    DataByte = buffer[i],
                    EndTime = endTime.Value - TimeSpan.FromSeconds((double)(bufferLen - i) / bytesPerSecond)
                });
                while (rawData.Count > bufferSize) 
                {
                    rawData.Dequeue();
                }
            }
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
