namespace DndMapBlazor.Models
{
    public class Field : WorldMapEntity
    {
        public List<Wall> Walls { get; set; } = new List<Wall>();

        public int gridX { get; set; } = 4;
        public int gridY { get; set; } = 4;

        public int offsetXStart { get; set; }
        public int offsetXEnd { get; set; }
        public int offsetYStart { get; set; }
        public int offsetYEnd { get; set; }

        public Field()
        {
        }
    }
}
