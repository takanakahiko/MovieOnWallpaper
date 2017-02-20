using System;
using System.Text;
using System.Runtime.InteropServices;

class WinAPI {
    public static UInt32 WM_SPAWN_WORKER = 0x052C;

    public enum WindowLongFlags : int {
        GWL_EXSTYLE = -20,
        GWLP_HINSTANCE = -6,
        GWLP_HWNDPARENT = -8,
        GWL_ID = -12,
        GWL_STYLE = -16,
        GWL_USERDATA = -21,
        GWL_WNDPROC = -4,
        DWLP_USER = 0x8,
        DWLP_MSGRESULT = 0x0,
        DWLP_DLGPROC = 0x4,
    }

    [Flags]
    public enum SendMessageTimeoutFlags : uint {
        SMTO_NORMAL = 0x0,
        SMTO_BLOCK = 0x1,
        SMTO_ABORTIFHUNG = 0x2,
        SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
        SMTO_ERRORONEXIT = 0x20
    }

    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll")]
    internal static extern bool EnumChildWindows(IntPtr hwnd, EnumWindowsProc func, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessageTimeout(IntPtr windowHandle, uint Msg, IntPtr wParam, IntPtr lParam, SendMessageTimeoutFlags flags, uint timeout, out IntPtr result);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

    //Set window min/max/normal status
    [DllImport("user32.dll")]
    public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

}

class Wallpainter {
    public static IntPtr GetProgman() {
        return WinAPI.FindWindow("Progman", null);
    }

    public static IntPtr SetupWallpaper() {
        IntPtr progman = GetProgman();
        WinAPI.SendMessage(progman, WinAPI.WM_SPAWN_WORKER, IntPtr.Zero, IntPtr.Zero);

        IntPtr workerw = IntPtr.Zero;
        WinAPI.EnumWindows(new WinAPI.EnumWindowsProc((tophandle, topparamhandle) => {
            IntPtr p = WinAPI.FindWindowEx(tophandle, IntPtr.Zero, "SHELLDLL_DefView", null);
            if (p != IntPtr.Zero) workerw = WinAPI.FindWindowEx(IntPtr.Zero, tophandle, "WorkerW", null);
            return true;
        }), IntPtr.Zero);

        WinAPI.ShowWindowAsync(workerw, 0);

        return progman;
    }
}
