namespace DndMapBlazor.Models.WorldBuilderModels
{
    public class ChangeMap
    {
        public string? FromImage;
        public string? ToImage;
        public double? xPos;
        public double? yPos;
        public bool? ZoomIn;
        public double height;

        public ChangeMap()
        {
        }
    }
}
