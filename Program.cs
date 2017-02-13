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
    //
    private System.Windows.Forms.Integration.ElementHost elementHost;
    private System.Windows.Controls.MediaElement mediaElement;

    private System.Windows.Forms.NotifyIcon notifyIcon;
    private System.Windows.Forms.ContextMenu contextMenu;
    private System.Windows.Forms.MenuItem menuItem0;
    private System.Windows.Forms.MenuItem menuItem1;
    private System.Windows.Forms.MenuItem menuItem2;

    const int SPI_GETDESKWALLPAPER = 115;
    const int SPI_SETDESKWALLPAPER = 20;
    const int SPIF_UPDATEINIFILE = 1;
    const int SPIF_SENDWININICHANGE = 2;

    [STAThread]
    static void Main(){
        Application.Run(new WallPaperEngine());
    }

    public WallPaperEngine(){

        IntPtr progman = IntPtr.Zero;
        progman = Wallpainter.SetupWallpaper();

        if (progman == IntPtr.Zero)
            writeLog("Error : Failed to retrieve progman!");

        if (WinAPI.SetParent(this.Handle, progman) == IntPtr.Zero)
            writeLog("Error : Failed to set Parent!");

        WinAPI.ShowWindowAsync(this.Handle, 1);


        // フォームの設定
        writeLog("init Form");
        this.Text = "WallPaperEngine";
        this.FormBorderStyle = FormBorderStyle.None;
        this.Left = this.Top = 0;
        this.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
        this.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

        // フォーム内のパーツ
        writeLog("init form parts");
        this.elementHost = new System.Windows.Forms.Integration.ElementHost();
        this.mediaElement = new System.Windows.Controls.MediaElement();
        this.contextMenu = new System.Windows.Forms.ContextMenu();
        this.menuItem0 = new System.Windows.Forms.MenuItem();
        this.menuItem1 = new System.Windows.Forms.MenuItem();
        this.menuItem2 = new System.Windows.Forms.MenuItem();
        this.notifyIcon = new System.Windows.Forms.NotifyIcon();

        // mediaElementを置くためのパーツ
        writeLog("init elementHost");
        this.elementHost.Visible = true;
        this.elementHost.Dock = DockStyle.Fill;
        this.Controls.Add(this.elementHost);

        // 動画再生パーツ
        writeLog("init mediaElement");
        this.mediaElement.Visibility = System.Windows.Visibility.Visible;
        this.mediaElement.Margin = new System.Windows.Thickness(0, 0, 0, 0);
        this.mediaElement.UnloadedBehavior = System.Windows.Controls.MediaState.Manual;
        this.mediaElement.MediaEnded += (sender, eventArgs) =>　{
            this.mediaElement.Position = TimeSpan.FromMilliseconds(1);
            this.mediaElement.Play();
        };
        this.elementHost.Child = this.mediaElement;

        // 動画ソースの読み込み
        writeLog("load video");
        loadVideo();

        // メニューにmenuItemを追加
        writeLog("add notifyIcon");
        this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {this.menuItem0,this.menuItem1,this.menuItem2});

        // menuItem0として終了ボタンを追加
        this.menuItem0.Index = 0;
        this.menuItem0.Text = "E&xit";
        this.menuItem0.Click += new System.EventHandler(this.menuItem0_Click);

        // menuItem1としてミュートボタンを追加
        this.menuItem1.Index = 1;
        this.menuItem1.Text = "Mute Audio";
        this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
        this.menuItem1.Checked = this.mediaElement.IsMuted;

        // menuItem2としてミュートボタンを追加
        this.menuItem2.Index = 2;
        this.menuItem2.Text = "Load Video";
        this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);

        // タスクトレイのアイコンの設定
        this.notifyIcon.Icon = new Icon("favicon.ico");
        this.notifyIcon.ContextMenu = this.contextMenu;
        this.notifyIcon.Text = this.Text;
        this.notifyIcon.Visible = true;

    }

    private void loadVideo(){
      // 動画読み込みダイアログの追加
      OpenFileDialog ofd = new OpenFileDialog();
      if (ofd.ShowDialog() == DialogResult.OK){
          this.mediaElement.Source = new Uri(ofd.FileName);
      }
    }

    private void menuItem0_Click(object Sender, EventArgs e) {
        this.Close();
    }

    private void menuItem1_Click(object Sender, EventArgs e) {
        this.mediaElement.IsMuted = !this.mediaElement.IsMuted;
        this.menuItem1.Checked = this.mediaElement.IsMuted;
    }

    private void menuItem2_Click(object Sender, EventArgs e) {
        this.loadVideo();
    }

    private void writeLog(string message){
      string appendText = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + message + Environment.NewLine;
      System.IO.File.AppendAllText("log.txt", appendText);
      //Console.WriteLine(appendText);
    }

}
