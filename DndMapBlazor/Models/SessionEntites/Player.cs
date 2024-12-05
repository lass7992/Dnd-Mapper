using System.Drawing;

namespace DndMapBlazor.Models.SessionEntites
{
    public class Player : bordEntity
    {
        public string Name = "";
        public int PlayerId = 0;

        static int LastId = -1;
        int NextId { get { LastId = LastId < 8 ? LastId + 1 : 0; return LastId; }}
        public int color = 0;

        public Player()
        {
            var tempId = NextId;
            color = tempId * 75;
            PlayerId = tempId;
        }
    }
}
