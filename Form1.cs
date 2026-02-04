namespace YellowPoint;

public partial class MainForm : Form
{
    private const int HotkeyId = 1;
    private readonly NotifyIcon _trayIcon;
    private readonly ContextMenuStrip _trayMenu;
    private readonly ToolStripMenuItem _toggleItem;
    private readonly ToolStripMenuItem _settingsItem;
    private readonly ToolStripMenuItem _exitItem;
    private readonly OverlayForm _overlay;
    private readonly System.Windows.Forms.Timer _cursorTimer;
    private readonly SettingsService _settingsService;
    private AppSettings _settings;
    private bool _highlightEnabled;
    private Icon? _appIcon;
    private bool _hotkeyRegistered;

    public MainForm()
    {
        InitializeComponent();

        Text = "YellowPoint";
        ShowInTaskbar = false;
        WindowState = FormWindowState.Minimized;
        Visible = false;

        _settingsService = new SettingsService();
        _settings = _settingsService.Load();

        _overlay = new OverlayForm(_settings);

        var iconPath = Path.Combine(AppContext.BaseDirectory, "cursor.ico");
        if (File.Exists(iconPath))
        {
            _appIcon = new Icon(iconPath);
            Icon = _appIcon;
        }

        _toggleItem = new ToolStripMenuItem("Toggle Highlight", null, (_, _) => ToggleHighlight());
        _settingsItem = new ToolStripMenuItem("Settings", null, (_, _) => OpenSettings());
        _exitItem = new ToolStripMenuItem("Exit", null, (_, _) => ExitApp());

        _trayMenu = new ContextMenuStrip();
        _trayMenu.Items.AddRange(new ToolStripItem[] { _toggleItem, _settingsItem, new ToolStripSeparator(), _exitItem });

        _trayIcon = new NotifyIcon
        {
            Icon = _appIcon ?? SystemIcons.Information,
            Visible = true,
            Text = "YellowPoint",
            ContextMenuStrip = _trayMenu
        };

        _trayIcon.DoubleClick += (_, _) => ToggleHighlight();

        _cursorTimer = new System.Windows.Forms.Timer { Interval = 16 };
        _cursorTimer.Tick += (_, _) => UpdateOverlayPosition();

        UpdateTrayState();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        RegisterHotkey();
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        UnregisterHotkey();
        base.OnHandleDestroyed(e);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        Hide();
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == NativeMethods.WM_HOTKEY && m.WParam.ToInt32() == HotkeyId)
        {
            ToggleHighlight();
            return;
        }

        base.WndProc(ref m);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _cursorTimer.Stop();
        _cursorTimer.Dispose();
        _overlay.Dispose();
        _trayIcon.Visible = false;
        _trayIcon.Icon = null;
        _trayIcon.Dispose();
        _trayMenu.Dispose();
        _appIcon?.Dispose();
        base.OnFormClosing(e);
    }

    private void ToggleHighlight()
    {
        _highlightEnabled = !_highlightEnabled;

        if (_highlightEnabled)
        {
            _overlay.ApplySettings(_settings);
            _overlay.Show();
            _cursorTimer.Start();
            UpdateOverlayPosition();
        }
        else
        {
            _cursorTimer.Stop();
            _overlay.Hide();
        }

        UpdateTrayState();
    }

    private void UpdateOverlayPosition()
    {
        if (!_highlightEnabled)
        {
            return;
        }

        var cursor = Cursor.Position;
        var radius = _settings.Diameter / 2;
        _overlay.SetBounds(cursor.X - radius, cursor.Y - radius, _settings.Diameter, _settings.Diameter);
        _overlay.Invalidate();
    }

    private void OpenSettings()
    {
        using var dialog = new SettingsForm(_settings);
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _settings = dialog.UpdatedSettings;
            _settingsService.Save(_settings);
            _overlay.ApplySettings(_settings);
            if (_highlightEnabled)
            {
                UpdateOverlayPosition();
            }
        }
    }

    private void ExitApp()
    {
        Close();
    }

    private void UpdateTrayState()
    {
        _toggleItem.Checked = _highlightEnabled;
    }

    private void RegisterHotkey()
    {
        if (!NativeMethods.RegisterHotKey(Handle, HotkeyId, NativeMethods.MOD_CONTROL | NativeMethods.MOD_ALT, (int)Keys.Y))
        {
            _trayIcon.ShowBalloonTip(3000, "YellowPoint", "Failed to register hotkey Ctrl+Alt+Y. It may be in use by another application.", ToolTipIcon.Warning);
            _hotkeyRegistered = false;
            return;
        }

        _hotkeyRegistered = true;
    }

    private void UnregisterHotkey()
    {
        if (_hotkeyRegistered)
        {
            _ = NativeMethods.UnregisterHotKey(Handle, HotkeyId);
            _hotkeyRegistered = false;
        }
    }
}
