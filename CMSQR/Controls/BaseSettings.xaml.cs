using QRCoder;
using System.Windows;
using System.IO;
using System.Drawing;
using System.Windows.Media.Imaging;
using CMSQR.ViewModel;
using System.Drawing.Imaging;
using System.Windows.Controls;
using iTextSharp.text;
using iTextSharp.text.pdf;

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
                //After Image saved, The saved path reflected in QRImage textbox
                QRImage.Text = dialog.FileName;
            }
        }

        private void MergePDF_Click(object sender, RoutedEventArgs e)
        {

            string imgPath = QRImage.Text;
            string pdfPath = PDFFile.Text;

            //Loading the existing PDF file
            PdfReader reader = new PdfReader(pdfPath);

            //Create a new PDF Document
            Document document = new Document(PageSize.A4);

            //Create a PDFWriter Object
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(pdfPath.Replace(".pdf", " WithQRCode.pdf"), FileMode.OpenOrCreate)); //Replace .pdf to WithQRcode.pdf

            //Open the document and add the existing PDF file to it
            document.Open();
            document.AddDocListener(writer);
            PdfContentByte canvas = writer.DirectContentUnder;
            PdfImportedPage page = writer.GetImportedPage(reader, 1);
            canvas.AddTemplate(page, 0, 0);

            //Add the generated QR image to the modified PDF file
            Bitmap qrImage = new Bitmap(imgPath);
            iTextSharp.text.Image pdfImage = iTextSharp.text.Image.GetInstance(qrImage, ImageFormat.Jpeg);

            // Add the QR code to the PDF file at the selected position
            ComboBoxItem SelectedPosition = (ComboBoxItem)cmbx.SelectedItem;
            if (SelectedPosition.Content.ToString() == "Top-Left")
            {
                // QRcode Position at Left-Top in PDF
                pdfImage.SetAbsolutePosition(0, 680);  //LtoR-0 TtoB-680
            }
            else if (SelectedPosition.Content.ToString() == "Top-Right")
            {
                // QRcode Position at Right-Top in PDF
                pdfImage.SetAbsolutePosition(445, 680);
            }
            else if (SelectedPosition.Content.ToString() == "Center")
            {
                // QRcode Position at Center in PDF
                pdfImage.SetAbsolutePosition(200, 300); //LtoR-200 TtoB-300
            }
            else if (SelectedPosition.Content.ToString() == "Bottom-Left")
            {
                // QRcode Position at Left-Bottom in PDF
                pdfImage.SetAbsolutePosition(0, 0);
            }
            else if (SelectedPosition.Content.ToString() == "Bottom-Right")
            {
                // QRcode Position at Right-Bottom in PDF
                pdfImage.SetAbsolutePosition(445, 0);
            }

            //Choose the Size of the QRCode
            ComboBoxItem Size = (ComboBoxItem)cmbx_Size.SelectedItem;
            if (Size.Content.ToString() == "Small")
            {
                // QRcode Size Small
                pdfImage.ScaleToFit(60, 60);
            }
            else if (Size.Content.ToString() == "Medium")
            {
                // QRcode Size Medium
                pdfImage.ScaleToFit(90, 90);
            }
            else if (Size.Content.ToString() == "Large")
            {
                // QRcode Size Large
                pdfImage.ScaleToFit(120, 120); 
            }

            document.Add(pdfImage);

            //Close the document and the pdfWriter object
            document.Close();
            writer.Close();
            MessageBox.Show("Merging Done..", "Result Window");
        }
    }
}