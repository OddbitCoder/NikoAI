using OddbitAi.Models.VisionDto;
using System.ComponentModel;

namespace OddbitAi.AudioRecorder
{
    public class Overlay : Label
    {
        private Pen faceAnnotationPen
            = new Pen(Color.Yellow, 8);
        private Pen objectAnnotationPen
            = new Pen(Color.Red, 8);
        private SolidBrush faceAnnotationLabelTextBrush
            = new SolidBrush(Color.Black);
        private SolidBrush faceAnnotationBrush
            = new SolidBrush(Color.Yellow);
        private SolidBrush objectAnnotationLabelTextBrush
            = new SolidBrush(Color.White);
        private SolidBrush objectAnnotationBrush
            = new SolidBrush(Color.Red);

        private List<DetectedObjectDto> detectedObjects
            = new();
        private List<DetectedObjectDto> detectedFaces
            = new();

        private void ChangePenColor(ref Pen pen, Color color)
        { 
            var penWidth = pen.Width;
            pen.Dispose();
            pen = new Pen(color, penWidth);
        }

        private void ChangePenWidth(ref Pen pen, float width)
        {
            var color = pen.Color;
            pen.Dispose();
            pen = new Pen(color, width);
        }

        private void ChangeBrushColor(ref SolidBrush brush, Color color)
        {
            brush.Dispose();
            brush = new SolidBrush(color);
        }

        [Category("Appearance")]
        public Color FaceAnnotationColor
        {
            get => faceAnnotationPen.Color;
            set
            {
                ChangePenColor(ref faceAnnotationPen, value);
                ChangeBrushColor(ref faceAnnotationBrush, value);
            }
        }

        [Category("Appearance")]
        public Color ObjectAnnotationColor
        {
            get => objectAnnotationPen.Color;
            set
            {
                ChangePenColor(ref objectAnnotationPen, value);
                ChangeBrushColor(ref objectAnnotationBrush, value);
            }
        }

        [Category("Appearance")]
        public float AnnotationLineWidth
        {
            get => faceAnnotationPen.Width;
            set 
            { 
                ChangePenWidth(ref faceAnnotationPen, value); 
                ChangePenWidth(ref objectAnnotationPen, value); 
            }
        }

        [Category("Appearance")]
        public Color FaceAnnotationLabelTextColor
        {
            get => faceAnnotationLabelTextBrush.Color;
            set => ChangeBrushColor(ref faceAnnotationLabelTextBrush, value);
        }

        [Category("Appearance")]
        public Color ObjectAnnotationLabelTextColor
        {
            get => objectAnnotationLabelTextBrush.Color;
            set => ChangeBrushColor(ref objectAnnotationLabelTextBrush, value);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Bindable(false)]
        [Browsable(false)]
        public new Color BackColor { get => base.BackColor; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Bindable(false)]
        [Browsable(false)]
        public Size FrameSize { get; set; }
            = new Size(640, 480);

        public void UpdateObjectAnnotations(IEnumerable<DetectedObjectDto> detectedObjects)
        {
            lock (this.detectedObjects)
            {
                this.detectedObjects.Clear();
                this.detectedObjects.AddRange(detectedObjects);
            }
        }

        public void UpdateFaceAnnotations(IEnumerable<DetectedObjectDto> detectedFaces)
        {
            lock (this.detectedFaces)
            {
                this.detectedFaces.Clear();
                this.detectedFaces.AddRange(detectedFaces);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Height <= 0 || Width <= 0)
            {
                return;
            }
            // compute video projection size
            double projWidth, projHeight;
            if ((double)Width / Height < (double)FrameSize.Width / FrameSize.Height) // projection is "wider" than the picture box
            {
                projWidth = Width;
                projHeight = Width / ((double)FrameSize.Width / FrameSize.Height);
            }
            else
            {
                projHeight = Height;
                projWidth = ((double)FrameSize.Width / FrameSize.Height) * Height;
            }
            // draw annotations
            double ofsX = (Width - projWidth) / 2;
            double ofsY = (Height - projHeight) / 2;
            lock (detectedObjects)
            {
                foreach (var obj in detectedObjects)
                {
                    float x = (float)(obj.X * projWidth + ofsX);
                    float y = (float)(obj.Y * projHeight + ofsY);
                    e.Graphics.DrawRectangle(objectAnnotationPen,
                        x,
                        y,
                        (float)(obj.Width * projWidth),
                        (float)(obj.Height * projHeight)
                        );
                    SizeF textSz = e.Graphics.MeasureString(obj.Name, Font);
                    e.Graphics.FillRectangle(objectAnnotationBrush,
                        x,
                        y,
                        textSz.Width,
                        textSz.Height
                        );
                    e.Graphics.DrawString(obj.Name, Font, objectAnnotationLabelTextBrush, x, y);
                }
            }
            lock (detectedFaces)
            {
                foreach (var obj in detectedFaces)
                {
                    float x = (float)(obj.X * projWidth + ofsX);
                    float y = (float)(obj.Y * projHeight + ofsY);
                    e.Graphics.DrawRectangle(faceAnnotationPen,
                        x,
                        y,
                        (float)(obj.Width * projWidth),
                        (float)(obj.Height * projHeight)
                        );
                    SizeF textSz = e.Graphics.MeasureString(obj.Name, Font);
                    e.Graphics.FillRectangle(faceAnnotationBrush,
                        x,
                        y,
                        textSz.Width,
                        textSz.Height
                        );
                    e.Graphics.DrawString(obj.Name, Font, faceAnnotationLabelTextBrush, x, y);
                }
            }
        }

        public Overlay()
        {
            base.BackColor = Color.Transparent;
        }
    }
}
