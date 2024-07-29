using Microsoft.AspNetCore.Components.Forms;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.CompilerServices;
using Microsoft.JSInterop;

namespace DndMapBlazor.Helper
{
    public static class ImageHelper
    {
        public static byte[] GetBytes(Stream stream)
        {
            var bytes = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.ReadAsync(bytes, 0, bytes.Length);
            stream.Dispose();
            return bytes;
        }

        public static async Task<(int x, int y)?> GetMapSize(IJSRuntime JS, string ID)
        {
            var newSize = await JS.InvokeAsync<JsXYResult>("GetElementSize", ID);
            if (newSize.x == null || newSize.y == null || newSize.x == 0 || newSize.y == 0)
            {
                return null;
            }

            return (newSize.x, newSize.y);
        }

        private class JsXYResult 
        {
            public int x { get; set; }
            public int y { get; set; }
        }
    }
}
