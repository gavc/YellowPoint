using System.ComponentModel;
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
        Enabled = false;
        Size = new Size(_settings.Diameter, _settings.Diameter);
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
            UpdateLayeredBitmap();
        }
        catch (Win32Exception ex)
        {
            AppLogger.LogException("Failed to initialize the layered overlay window.", ex);
        }
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
        Size = new Size(_settings.Diameter, _settings.Diameter);
        if (IsHandleCreated)
        {
            UpdateLayeredBitmap();
        }
    }

    internal void MoveTo(Point location)
    {
        if (Location != location)
        {
            Location = location;
        }
    }

    private void UpdateLayeredBitmap()
    {
        using var bitmap = OverlayBitmapRenderer.Render(_settings);
        NativeMethods.UpdateLayeredBitmap(Handle, bitmap, Location);
    }
}
