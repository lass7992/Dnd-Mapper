namespace DndMapBlazor.Models
{
    public class Wall
    {
        public int width = 5;
        public List<Points> WallPoints = new List<Points>();

        public void AddWallpoint(double x, double y) 
        {
            WallPoints.Add(new Points(x, y));        
        }

        public void RemoveWallpoint(Points point)
        {
            WallPoints.Add(point);
        }

        public void RemoveWallpoint(int index)
        {
            if (WallPoints.Count == 0) {
                return;
            }

            if (index < 0) 
            { 
                index = WallPoints.Count + index;
            }
            WallPoints.RemoveAt(index);
        }
    }

    public class Points 
    {

        public double x, y;

        public Points(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
