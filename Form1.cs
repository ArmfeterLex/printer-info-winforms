using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PrinterApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();  
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            foreach (string printerName in PrinterSettings.InstalledPrinters)
            {
                textBox1.AppendText("Принтер: " + printerName + "\r\n");

                try
                {
                    PrinterSettings printer = new PrinterSettings();
                    printer.PrinterName = printerName;

                    if (printer.IsValid)
                    {
                        textBox1.AppendText("  Поддерживаемые разрешения:\r\n");
                        foreach (PrinterResolution resolution in printer.PrinterResolutions)
                        {
                            textBox1.AppendText("    " + resolution + "\r\n");
                        }

                        textBox1.AppendText("  Поддерживаемые размеры бумаги:\r\n");
                        foreach (PaperSize size in printer.PaperSizes)
                        {
                            if (Enum.IsDefined(size.Kind.GetType(), size.Kind))
                            {
                                textBox1.AppendText("    " + size.PaperName + "\r\n");
                            }
                        }
                    }
                    else
                    {
                        textBox1.AppendText("  Принтер недействителен.\r\n");
                    }
                }
                catch (Exception ex)
                {
                    textBox1.AppendText("  Ошибка при получении информации о принтере: " + ex.Message + "\r\n");
                }
            }
        }

        private void butPrint_Click(object sender, EventArgs e)
        {
            PrintDocument doc = new PrintDocument();
            doc.PrintPage += this.Doc_PrintPage;

            PrintDialog dlgSettings = new PrintDialog();
            dlgSettings.Document = doc;

            if (dlgSettings.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    doc.Print();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при печати: " + ex.Message);
                }
            }
        }

        private void butPreview_Click(object sender, EventArgs e)
        {
            PrintDocument doc = new PrintDocument();
            doc.PrintPage += this.Doc_PrintPage;

            PrintPreviewDialog dlgPreview = new PrintPreviewDialog();
            dlgPreview.Document = doc;

            dlgPreview.ShowDialog();
        }

        private void Doc_PrintPage(object sender, PrintPageEventArgs e)
        {
            using (Font font = new Font("Arial", 30))
            {
                float x = e.MarginBounds.Left;
                float y = e.MarginBounds.Top;
                float lineHeight = font.GetHeight(e.Graphics);

                for (int i = 1; i <= 5; i++)
                {
                    e.Graphics.DrawString("Это строчка номер " + i.ToString(), font, Brushes.Black, x, y);
                    y += lineHeight;
                }

                y += lineHeight;

                try
                {
                    string imagePath = Path.Combine(Application.StartupPath, "test.jpg");
                    if (File.Exists(imagePath))
                    {
                        Image image = Image.FromFile(imagePath);
                        e.Graphics.DrawImage(image, x, y);
                    }
                    else
                    {
                        e.Graphics.DrawString("Изображение test.jpg не найдено", font, Brushes.Red, x, y);
                    }
                }
                catch (Exception ex)
                {
                    e.Graphics.DrawString("Ошибка при загрузке изображения: " + ex.Message, font, Brushes.Red, x, y);
                }
            }
        }
    }
}