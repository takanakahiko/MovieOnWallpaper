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


public class Mirror : System.Windows.Forms.Form
{
    private System.Windows.Controls.MediaElement mediaElement;
    private System.Windows.Forms.Integration.ElementHost elementHost;
    private System.Windows.Controls.Grid baseGrid;
    public Mirror(System.Windows.Forms.Screen sc, System.Windows.Forms.Integration.ElementHost ehost)
    {
        //壁紙にFormを張り付ける
        IntPtr progman = IntPtr.Zero;
        progman = Wallpainter.SetupWallpaper();
        if (progman == IntPtr.Zero) Utillities.writeLog("Error : Failed to retrieve progman!");
        if (WinAPI.SetParent(this.Handle, progman) == IntPtr.Zero) Utillities.writeLog("Error : Failed to set Parent!");
        WinAPI.ShowWindowAsync(this.Handle, 1);

        this.elementHost = new System.Windows.Forms.Integration.ElementHost();
        this.elementHost.Visible = true;
        this.elementHost.Dock = DockStyle.Fill;

        // フォームの設定
        Utillities.writeLog("init Mirror Form");
        this.Text = "WallPaperEngine";
        this.FormBorderStyle = FormBorderStyle.None;
        SetDisplay setdsp = new SetDisplay(this);
        setdsp.setDsp(sc);

        VisualBrush vBrush = new VisualBrush();
        vBrush.TileMode = System.Windows.Media.TileMode.None;
        vBrush.Visual = ehost.Child;

        System.Windows.Shapes.Rectangle mirrorRect = new System.Windows.Shapes.Rectangle();
        mirrorRect.Width = this.Width;
        mirrorRect.Height = this.Height;
        mirrorRect.Fill = vBrush;

        this.elementHost.Child = mirrorRect;
        this.elementHost.SetBounds(0, 0, this.Width, this.Height);
        this.Controls.Add(this.elementHost);
    }
}
