#pragma warning disable CA1416

using System.ComponentModel;
using OddbitAi.Niko.Models;

namespace OddbitAi.Niko.Components
{
    public partial class TranscriptViewer : Control
    {
        private readonly List<TranscriptItem> textSnippets
            = new();

        private const float bigNumber
            = 100000;
        private Brush? brush
            = null;

        protected override void OnPaint(PaintEventArgs e)
        {
            lock (textSnippets)
            {
                float y = Height;
                for (int i = textSnippets.Count - 1; i >= 0; i--)
                {
                    var snippet = textSnippets[i];
                    if (snippet != null)
                    {
                        Console.WriteLine(snippet.Text);
                        var size = e.Graphics.MeasureString(snippet.Text, Font, Width);
                        y -= size.Height;
                        brush?.Dispose();
                        e.Graphics.DrawString(snippet.Text, Font, brush = new SolidBrush(snippet.Color), new RectangleF(0, y, Width, bigNumber));
                        if (y < 0) { break; }
                    }
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Bindable(false)]
        [Browsable(false)]
        public void SetSnippets(List<TranscriptItem> textSnippets)
        {
            lock (this.textSnippets)
            {
                this.textSnippets.Clear();
                this.textSnippets.AddRange(textSnippets);
            }
            Refresh();
        }

        public TranscriptViewer()
        {
            SetStyle(ControlStyles.DoubleBuffer, true);
        }
    }
}
