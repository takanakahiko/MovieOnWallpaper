using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;


public class Mirror : System.Windows.Forms.Form {
  private System.Windows.Forms.Integration.ElementHost elementHost;
  public Mirror(System.Windows.Forms.Screen sc, System.Windows.Forms.Integration.ElementHost ehost) {
    //prevent freeze
    System.Threading.Thread.Sleep(2000);

    //壁紙にFormを張り付ける
    IntPtr progman = IntPtr.Zero;
    progman = Wallpainter.SetupWallpaper();
    if (progman == IntPtr.Zero) Utilities.writeLog("Error : Failed to retrieve progman!");
    if (WinAPI.SetParent(this.Handle, progman) == IntPtr.Zero) Utilities.writeLog("Error : Failed to set Parent!");
    WinAPI.ShowWindowAsync(this.Handle, 1);

    //MirrorのWPFコントロール用にElementHostを設定
    this.elementHost = new System.Windows.Forms.Integration.ElementHost();
    this.elementHost.Visible = true;
    this.elementHost.Dock = DockStyle.Fill;

    // フォームの設定
    Utilities.writeLog("init Mirror Form");
    this.Text = "WallPaperEngine";
    this.FormBorderStyle = FormBorderStyle.None;

    //渡されたディスプレイ(sc)にミラーフォームのサイズを調整
    SetDisplay setdsp = new SetDisplay(this);
    setdsp.setDsp(sc);

    //マスターの内容をコピーするためにVisual Brushを設定
    VisualBrush vBrush = new VisualBrush();
    vBrush.TileMode = System.Windows.Media.TileMode.None;
    //Visual Brushにキャッシュを設定
    RenderOptions.SetCachingHint(vBrush, CachingHint.Cache);
    RenderOptions.SetCacheInvalidationThresholdMinimum(vBrush, 0.5);
    RenderOptions.SetCacheInvalidationThresholdMaximum(vBrush, 2.0);
    //マスターのElementHost.ChildをVisual Brushに設定
    vBrush.Visual = ehost.Child;

    //Visual Brush描画用Rectangle(WPF)を設定
    System.Windows.Shapes.Rectangle mirrorRect = new System.Windows.Shapes.Rectangle();
    mirrorRect.Width = this.Width;
    mirrorRect.Height = this.Height;
    mirrorRect.Fill = vBrush;

    //MirrorのElementHostにVisual Brush描画用Rectangleを関連づける
    this.elementHost.Child = mirrorRect;

    //MirrorのELementHostのサイズを調整してフォームのコントロールに登録
    this.elementHost.SetBounds(0, 0, this.Width, this.Height);
    this.Controls.Add(this.elementHost);
  }
}
