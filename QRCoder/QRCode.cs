#if NETFRAMEWORK || NETSTANDARD2_0 || NET6_0
using System;
using System.Drawing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using static QRCoder.QRCodeGenerator;
using Color = SixLabors.ImageSharp.Color;
using Point = SixLabors.ImageSharp.Point;
using PointF = SixLabors.ImageSharp.PointF;
using Rectangle = SixLabors.ImageSharp.Rectangle;
using RectangleF = SixLabors.ImageSharp.RectangleF;

namespace QRCoder
{
    public class QRCode : AbstractQRCode, IDisposable
    {
        /// <summary>
        /// Constructor without params to be used in COM Objects connections
        /// </summary>
        public QRCode() { }

        public QRCode(QRCodeData data) : base(data) { }

        public Image<Bgra32> GetGraphic(int pixelsPerModule)
        {
            return this.GetGraphic(pixelsPerModule, Color.Black, Color.White, true);
        }

        public Image<Bgra32> GetGraphic(int pixelsPerModule, string darkColorHtmlHex, string lightColorHtmlHex, bool drawQuietZones = true)
        {
            return this.GetGraphic(pixelsPerModule, Util.ConvertFromHtmlHex(darkColorHtmlHex), Util.ConvertFromHtmlHex(lightColorHtmlHex), drawQuietZones);
        }

        public Image<Bgra32> GetGraphic(int pixelsPerModule, Color darkColor, Color lightColor, bool drawQuietZones = true)
        {
            var size = (this.QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : 8)) * pixelsPerModule;
            var offset = drawQuietZones ? 0 : 4 * pixelsPerModule;
            var lightBrush = new SolidBrush(lightColor);
            var darkBrush = new SolidBrush(darkColor);
            var bmp = new Image<Bgra32>(size, size);
            bmp.Mutate(_ =>
            {
                for (var x = 0; x < size + offset; x = x + pixelsPerModule)
                {
                    for (var y = 0; y < size + offset; y = y + pixelsPerModule)
                    {
                        var module = this.QrCodeData.ModuleMatrix[(y + pixelsPerModule) / pixelsPerModule - 1][(x + pixelsPerModule) / pixelsPerModule - 1];

                        if (module)
                        {
                            _.Fill(darkBrush, new Rectangle(x - offset, y - offset, pixelsPerModule, pixelsPerModule));
                        }
                        else
                        {
                            _.Fill(lightBrush, new Rectangle(x - offset, y - offset, pixelsPerModule, pixelsPerModule));
                        }
                    }
                }
            });

            return bmp;
        }

        internal IPath CreateRoundedRectanglePath(RectangleF rect, int cornerRadius)
        {
            var roundedRect = new PathBuilder();
            roundedRect.AddEllipticalArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2,90f, 180f, 90f);
            roundedRect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
            roundedRect.AddEllipticalArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 90f, 270, 90);
            roundedRect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
            roundedRect.AddEllipticalArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90f, 0, 90);
            roundedRect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
            roundedRect.AddEllipticalArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90f, 90, 90);
            roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
            roundedRect.CloseFigure();
            return roundedRect.Build();
        }
    }

    public static class QRCodeHelper
    {
        public static Image<Bgra32> GetQRCode(string plainText, int pixelsPerModule, Color darkColor, Color lightColor, ECCLevel eccLevel, bool forceUtf8 = false, bool utf8BOM = false, EciMode eciMode = EciMode.Default, int requestedVersion = -1, bool drawQuietZones = true)
        {
            using (var qrGenerator = new QRCodeGenerator())
            using (var qrCodeData = qrGenerator.CreateQrCode(plainText, eccLevel, forceUtf8, utf8BOM, eciMode, requestedVersion))
            using (var qrCode = new QRCode(qrCodeData))
                return qrCode.GetGraphic(pixelsPerModule, darkColor, lightColor, drawQuietZones);
        }
    }
}

#endif