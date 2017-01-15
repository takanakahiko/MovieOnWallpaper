using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

class WallPaperEngine : System.Windows.Forms.Form {

    public delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

    [Flags]
    public enum SendMessageTimeoutFlags : uint
    {
        SMTO_NORMAL = 0x0,
        SMTO_BLOCK = 0x1,
        SMTO_ABORTIFHUNG = 0x2,
        SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
        SMTO_ERRORONEXIT = 0x20
    }

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessageTimeout(IntPtr windowHandle, uint Msg, IntPtr wParam, IntPtr lParam, SendMessageTimeoutFlags flags, uint timeout, out IntPtr result);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, IntPtr windowTitle);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    // 壁紙の取得と変更に用いる関数
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, StringBuilder pvParam, uint fWinIni);

    //
    private System.Windows.Forms.Integration.ElementHost elementHost;
    private System.Windows.Controls.MediaElement mediaElement;

    private System.Windows.Forms.NotifyIcon notifyIcon;
    private System.Windows.Forms.ContextMenu contextMenu;
    private System.Windows.Forms.MenuItem menuItem;

    const uint SPI_GETDESKWALLPAPER = 115;
    const uint SPI_SETDESKWALLPAPER = 20;
    const uint SPIF_UPDATEINIFILE = 1;
    const uint SPIF_SENDWININICHANGE = 2;

    [STAThread]
    static void Main(){
        Application.Run(new WallPaperEngine());
    }

    public WallPaperEngine(){

        Console.WriteLine("get wallpaper");

        // 壁紙の取得
        StringBuilder wp = new StringBuilder("");
        SystemParametersInfo(SPI_GETDESKWALLPAPER, 260, wp, 0);

        Console.WriteLine("set wallpaper");

        this.FormClosed += (s, e) =>{
            SystemParametersInfo(SPI_SETDESKWALLPAPER, (uint)wp.Length, wp, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        };

        Console.WriteLine("set WorkerW");

        // ProgmanにWorkerWを作る
        setWorkerW();

        Console.WriteLine("get WorkerW");

        // SHELLDLL_DefViewを配下に持つWorkerWを調べる
        IntPtr workerw = getWorkerW();

        Console.WriteLine("set Parent");

        //　WorkerWにフォームをぶら下げる
        SetParent(this.Handle, workerw);

        Console.WriteLine("init Form");

        // フォームの設定
        this.Text = "WallPaperEngine";
        this.FormBorderStyle = FormBorderStyle.None;
        this.Left = this.Top = 0;
        this.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
        this.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

        Console.WriteLine("init form parts");

        // フォーム内のパーツ
        this.elementHost = new System.Windows.Forms.Integration.ElementHost();
        this.mediaElement = new System.Windows.Controls.MediaElement();
        this.contextMenu = new System.Windows.Forms.ContextMenu();
        this.menuItem = new System.Windows.Forms.MenuItem();
        this.notifyIcon = new System.Windows.Forms.NotifyIcon();

        Console.WriteLine("init elementHost");

        // mediaElementを置くためのパーツ
        this.elementHost.Visible = true;
        this.elementHost.Dock = DockStyle.Fill;
        this.Controls.Add(this.elementHost);

        Console.WriteLine("init mediaElement");

        // 動画再生パーツ
        this.mediaElement.Visibility = System.Windows.Visibility.Visible;
        this.mediaElement.Margin = new System.Windows.Thickness(0, 0, 0, 0);
        this.mediaElement.UnloadedBehavior = System.Windows.Controls.MediaState.Manual;
        this.mediaElement.MediaEnded += (sender, eventArgs) =>　{
            this.mediaElement.Position = TimeSpan.FromMilliseconds(1);
            this.mediaElement.Play();
        };
        this.elementHost.Child = this.mediaElement;

        Console.WriteLine("load movie");

        // 動画読み込みダイアログの追加
        OpenFileDialog ofd = new OpenFileDialog();
        if (ofd.ShowDialog() == DialogResult.OK){
            this.mediaElement.Source = new Uri(ofd.FileName);
        }

        Console.WriteLine("add notifyIcon");

        // メニューにmenuItemを追加
        this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {this.menuItem});

        // menuItemとして終了ボタンを追加
        this.menuItem.Index = 0;
        this.menuItem.Text = "E&xit";
        this.menuItem.Click += new System.EventHandler(this.menuItem_Click);

        // タスクトレイのアイコンの設定
        this.notifyIcon.Icon = new Icon("favicon.ico");
        this.notifyIcon.ContextMenu = this.contextMenu;
        this.notifyIcon.Text = this.Text;
        this.notifyIcon.Visible = true;

    }

    private void setWorkerW(){
        IntPtr progman = FindWindow("Progman", null);
        IntPtr result = IntPtr.Zero;
        SendMessageTimeout(progman,0x052C,new IntPtr(0),IntPtr.Zero,SendMessageTimeoutFlags.SMTO_NORMAL,1000,out result);
    }

    private IntPtr getWorkerW(){
        IntPtr workerw = IntPtr.Zero;
        EnumWindows(new EnumWindowsProc((tophandle, topparamhandle) => {
            IntPtr p = FindWindowEx(tophandle,IntPtr.Zero,"SHELLDLL_DefView",IntPtr.Zero);
            if (p != IntPtr.Zero) workerw = FindWindowEx(IntPtr.Zero,tophandle,"WorkerW",IntPtr.Zero);
            return true;
        }), IntPtr.Zero);
        return workerw;
    }

    private void menuItem_Click(object Sender, EventArgs e) {
        this.Close();
    }

}
