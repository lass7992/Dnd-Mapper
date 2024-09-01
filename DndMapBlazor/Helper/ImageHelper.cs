using Microsoft.AspNetCore.Components.Forms;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.CompilerServices;
using Microsoft.JSInterop;
using DndMapBlazor.Models.SessionEntites.PlayerBordCommunication;
using System.Reflection.Metadata.Ecma335;

namespace DndMapBlazor.Helper
{
    public static class ImageHelper
    {
        public static async Task<byte[]> GetBytes(Stream stream)
        {
            var bytes = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            await stream.ReadAsync(bytes, 0, bytes.Length);
            stream.Dispose();
            return bytes;
        }

        public static async Task<(int x, int y)?> GetMapSize(IJSRuntime JS, string ID)
        {
            var newSize = await JS.InvokeAsync<JsXYResult>("GetElementSize", ID);
            if (newSize.x == 0 || newSize.y == 0)
            {
                return null;
            }

            return (newSize.x, newSize.y);
        }

        public static async Task<WindowsSize?> GetWindowSize(IJSRuntime JS)
        {
            var newSize = await JS.InvokeAsync<JsXYResult>("GetWindowSize");
            if (newSize.x == 0 || newSize.y == 0)
            {
                return null;
            }

            return new WindowsSize() { width = newSize.x, height = newSize.y };
        }

        

        public class JsXYResult 
        {
            public int x { get; set; }
            public int y { get; set; }
        }


        public class WindowsSize
        {
            public int width { get; set; }
            public int height { get; set; }
        }
    }
}
