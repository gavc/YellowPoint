using System.Diagnostics;

namespace YellowPoint;

public sealed class SettingsForm : Form
{
    private readonly NumericUpDown _diameterInput;
    private readonly NumericUpDown _opacityInput;

    public AppSettings UpdatedSettings { get; private set; }

    public SettingsForm(AppSettings current)
    {
        Text = "YellowPoint Settings v202601";
        AutoScaleMode = AutoScaleMode.Dpi;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(340, 180);

        var diameterLabel = new Label
        {
            Text = "Diameter (px):",
            AutoSize = true,
            Location = new Point(20, 20)
        };

        _diameterInput = new NumericUpDown
        {
            Minimum = 10,
            Maximum = 200,
            Value = Math.Clamp(current.Diameter, 10, 200),
            Location = new Point(160, 16),
            Width = 120
        };

        var opacityLabel = new Label
        {
            Text = "Opacity (%):",
            AutoSize = true,
            Location = new Point(20, 60)
        };

        _opacityInput = new NumericUpDown
        {
            Minimum = 10,
            Maximum = 100,
            Value = (decimal)(Math.Clamp(current.Opacity, 0.1, 1.0) * 100),
            Location = new Point(160, 56),
            Width = 120
        };

        var okButton = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Location = new Point(120, 110),
            Width = 80,
            AutoSize = true,
            MinimumSize = new Size(80, 32)
        };

        var cancelButton = new Button
        {
            Text = "Cancel",
            DialogResult = DialogResult.Cancel,
            Location = new Point(210, 110),
            Width = 80,
            AutoSize = true,
            MinimumSize = new Size(80, 32)
        };

        var repoLink = new LinkLabel
        {
            Text = "YellowPoint",
            AutoSize = true,
            LinkColor = Color.RoyalBlue,
            ActiveLinkColor = Color.DodgerBlue,
            VisitedLinkColor = Color.MediumPurple,
            Location = new Point(20, 120),
            Anchor = AnchorStyles.Left | AnchorStyles.Bottom
        };

        repoLink.LinkClicked += (_, _) =>
        {
            Process.Start(new ProcessStartInfo("https://github.com/gavc/YellowPoint")
            {
                UseShellExecute = true
            });
        };

        Controls.AddRange(new Control[] { diameterLabel, _diameterInput, opacityLabel, _opacityInput, okButton, cancelButton, repoLink });

        AcceptButton = okButton;
        CancelButton = cancelButton;

        UpdatedSettings = new AppSettings
        {
            Diameter = current.Diameter,
            Opacity = current.Opacity,
            ColorArgb = current.ColorArgb
        };

        FormClosing += (_, e) =>
        {
            if (DialogResult == DialogResult.OK)
            {
                UpdatedSettings = new AppSettings
                {
                    Diameter = (int)_diameterInput.Value,
                    Opacity = (double)_opacityInput.Value / 100d,
                    ColorArgb = current.ColorArgb
                };
            }
        };
    }
}
