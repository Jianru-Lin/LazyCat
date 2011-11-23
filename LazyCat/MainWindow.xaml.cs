using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.IO;

namespace LazyCat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                installedHandle = hwndSource.Handle;
                viewerHandle = SetClipboardViewer(installedHandle);
                hwndSource.AddHook(new HwndSourceHook(this.hwndSourceHook));
            } 
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            ChangeClipboardChain(this.installedHandle, this.viewerHandle);
            int error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
            e.Cancel = error != 0;

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            this.viewerHandle = IntPtr.Zero;
            this.installedHandle = IntPtr.Zero;
            base.OnClosed(e);
        }

        IntPtr hwndSourceHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_CHANGECBCHAIN:
                    this.viewerHandle = lParam;
                    if (this.viewerHandle != IntPtr.Zero)
                    {
                        SendMessage(this.viewerHandle, msg, wParam, lParam);
                    }

                    break;

                case WM_DRAWCLIPBOARD:
                    EventArgs clipChange = new EventArgs();
                    OnClipboardChanged(clipChange);

                    if (this.viewerHandle != IntPtr.Zero)
                    {
                        SendMessage(this.viewerHandle, msg, wParam, lParam);
                    }

                    break;

            }
            return IntPtr.Zero;
        }

        private void OnClipboardChanged(EventArgs clipChange)
        {
            try
            {
                if (Clipboard.ContainsData(DataFormats.Bitmap))
                {
                    BitmapSource source = (System.Windows.Interop.InteropBitmap)Clipboard.GetData(DataFormats.Bitmap);
                    c++;
                    this.image.SetImage(source);
                    using (FileStream fs = new FileStream(string.Format("{0}.png", c), FileMode.Create, FileAccess.Write))
                    {
                        PngBitmapEncoder enc = new PngBitmapEncoder();
                        enc.Frames.Add(BitmapFrame.Create(source));
                        enc.Save(fs);
                        fs.Close();
                        fs.Dispose();
                    }
                }
            }
            catch { }
        }

        int c = 0;

        #region 声明
        IntPtr viewerHandle = IntPtr.Zero;
        IntPtr installedHandle = IntPtr.Zero;

        const int WM_DRAWCLIPBOARD = 0x308;
        const int WM_CHANGECBCHAIN = 0x30D;

        [DllImport("user32.dll")]
        private extern static IntPtr SetClipboardViewer(IntPtr hWnd);

        [DllImport("user32.dll")]
        private extern static int ChangeClipboardChain(IntPtr hWnd, IntPtr hWndNext);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private extern static int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
        #endregion

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Process.Start(Environment.CurrentDirectory, null);
        }
    }
}
