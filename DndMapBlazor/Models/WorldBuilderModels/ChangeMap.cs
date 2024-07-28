namespace DndMapBlazor.Models.WorldBuilderModels
{
    public class ChangeMap
    {
        public string? FromImage;
        public string? ToImage;
        public int? xPos;
        public int? yPos;
        public bool? ZoomIn;
        public int height;

        public ChangeMap()
        {
        }

        public ChangeMap(string fromImage, string? toImage, int xPos, int yPos, bool zoomIm)
        {
            FromImage = fromImage;
            ToImage = toImage;
            this.xPos = xPos;
            this.yPos = yPos;
            ZoomIn = zoomIm;
        }
    }
}
