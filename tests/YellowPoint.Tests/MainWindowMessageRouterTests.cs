using Xunit;

namespace YellowPoint.Tests;

public sealed class MainWindowMessageRouterTests
{
    [Fact]
    public void SessionQueryUsesDefaultWindowProcessing()
    {
        var action = MainWindowMessageRouter.GetAction(
            NativeMethods.WM_QUERYENDSESSION,
            IntPtr.Zero);

        Assert.Equal(MainWindowMessageAction.Default, action);
    }
}
