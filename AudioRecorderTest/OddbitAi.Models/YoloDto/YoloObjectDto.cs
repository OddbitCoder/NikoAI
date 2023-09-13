namespace OddbitAi.Models.YoloDto
{
    public class YoloObjectDto
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public int Class { get; set; }
        public string? Name { get; set; }
    }
}
