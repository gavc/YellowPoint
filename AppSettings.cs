using System.Drawing;

namespace YellowPoint;

public sealed class AppSettings
{
    public int Diameter { get; set; } = 40;
    public double Opacity { get; set; } = 0.5;
    public int ColorArgb { get; set; } = Color.Gold.ToArgb();

    public Color HighlightColor => Color.FromArgb(ColorArgb);
}
