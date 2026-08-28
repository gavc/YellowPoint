namespace YellowPoint;

internal enum MainWindowMessageAction
{
    Default,
    ToggleHighlight
}

internal static class MainWindowMessageRouter
{
    public const int HotkeyId = 1;

    public static MainWindowMessageAction GetAction(int message, IntPtr wParam)
    {
        return message == NativeMethods.WM_HOTKEY && wParam.ToInt32() == HotkeyId
            ? MainWindowMessageAction.ToggleHighlight
            : MainWindowMessageAction.Default;
    }
}
