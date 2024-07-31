namespace DndMapBlazor.Models
{
    public class Field : WorldMapEntity
    { 
        public List<Wall> Walls { get; set; } = new List<Wall>();

        public int gridX = 4;
        public int gridY = 4;

        public int offsetXStart;
        public int offsetXEnd;
        public int offsetYStart;
        public int offsetYEnd;

        public Field()
        {
        }
    }
}
