using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace QRCoderTests.Helpers
{
    public static class HelperFunctions
    {
        
        public static string GetAssemblyPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        
        public static string BitmapToHash(Image<Bgra32> bmp)
        {
            using var ms = new MemoryStream();
            bmp.Save(ms, PngFormat.Instance);
            return ByteArrayToHash(ms.ToArray());
        }

        public static string ByteArrayToHash(byte[] data)
        {
            var hash = MD5.HashData(data);
            return Convert.ToHexString(hash).ToLower();
        }

        public static string StringToHash(string data)
        {
            return ByteArrayToHash(Encoding.UTF8.GetBytes(data));
        }
    }
}
