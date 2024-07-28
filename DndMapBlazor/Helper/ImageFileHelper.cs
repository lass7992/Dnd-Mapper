using Microsoft.AspNetCore.Components.Forms;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.CompilerServices;

namespace DndMapBlazor.Helper
{
    public static class ImageFileHelper
    {
        public static byte[] GetBytes(Stream stream)
        {
            var bytes = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.ReadAsync(bytes, 0, bytes.Length);
            stream.Dispose();
            return bytes;
        }
    }
}
