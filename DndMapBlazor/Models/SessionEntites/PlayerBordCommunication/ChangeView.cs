using System.Security.Cryptography.X509Certificates;

namespace DndMapBlazor.Models.SessionEntites.PlayerBordCommunication
{
    public class ChangeView
    {
        public double xStart { get; set; }
        public double yStart { get; set; }
        public double xEnd { get; set; }
        public double yEnd { get; set; }
    }
}
