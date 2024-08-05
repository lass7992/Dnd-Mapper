using System.Text.Json.Serialization;

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

        [JsonIgnore]
        public List<WorldMapEntity> MapEntities { get; set; } = new List<WorldMapEntity>();


        public List<Zone> zones { get; set; } = new List<Zone>();
        public List<Field> fields { get; set; } = new List<Field>();

        public void LoadZone() 
        {
            MapEntities.AddRange(zones);
            MapEntities.AddRange(fields);
            foreach (var zone in zones) 
            {
                zone.ParentZone = this;
                zone.LoadZone();
            }
            foreach (var field in fields)
            {
                field.ParentZone = this;
            }

        }
        public void SaveZone() 
        {
            zones = MapEntities.Where(x => x is Zone).Select(x => x as Zone).ToList();
            fields = MapEntities.Where(x => x is Field).Select(x => x as Field).ToList();
            foreach (var zone in zones)
            {
                zone.SaveZone();
            }
        }
    }
}
