using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace YellowPoint;

public sealed class OverlayForm : Form
{
    private AppSettings _settings;

    public OverlayForm(AppSettings settings)
    {
        _settings = settings;

        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        TopMost = true;
        StartPosition = FormStartPosition.Manual;
        BackColor = Color.Magenta;
        TransparencyKey = Color.Magenta;
        Opacity = Math.Clamp(_settings.Opacity, 0.1, 1.0);
        DoubleBuffered = true;
        Enabled = false;
    }

    protected override bool ShowWithoutActivation => true;

    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            cp.ExStyle |= NativeMethods.WS_EX_NOACTIVATE;
            cp.ExStyle |= NativeMethods.WS_EX_TRANSPARENT;
            cp.ExStyle |= NativeMethods.WS_EX_LAYERED;
            cp.ExStyle |= NativeMethods.WS_EX_TOOLWINDOW;
            return cp;
        }
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        try
        {
            NativeMethods.EnableClickThrough(Handle);
        }
        catch (Win32Exception ex)
        {
            AppLogger.LogException("Failed to apply click-through overlay styles.", ex);
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var brush = new SolidBrush(_settings.HighlightColor);
        e.Graphics.FillEllipse(brush, 0, 0, Width - 1, Height - 1);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == NativeMethods.WM_NCHITTEST)
        {
            m.Result = (IntPtr)NativeMethods.HTTRANSPARENT;
            return;
        }

        base.WndProc(ref m);
    }

    public void ApplySettings(AppSettings settings)
    {
        _settings = settings;
        Opacity = Math.Clamp(_settings.Opacity, 0.1, 1.0);
        Invalidate();
    }
}
