using System;
using System.Text;
using System.Runtime.InteropServices;

class WinAPI
{
  public static UInt32 WM_SPAWN_WORKER = 0x052C;
  public static UInt32 WM_CLOSE = 0x0010;

  public enum WindowLongFlags : int{
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
  public enum SendMessageTimeoutFlags : uint{
    SMTO_NORMAL = 0x0,
    SMTO_BLOCK = 0x1,
    SMTO_ABORTIFHUNG = 0x2,
    SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
    SMTO_ERRORONEXIT = 0x20
  }

  public static class SetWindowPosFlags{
    public static readonly uint
    NOSIZE = 0x0001,
    NOMOVE = 0x0002,
    NOZORDER = 0x0004,
    NOREDRAW = 0x0008,
    NOACTIVATE = 0x0010,
    DRAWFRAME = 0x0020,
    FRAMECHANGED = 0x0020,
    SHOWWINDOW = 0x0040,
    HIDEWINDOW = 0x0080,
    NOCOPYBITS = 0x0100,
    NOOWNERZORDER = 0x0200,
    NOREPOSITION = 0x0200,
    NOSENDCHANGING = 0x0400,
    DEFERERASE = 0x2000,
    ASYNCWINDOWPOS = 0x4000;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct RECT{
    public int Left;        // x position of upper-left corner
    public int Top;         // y position of upper-left corner
    public int Right;       // x position of lower-right corner
    public int Bottom;      // y position of lower-right corner

    public int Width { get { return Right - Left; } }
    public int Height { get { return Bottom - Top; } }

    public RECT(int x, int y, int w, int h)
    {
      Left = x;
      Top = y;
      Right = w + x;
      Bottom = y + h;
    }
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

  [DllImport("user32.dll")]
  public static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

  [DllImport("user32.dll", SetLastError = true)]
  public static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

  //Set window min/max/normal status
  [DllImport("user32.dll")]
  public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

  [DllImport("user32.dll", SetLastError = true)]
  public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

  [DllImport("user32.dll", SetLastError = true)]
  public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

}

class Wallpainter{
  public static IntPtr GetProgman(){
    return WinAPI.FindWindow("Progman", null);
  }

  public static IntPtr SetupWallpaper(){
    IntPtr progman = GetProgman();
    WinAPI.SendMessage(progman, WinAPI.WM_SPAWN_WORKER, IntPtr.Zero, IntPtr.Zero);

    IntPtr workerw = IntPtr.Zero;
    WinAPI.EnumWindows(new WinAPI.EnumWindowsProc((tophandle, topparamhandle) =>{
      IntPtr p = WinAPI.FindWindowEx(tophandle, IntPtr.Zero, "SHELLDLL_DefView", null);
      if (p != IntPtr.Zero) workerw = WinAPI.FindWindowEx(IntPtr.Zero, tophandle, "WorkerW", null);
      return true;
    }), IntPtr.Zero);

    WinAPI.ShowWindowAsync(workerw, 0);

    return progman;
  }
}
