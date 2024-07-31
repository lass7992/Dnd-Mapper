using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace DndMapBlazor.Models
{
    public class WorldMapEntity 
    {
        public WorldMapEntity()
        {
        }

        public WorldMapEntity(double x, double y, double width, double height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public double x { get; set; } = 200000;
        public double y { get; set; } = 200000;
        public double width { get; set; } 
        public double height { get; set; }
        public List<Points> points { get; set; } = new List<Points>();
        public string name { get; set; } = "";
        public string? mapImage { get; set; }

        [JsonIgnore]
        public Zone? ParentZone { get; set; }

        public void AddPoint(double x, double y)
        {
            if (x < this.x) 
            {
                var diff = this.x - x;
                foreach (var point in points) 
                {
                    point.x += diff;
                    if (point.x > this.width)
                    {
                        width = point.x;
                    }
                }
                this.x = x;
            }
            if (y < this.y)
            {
                var diff = this.y - y;
                foreach (var point in points)
                {
                    point.y += diff;

                    if (point.y > this.height)
                    {
                        height = point.y;
                    }
                }
                this.y = y;
            }

            x -= this.x;
            y -= this.y;

            if (x > this.width) 
            { 
                width = x;
            }
            if (y > this.height)
            {
                height = y;
            }

            points.Add(new Points(x, y));
        }

        public void RemoveWallpoint(Points point)
        {
            points.Add(point);
        }

        public void RemoveWallpoint(int index)
        {
            if (points.Count == 0)
            {
                return;
            }

            if (index < 0)
            {
                index = points.Count + index;
            }
            points.RemoveAt(index);
        }
    }
}
