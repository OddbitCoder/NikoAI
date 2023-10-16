using OddbitAi.Models.VisionDto;

namespace OddbitAi.AudioRecorder
{
    public class OverlayManager
    {
        private readonly PictureBox pb;
        private Bitmap bmp;
        private (int width, int height) pbSz 
            = (-1, -1);
        private (int width, int height) frameSz
            = (-1, -1);
        private List<DetectedObjectDto> detectedObjects
            = new();
        private List<DetectedObjectDto> detectedFaces
            = new();

        private Pen penDetectedObjects 
            = new Pen(Color.Red, 8);
        private Pen penDetectedFaces 
            = new Pen(Color.Yellow, 8);
        private Font font
            = new Font("Arial", 28, FontStyle.Regular, GraphicsUnit.Pixel);
        private Brush textBrushDetectedFaces
            = Brushes.Black;
        private Brush textBkBrushDetectedFaces
            = Brushes.Yellow;
        private Brush textBrushDetectedObjects
            = Brushes.White;
        private Brush textBkBrushDetectedObjects
            = Brushes.Red;

        public OverlayManager(PictureBox pbOverlay, int frameWidth, int frameHeight)
        { 
            pb = pbOverlay;
            frameSz = (frameWidth, frameHeight);
        }

        public void UpdateObjects(IEnumerable<DetectedObjectDto> detectedObjects)
        {
            lock (this.detectedObjects)
            {
                this.detectedObjects.Clear();
                this.detectedObjects.AddRange(detectedObjects);
            }
        }

        public void UpdateFaces(IEnumerable<DetectedObjectDto> detectedFaces)
        {
            lock (this.detectedFaces)
            {
                this.detectedFaces.Clear();
                this.detectedFaces.AddRange(detectedFaces);
            }
        }

        public void Paint()
        {
            if (pb.Height <= 0 || pb.Width <= 0)
            {
                return;
            }
            // prepare "canvas"
            if (pbSz.width != pb.Width || pbSz.height != pb.Height)
            {
                var b = new Bitmap(pb.Width, pb.Height);
                pb.Image = b;
                bmp?.Dispose();
                bmp = b;
                pbSz = (pb.Width, pb.Height);
            }
            // redraw annotations 
            // compute video projection size
            double projWidth, projHeight;
            if ((double)pbSz.width / pbSz.height < (double)frameSz.width / frameSz.height) // projection is more "wide" than the picture box
            {
                projWidth = pbSz.width;
                projHeight = pbSz.width / ((double)frameSz.width / frameSz.height);
            }
            else
            {
                projHeight = pbSz.height;
                projWidth = ((double)frameSz.width / frameSz.height) * pbSz.height;
            }
            //Console.WriteLine($"{projWidth} {projHeight}");
            double ofsX = (pbSz.width - projWidth) / 2;
            double ofsY = (pbSz.height - projHeight) / 2;
            using (var gfx = Graphics.FromImage(bmp))
            {
                gfx.Clear(Color.Transparent);
                lock (detectedObjects)
                {
                    foreach (var obj in detectedObjects)
                    {
                        float x = (float)(obj.X * projWidth + ofsX);
                        float y = (float)(obj.Y * projHeight + ofsY);
                        gfx.DrawRectangle(penDetectedObjects,
                            x,
                            y,
                            (float)(obj.Width * projWidth),
                            (float)(obj.Height * projHeight)
                            );
                        SizeF textSz = gfx.MeasureString(obj.Name, font);
                        gfx.FillRectangle(textBkBrushDetectedObjects,
                            x,
                            y,
                            textSz.Width,
                            textSz.Height
                            );
                        gfx.DrawString(obj.Name, font, textBrushDetectedObjects, new PointF(x, y));
                    }
                }
                lock (detectedFaces)
                {
                    foreach (var obj in detectedFaces)
                    {
                        float x = (float)(obj.X * projWidth + ofsX);
                        float y = (float)(obj.Y * projHeight + ofsY);
                        gfx.DrawRectangle(penDetectedFaces,
                            x,
                            y,
                            (float)(obj.Width * projWidth),
                            (float)(obj.Height * projHeight)
                            );
                        SizeF textSz = gfx.MeasureString(obj.Name, font);
                        gfx.FillRectangle(textBkBrushDetectedFaces,
                            x,
                            y,
                            textSz.Width,
                            textSz.Height
                            );
                        gfx.DrawString(obj.Name, font, textBrushDetectedFaces, new PointF(x, y));
                    }
                }
            }
        }
    }
}
