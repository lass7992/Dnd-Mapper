namespace DndMapBlazor.Models
{
    public class Zone : WorldMapEntity
    {
        public Zone()
        {
        }

        public Zone(double x, double y, double width, double height) : base(x, y, width, height)
        {
        }

        public List<WorldMapEntity> MapEntities { get; set; } = new List<WorldMapEntity>();
    }
}
