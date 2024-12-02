using System.Drawing;

namespace DndMapBlazor.Models.SessionEntites
{
    public class Player : bordEntity
    {
        public string Name = "";

        static int LastColor = -1;
        int NextColor { get { LastColor = LastColor < 8 ? LastColor + 1 : 0; return LastColor; }}
        public int color = 0;

        public Player()
        {
            color = NextColor * 75;
        }
    }
}
