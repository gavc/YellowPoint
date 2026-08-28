using System.Drawing;
using Xunit;

namespace YellowPoint.Tests;

public sealed class OverlayFormTests
{
    [Fact]
    public void ConstructorDoesNotReserveASelectableTransparencyColor()
    {
        using var overlay = new OverlayForm(new AppSettings
        {
            ColorArgb = Color.Magenta.ToArgb()
        });

        Assert.Equal(Color.Empty, overlay.TransparencyKey);
    }

    [Fact]
    public void MoveToChangesLocationWithoutInvalidatingSurface()
    {
        using var overlay = new OverlayForm(new AppSettings());
        var invalidationCount = 0;
        overlay.Invalidated += (_, _) => invalidationCount++;

        overlay.MoveTo(new Point(120, 80));

        Assert.Equal(new Point(120, 80), overlay.Location);
        Assert.Equal(0, invalidationCount);
    }
}
