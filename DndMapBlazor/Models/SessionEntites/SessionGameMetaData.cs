using Microsoft.AspNetCore.Components;

namespace DndMapBlazor.Models.SessionEntites
{
    public class SessionGameMetaData
    {
        public double DmImageWidth { get; set; }
        public double DmImageHeight { get; set; }

        /// <summary>
        /// cm pr 100px
        /// </summary>
        public double RealWorldScaling { get; set; }

        public double ClientWindowWidth { get; set; }
        public double ClientWindowHeight { get; set; }

        public bool UseCamara { get; set; } = false;

        /// <summary>
        /// Event for new data. Mostly used when DMimageWidth and DMimageHeight are changed
        /// </summary>
        public EventCallback UpdatedDataEvent { get; set; }


        //Maybe not used?
        public (double x, double y)[] CamaraPoints { get; set; } = new (double x, double y)[] { new(0, 0), new(200, 0), new(200, 200), new(0, 200) };

        public Session? Session { get; set; } = new Session();
    }
}
