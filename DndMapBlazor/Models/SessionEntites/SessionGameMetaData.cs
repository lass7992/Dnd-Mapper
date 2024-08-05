namespace DndMapBlazor.Models.SessionEntites
{
    public class SessionGameMetaData
    {
        public double imageWidth { get; set; }
        public double imageHeight { get; set; }

        public double? RealWorldWidth { get; set; }
        public double? RealWorldHeight { get; set; }

        public double RealWorldScaling { get; set; }
    }
}
