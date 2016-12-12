using Novacode;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace GraphView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TextBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            scrollViewer.ScrollToBottom();
        }

        private void btnTakePicture_Click(object sender, RoutedEventArgs e)
        {
            Bitmap graphImage = GetGraphImage();
            bool imageAdded = AddGraphImageToDoc("Graph output.docx", graphImage);

            if (!imageAdded)
            {
                MessageBox.Show("Неможливо додати знімок графа");
            }
        }

        private Bitmap GetGraphImage()
        {
            System.Windows.Point screenCoords = canvas.PointToScreen(new System.Windows.Point(0, 0));
            var rect = new System.Drawing.Rectangle((int)screenCoords.X, (int)screenCoords.Y, (int)canvas.ActualWidth, (int)canvas.ActualHeight);
            Bitmap bmp = new Bitmap(rect.Width, rect.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(rect.Left, rect.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);

            return bmp;
        }

        private bool AddGraphImageToDoc(string fileName, Bitmap bmp)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                ms.Seek(0, SeekOrigin.Begin);

                try
                {
                    if (System.IO.File.Exists(fileName))
                        using (DocX doc = DocX.Load(fileName))
                        {
                            Novacode.Image img = doc.AddImage(ms);
                            Paragraph p = doc.InsertParagraph();
                            p.Alignment = Alignment.center;
                            p.Append("\n");
                            Picture pic1 = img.CreatePicture();
                            p.InsertPicture(pic1, 0);
                            doc.Save();
                        }
                    else
                        using (DocX doc = DocX.Create(fileName))
                        {
                            Novacode.Image img = doc.AddImage(ms); // Create image.
                            Paragraph p = doc.InsertParagraph();
                            p.Alignment = Alignment.center;
                            p.Append("\n");
                            Picture pic1 = img.CreatePicture();
                            p.InsertPicture(pic1, 0);
                            doc.Save();
                        }
                }
                catch(Exception e)
                {
                    return false;
                }

                return true;
            }
        }
    }
}
