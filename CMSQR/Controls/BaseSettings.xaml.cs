using QRCoder;
using System.Windows;
using System.IO;
using System.Drawing;
using System.Windows.Media.Imaging;
using CMSQR.ViewModel;
using System.Drawing.Imaging;
using System.Windows.Controls;

namespace CMSQR.Controls
{
    /// <summary>
    /// Interaction logic for BaseSettings.xaml
    /// </summary>
    public partial class BaseSettings : UserControl
    {
        public BaseSettings()
        {
            InitializeComponent();
            this.DataContext = MainVM.Singleton;
        }

        private void PlainButton_Click(object sender, RoutedEventArgs e)
        {
            if (Paths.Text == "")
            {
                Generate.IsEnabled = false;
            }
            else
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(Paths.Text, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(20);
                QrCode.Source = BitmapToImageSource(qrCodeImage);
            }
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Bmp);
            stream.Position = 0;
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.StreamSource = stream;
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.EndInit();
            return img;
        }

        private void PlainButton_Click_1(object sender, RoutedEventArgs e)
        {
            // Create a file dialog to let the user choose where to save the image
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            //dialog.FileName = "Save QRCode As";
            dialog.DefaultExt = ".png";
            dialog.Filter = "PNG files (*.png)|*.png|JPEG|*.jpg|BMP|*.bmp|GIF|*.gif|All files (*.*)|*.*|PDF files (*.pdf)|*.pdf";

            // Show the dialog and get the result
            bool? result = dialog.ShowDialog();

            // If the user clicked "OK", save the image to the selected file
            if (result == true)
            {
                // Create a BitmapEncoder to encode the image data
                PngBitmapEncoder encoder = new PngBitmapEncoder();

                // Add the image to the encoder
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)QrCode.Source));

                // Save the image to the selected file
                using (FileStream stream = new FileStream(dialog.FileName, FileMode.Create))
                {
                    encoder.Save(stream);
                }
            }
        }
    }
}
