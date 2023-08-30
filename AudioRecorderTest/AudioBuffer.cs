using NAudio.Wave;

namespace OddbitAi.AudioRecorder
{
    internal class AudioBuffer
    {
        private int bufferSize; // number of bytes

        private readonly Queue<byte> rawData
            = new();

        public byte[] RawData 
            => rawData.ToArray();
        public int RawByteCount
            => rawData.Count;

        public void Clear()
        {
            rawData.Clear();
        }

        public AudioBuffer(int bufferSize)
        {
            this.bufferSize = bufferSize;
        }

        public void WriteData(byte[] buffer, int bufferLen)
        {
            for (int i = 0; i < bufferLen; i++) 
            {
                rawData.Enqueue(buffer[i]);
                while (rawData.Count > bufferSize) 
                {
                    rawData.Dequeue();
                }
            }
        }

        public void WriteToFile(string fileName, WaveFormat wavFormat)
        {
            //Console.WriteLine(fileName);
            //Console.WriteLine(RawByteCount);
            using (WaveFileWriter writer = new WaveFileWriter(fileName, wavFormat))
            {
                var rawData = RawData; // snapshot
                writer.Write(rawData, 0, rawData.Length);
                writer.Flush();
            }
        }

        public byte[] GetWavBytes(WaveFormat wavFormat)
        {
            var ms = new MemoryStream();
            using (WaveFileWriter writer = new WaveFileWriter(ms, wavFormat))
            {
                var rawData = RawData; // snapshot
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
