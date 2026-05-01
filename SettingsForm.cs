using System.Diagnostics;

namespace YellowPoint;

public sealed class SettingsForm : Form
{
    private readonly NumericUpDown _diameterInput;
    private readonly NumericUpDown _opacityInput;
    private readonly Button _colorButton;
    private readonly Label _colorPreviewLabel;
    private int _selectedColorArgb;

    public AppSettings UpdatedSettings { get; private set; }

    public SettingsForm(AppSettings current)
    {
        _selectedColorArgb = current.ColorArgb;
        var productVersion = Application.ProductVersion;
        Text = string.IsNullOrWhiteSpace(productVersion)
            ? "YellowPoint Settings"
            : $"YellowPoint Settings v{productVersion}";
        AutoScaleMode = AutoScaleMode.Dpi;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(340, 220);

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

        var colorLabel = new Label
        {
            Text = "Color:",
            AutoSize = true,
            Location = new Point(20, 100)
        };

        _colorButton = new Button
        {
            Text = "",
            Location = new Point(160, 96),
            Size = new Size(40, 24),
            BackColor = Color.FromArgb(_selectedColorArgb),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        _colorButton.FlatAppearance.BorderSize = 1;
        _colorButton.FlatAppearance.BorderColor = SystemColors.GrayText;
        _colorButton.Click += (_, _) => ChooseColor();

        _colorPreviewLabel = new Label
        {
            Text = GetColorDisplayName(Color.FromArgb(_selectedColorArgb)),
            AutoSize = true,
            Location = new Point(210, 100),
            ForeColor = SystemColors.GrayText
        };

        var okButton = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Location = new Point(120, 150),
            Width = 80,
            AutoSize = true,
            MinimumSize = new Size(80, 32)
        };

        var cancelButton = new Button
        {
            Text = "Cancel",
            DialogResult = DialogResult.Cancel,
            Location = new Point(210, 150),
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
            Location = new Point(20, 160),
            Anchor = AnchorStyles.Left | AnchorStyles.Bottom
        };

        repoLink.LinkClicked += (_, _) =>
        {
            Process.Start(new ProcessStartInfo("https://github.com/gavc/YellowPoint")
            {
                UseShellExecute = true
            });
        };

        Controls.AddRange(new Control[] { diameterLabel, _diameterInput, opacityLabel, _opacityInput, colorLabel, _colorButton, _colorPreviewLabel, okButton, cancelButton, repoLink });

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
                    ColorArgb = _selectedColorArgb
                };
            }
        };
    }

    private void ChooseColor()
    {
        using var dialog = new ColorDialog
        {
            FullOpen = true,
            Color = Color.FromArgb(_selectedColorArgb),
            AnyColor = true,
            SolidColorOnly = true
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _selectedColorArgb = dialog.Color.ToArgb();
            _colorButton.BackColor = dialog.Color;
            _colorPreviewLabel.Text = GetColorDisplayName(dialog.Color);
        }
    }

    private static string GetColorDisplayName(Color color)
    {
        // Try to find a known color name, otherwise show hex
        foreach (KnownColor kc in Enum.GetValues<KnownColor>())
        {
            var known = Color.FromKnownColor(kc);
            if (known.ToArgb() == color.ToArgb() && !known.IsSystemColor)
            {
                return kc.ToString();
            }
        }
        return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
    }
}
