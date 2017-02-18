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

  //
  private System.Windows.Forms.Integration.ElementHost elementHost;
  private System.Windows.Controls.MediaElement mediaElement;

  private System.Windows.Forms.NotifyIcon notifyIcon;
  private System.Windows.Forms.ContextMenu contextMenu;
  private System.Windows.Forms.MenuItem menuItem0;
  private System.Windows.Forms.MenuItem menuItem1;
  private System.Windows.Forms.MenuItem menuItem2;
  private System.Windows.Forms.MenuItem menuItem3;
  private System.Windows.Forms.MenuItem menuItem4;
  private System.Windows.Controls.WebBrowser webBrowser;

  Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(FEATURE_BROWSER_EMULATION);
  const string FEATURE_BROWSER_EMULATION = @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION";
  string process_name = System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe";
  string process_dbg_name = System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".vshost.exe";

  [STAThread]
  static void Main(){
    Application.Run(new WallPaperEngine());
  }

  public WallPaperEngine(){
    //下記処理（レジストリを）をリセットする
    this.FormClosing += Form1_FormClosing;

    //IEのレジストリキーを登録
    regkey.SetValue(process_name, 11001, Microsoft.Win32.RegistryValueKind.DWord);
    regkey.SetValue(process_dbg_name, 11001, Microsoft.Win32.RegistryValueKind.DWord);

    //壁紙にFormを張り付ける
    IntPtr progman = IntPtr.Zero;
    progman = Wallpainter.SetupWallpaper();
    if (progman == IntPtr.Zero) writeLog("Error : Failed to retrieve progman!");
    if (WinAPI.SetParent(this.Handle, progman) == IntPtr.Zero) writeLog("Error : Failed to set Parent!");
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
    this.menuItem3 = new System.Windows.Forms.MenuItem();
    this.menuItem4 = new System.Windows.Forms.MenuItem();
    this.notifyIcon = new System.Windows.Forms.NotifyIcon();
    this.webBrowser = new System.Windows.Controls.WebBrowser();

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

    writeLog("init webBrowser");
    this.webBrowser.Visibility = System.Windows.Visibility.Visible;
    this.webBrowser.Margin = new System.Windows.Thickness(0, 0, 0, 0);

    // 動画ソースの読み込み
    writeLog("load video");
    loadVideo();

    // メニューにmenuItemを追加
    writeLog("add notifyIcon");
    this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {this.menuItem0,this.menuItem1,this.menuItem2,this.menuItem3,this.menuItem4});

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

    // menuItem3として「作者に奢る」ボタンを追加
    this.menuItem3.Index = 3;
    this.menuItem3.Text = "Load URL";
    this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);

    // menuItem4として「作者に奢る」ボタンを追加
    this.menuItem4.Index = 4;
    this.menuItem4.Text = "作者に奢る";
    this.menuItem4.Click += new System.EventHandler(this.menuItem4_Click);

    // タスクトレイのアイコンの設定
    this.notifyIcon.Icon = new Icon("favicon.ico");
    this.notifyIcon.ContextMenu = this.contextMenu;
    this.notifyIcon.Text = this.Text;
    this.notifyIcon.Visible = true;

  }

  private void loadVideo(){
    // 動画読み込みダイアログの追加
    this.elementHost.Child = null;
    OpenFileDialog ofd = new OpenFileDialog();
    if (ofd.ShowDialog() == DialogResult.OK){
      this.mediaElement.Source = new Uri(ofd.FileName);
    }else{
      MessageBox.Show("動画が読み込めませんでした。\n読み込みなおすにはタスクトレイアイコンを右クリックして「Load Video」を選択してください。","エラー",MessageBoxButtons.OK,MessageBoxIcon.Error);
    }
    this.elementHost.Child = this.mediaElement;
  }

  private void loadURL(){
    // URL読み込みダイアログの追加
    this.mediaElement.Pause();
    this.elementHost.Child = null;
    string s1 = Microsoft.VisualBasic.Interaction.InputBox("メッセージを入力して下さい。");
    try{
        this.webBrowser.Navigate(s1);
        this.elementHost.Child = this.webBrowser;
    }catch (Exception e) {
        writeLog("url error : "+e.GetType().ToString());
        MessageBox.Show("無効なURLです。\n正しい形式のURLを指定してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        this.elementHost.Child = this.mediaElement;
    }
  }

  private void Form1_FormClosing(object sender, FormClosingEventArgs e){
      regkey.DeleteValue(process_name);
      regkey.DeleteValue(process_dbg_name);
      regkey.Close();
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

  private void menuItem3_Click(object Sender, EventArgs e) {
    this.loadURL();
  }

  private void menuItem4_Click(object Sender, EventArgs e) {
    System.Diagnostics.Process.Start("http://www.amazon.co.jp/registry/wishlist/2X1XQIFXKS456/ref=cm_sw_r_tw_ws_x_gBDOybXPBC2NP");
  }

  private void writeLog(string message){
    string appendText = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + message + Environment.NewLine;
    System.IO.File.AppendAllText("log.txt", appendText);
  }

}
