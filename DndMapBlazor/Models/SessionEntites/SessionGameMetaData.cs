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
        public double CamaraRealWorldWidth { get; set; }
        public double CamaraRealWorldHeight { get; set; }

        public Session? session { get; set; } = new Session();
    }
}
