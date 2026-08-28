using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace YellowPoint;

internal static class OverlayBitmapRenderer
{
    public static Bitmap Render(AppSettings settings)
    {
        var diameter = Math.Clamp(settings.Diameter, 10, 200);
        var opacity = Math.Clamp(settings.Opacity, 0.1, 1.0);
        var alpha = (int)Math.Round(opacity * byte.MaxValue, MidpointRounding.AwayFromZero);
        var bitmap = new Bitmap(diameter, diameter, PixelFormat.Format32bppPArgb);

        using var graphics = Graphics.FromImage(bitmap);
        graphics.CompositingMode = CompositingMode.SourceCopy;
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        graphics.Clear(Color.Transparent);

        using var brush = new SolidBrush(Color.FromArgb(alpha, settings.HighlightColor));
        graphics.FillEllipse(brush, 0.5f, 0.5f, diameter - 1f, diameter - 1f);

        return bitmap;
    }
}
