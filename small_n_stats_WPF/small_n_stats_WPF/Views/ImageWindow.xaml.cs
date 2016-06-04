using Microsoft.Win32;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace small_n_stats_WPF.Views
{
    /// <summary>
    /// Interaction logic for ImageWindow.xaml
    /// </summary>
    public partial class ImageWindow : Window
    {
        public string filePath = null;

        public ImageWindow()
        {
            InitializeComponent();
        }

        private void saveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "TIFF Image|*.tiff|PNG Image|*.png|Jpeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            saveFileDialog1.Title = "Save an Image File";
            saveFileDialog1.ShowDialog();

            string mExt = Path.GetExtension(saveFileDialog1.FileName);

            WpfDrawingSettings settings = new WpfDrawingSettings();
            settings.IncludeRuntime = true;
            settings.TextAsGeometry = false;

            ImageSvgConverter converter = new ImageSvgConverter(settings);

            if (mExt.Equals(".jpg"))
            {
                converter.EncoderType = ImageEncoderType.JpegBitmap;
                converter.Convert(filePath, saveFileDialog1.FileName);
            }
            else if (mExt.Equals(".png"))
            {
                converter.EncoderType = ImageEncoderType.PngBitmap;
                converter.Convert(filePath, saveFileDialog1.FileName);
            }
            else if (mExt.Equals(".gif"))
            {
                converter.EncoderType = ImageEncoderType.GifBitmap;
                converter.Convert(filePath, saveFileDialog1.FileName);
            }
            else if (mExt.Equals(".tiff"))
            {
                converter.EncoderType = ImageEncoderType.TiffBitmap;
                converter.Convert(filePath, saveFileDialog1.FileName);
            }
        }

        private void closeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
