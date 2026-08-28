using System.Drawing;
using Xunit;

namespace YellowPoint.Tests;

public sealed class OverlayBitmapRendererTests
{
    [Fact]
    public void RenderSupportsMagentaWithPerPixelAlpha()
    {
        var settings = new AppSettings
        {
            Diameter = 40,
            Opacity = 0.5,
            ColorArgb = Color.Magenta.ToArgb()
        };

        using var bitmap = OverlayBitmapRenderer.Render(settings);

        Assert.Equal(new Size(40, 40), bitmap.Size);

        var center = bitmap.GetPixel(20, 20);
        Assert.Equal(128, center.A);
        Assert.Equal(Color.Magenta.R, center.R);
        Assert.Equal(Color.Magenta.G, center.G);
        Assert.Equal(Color.Magenta.B, center.B);
        Assert.Equal(0, bitmap.GetPixel(0, 0).A);

        var hasAntialiasedEdge = false;
        for (var y = 0; y < bitmap.Height && !hasAntialiasedEdge; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var alpha = bitmap.GetPixel(x, y).A;
                if (alpha is > 0 and < 128)
                {
                    hasAntialiasedEdge = true;
                    break;
                }
            }
        }

        Assert.True(hasAntialiasedEdge);
    }
}
