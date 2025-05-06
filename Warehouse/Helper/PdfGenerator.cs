using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Warehouse.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Warehouse.Helper
{
    public class PdfGenerator
    {


        public void GenerateAndShowInvoice(Order order)
        {
            string tempPdfPath = null;
            try
            {
                PdfDocument document = CreatePdfDocument(order);
                tempPdfPath = Path.GetTempFileName() + ".pdf"; 
                document.Save(tempPdfPath); 

                Process.Start(new ProcessStartInfo
                {
                    FileName = tempPdfPath,
                    UseShellExecute = true
                });

                StartFileDeletionWatcher(tempPdfPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                if (tempPdfPath != null && File.Exists(tempPdfPath))
                    File.Delete(tempPdfPath);
            }
        }

        // удаление временого файла
        private void StartFileDeletionWatcher(string filePath)
        {
            Task.Run(async () =>
            {
                await Task.Delay(10000); 

                while (true)
                {
                    try
                    {
                        File.Delete(filePath);
                        break; 
                    }
                    catch (IOException)
                    {
                        await Task.Delay(5000); 
                    }
                }
            });
        }

        private PdfDocument CreatePdfDocument(Order order)
        {
            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            var titleFont = new XFont("Arial", 18);
            var headerFont = new XFont("Arial", 12);
            var regularFont = new XFont("Arial", 12);

            double margin = 40;
            double pageWidth = page.Width;
            double y = margin;

            gfx.DrawString("Расходная накладная", titleFont, XBrushes.Black,
                new XRect(0, y, pageWidth, 30), XStringFormats.TopCenter);

            gfx.DrawString($"Дата: {order.OrderDate:dd.MM.yyyy}", regularFont, XBrushes.Black,
                new XRect(0, y+3, pageWidth - margin, 30), XStringFormats.TopRight);

            y += 50;

            double col1 = margin;
            double col2 = col1 + 40;
            double col3 = col2 + 260;
            double col4 = col3 + 80;
            double col5 = col4 + 80;
            double col6 = col5 + 80;
            double rowHeight = 25;

            gfx.DrawRectangle(XPens.Black, col1, y, col6 - col1, rowHeight);
            gfx.DrawLine(XPens.Black, col2, y, col2, y + rowHeight);
            gfx.DrawLine(XPens.Black, col3, y, col3, y + rowHeight);
            gfx.DrawLine(XPens.Black, col4, y, col4, y + rowHeight);
            gfx.DrawLine(XPens.Black, col5, y, col5, y + rowHeight);

            double textOffsetY = 12; 

            gfx.DrawString("№", headerFont, XBrushes.Black, col1 + 5, y + textOffsetY + 5);
            gfx.DrawString("Наименование", headerFont, XBrushes.Black, col2 + 5, y + textOffsetY + 5);
            gfx.DrawString("Кол-во", headerFont, XBrushes.Black, col3 + 5, y + textOffsetY + 5);
            gfx.DrawString("Цена", headerFont, XBrushes.Black, col4 + 5, y + textOffsetY + 5);
            gfx.DrawString("Сумма", headerFont, XBrushes.Black, col5 + 5, y + textOffsetY + 5);

            y += rowHeight;

            int index = 1;
            foreach (var item in order.OrderProducts)
            {
                //прверка на то влазит ли текст
                XSize textSize = gfx.MeasureString(item.Product.Name, regularFont);

                if (textSize.Width > 250)
                {
                    string[] words = item.Product.Name.Split(" ");
                    string line = "";
                    double lineHeight = gfx.MeasureString("A", regularFont).Height;
                    double currentY = y + textOffsetY + 5;
                    int lineCount = 0;

                    foreach (var word in words)
                    {
                        string temp = string.IsNullOrEmpty(line) ? word : line + " " + word;
                        if (gfx.MeasureString(temp, regularFont).Width < 250)
                        {
                            line = temp;
                        }
                        else
                        {
                            gfx.DrawString(line, regularFont, XBrushes.Black, col2 + 5, currentY);
                            currentY += lineHeight;
                            line = word;
                            lineCount++;
                        }
                    }

                    if (!string.IsNullOrEmpty(line))
                    {
                        gfx.DrawString(line, regularFont, XBrushes.Black, col2 + 5, currentY);
                        currentY += lineHeight;
                        lineCount++;
                    }

                    rowHeight = (rowHeight*lineCount)-10; 
                }
                else
                {
                    gfx.DrawString(item.Product.Name, regularFont, XBrushes.Black, col2 + 5, y + textOffsetY + 5);
                }
                gfx.DrawRectangle(XPens.Black, col1, y, col6 - col1, rowHeight);
                gfx.DrawLine(XPens.Black, col2, y, col2, y + rowHeight);
                gfx.DrawLine(XPens.Black, col3, y, col3, y + rowHeight);
                gfx.DrawLine(XPens.Black, col4, y, col4, y + rowHeight);
                gfx.DrawLine(XPens.Black, col5, y, col5, y + rowHeight);

                gfx.DrawString(index.ToString(), regularFont, XBrushes.Black, col1 + 5, y + textOffsetY + 5);
                //
                gfx.DrawString(item.Quantity.ToString(), regularFont, XBrushes.Black, col3 + 5, y + textOffsetY + 5);
                gfx.DrawString(item.Product.Price.ToString("F2"), regularFont, XBrushes.Black, col4 + 5, y + textOffsetY + 5);
                gfx.DrawString(item.PriceAtOrder.ToString("F2"), regularFont, XBrushes.Black, col5 + 5, y + textOffsetY + 5);

                y += rowHeight;
                index++;
                rowHeight = 25;
            }

            y += 5;
            gfx.DrawString($"Итого:", headerFont, XBrushes.Black, col4 + 5, y + 10);
            gfx.DrawString($"{order.TotalAmount:F2}", headerFont, XBrushes.Black, col5 + 5, y + 10);

            return document;
        }


    }
}

