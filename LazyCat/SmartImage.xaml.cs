using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace LazyCat
{
    /// <summary>
    /// Interaction logic for SmartImage.xaml
    /// </summary>
    public partial class SmartImage : UserControl
    {
        public SmartImage()
        {
            InitializeComponent();
        }

        public void SetURL(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                BitmapImage bitmapImage = new BitmapImage(uri);
                image.Source = bitmapImage;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void SetImage(BitmapSource source)
        {
            image.Source = source;
            updateImageSize();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            updateImageSize();
        }

        void updateImageSize()
        {
            if (image.Source != null)
            {
                if (image.Source.Width > this.ActualWidth || image.Source.Height > this.ActualHeight)
                {
                    image.Width = this.ActualWidth;
                    image.Height = this.ActualHeight;
                }
                else
                {
                    image.Width = image.Source.Width;
                    image.Height = image.Source.Height;
                }
            }
        }
    }
}
