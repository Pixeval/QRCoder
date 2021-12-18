using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.PixelFormats;

namespace QRCoder
{
    internal class Util
    {
        public static Color ConvertFromHtmlHex(string hex)
        {
            var argb = uint.Parse(hex[1..], NumberStyles.HexNumber);
            return new Color(new Argb32(argb));
        }

        public static Polygon ConvertRectangleToPolygon(RectangleF rectangle)
        {
            return new Polygon(new LinearLineSegment(
                new(rectangle.X, rectangle.Y),
                new(rectangle.X + rectangle.Width, rectangle.Y),
                new(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height),
                new(rectangle.X, rectangle.Y + rectangle.Height)));
        }

    }
}

