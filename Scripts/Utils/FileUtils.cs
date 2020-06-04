using CsvHelper;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Core;

namespace Utils
{
    public class FileUtils
    {
        public static string GetProjectPath()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            return baseDir.Split("bin").FirstOrDefault();
        }

        public static void CreateDir(string filePath)
        {
            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public static List<CsvCloumn> ReadCsvFile(string csvPath)
        {
            List<CsvCloumn> csvCloumns = new List<CsvCloumn>();
            using (var reader = new StreamReader(csvPath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                // 标题大写
                csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.ToUpper();
                csvCloumns = csv.GetRecords<CsvCloumn>().ToList();
            }
            return csvCloumns;
        }

        public static void WriteCsvFile(List<CsvRow> csvRowList, string csvPath, List<string> dateList)
        {
            if (File.Exists(csvPath))
            {
                File.Delete(csvPath);
            }
            if (!Directory.Exists(GetProjectPath() + "output"))
            {
                Directory.CreateDirectory(GetProjectPath() + "output");
            }
            // Foreach在list中是按顺序的
            foreach (var csvRow in csvRowList)
            {
                string valueText = null;
                string dateText = null;
                foreach (var date in dateList)
                {

                    if (date == dateList.LastOrDefault())
                    {
                        valueText += csvRow[date];
                        dateText += date;
                    }
                    else
                    {
                        valueText += csvRow[date] + ",";
                        dateText += date + ",";
                    }
                }
                // 写入标题
                if (csvRow == csvRowList.FirstOrDefault())
                {
                    var text = nameof(csvRow.Label) + "," + nameof(csvRow.Category) + "," + nameof(csvRow.Image) + "," + dateText + "\r\n";
                    File.AppendAllText(csvPath, text);
                    Console.WriteLine(text);

                    text = csvRow.Label + "," + csvRow.Category + "," + csvRow.Image + "," + valueText + "\r\n";
                    File.AppendAllText(csvPath, text);
                    Console.WriteLine(text);
                }
                // 写入内容
                else
                {
                    var text = csvRow.Label + "," + csvRow.Category + "," + csvRow.Image + "," + valueText + "\r\n";
                    File.AppendAllText(csvPath, text);
                    Console.WriteLine(text);
                }
            }
        }

        public static List<string> GetUniqueDateList(List<CsvCloumn> csvCloumnList)
        {
            var dateList = new List<string>();
            csvCloumnList.ForEach(p => dateList.Add(p.Date));
            var uniqeDateList = dateList.Distinct().ToList();
            uniqeDateList.Sort();
            return uniqeDateList;
        }

        public static List<string> GetImageFilesPath(string folderName)
        {
            if (!Directory.Exists(folderName))
            {
                throw new Exception($"{folderName} doesn't exist!");
            }
            var ImageFiles = Directory.EnumerateFiles(folderName).Where(f => f.EndsWith(".jpg") ||
                  f.EndsWith(".jpeg") || f.EndsWith(".png") || f.EndsWith(".bmp") || f.EndsWith(".tif") || f.EndsWith(".tiff"));
            return ImageFiles.ToList();
        }

        public static System.Drawing.Size GetScaleSize(Size imageSize, Size slideSize)
        {
            var scaleWidth = 0.0f;
            var scaleHeight = 0.0f;
            var scale = (double)imageSize.Height / (double)imageSize.Width;
            if (imageSize.Width > imageSize.Height)
            {
                if (imageSize.Width > slideSize.Width)
                {
                    scaleWidth = slideSize.Width;
                    scaleHeight = (float)(scaleWidth * scale);
                    if (scaleHeight > slideSize.Height)
                    {
                        scaleHeight = slideSize.Height;
                        scaleWidth = (float)(scaleHeight / scale);
                    }
                }
                else
                {
                    scaleWidth = imageSize.Width;
                    scaleHeight = imageSize.Height;
                }
            }
            else
            {
                if (imageSize.Height > slideSize.Height)
                {
                    scaleHeight = slideSize.Height;
                    scaleWidth = (float)(scaleHeight / scale);
                    if (scaleWidth > slideSize.Width)
                    {
                        scaleWidth = slideSize.Width;
                        scaleHeight = (float)(scaleWidth * scale);
                    }
                }
                else
                {
                    scaleWidth = imageSize.Width;
                    scaleHeight = imageSize.Height;
                }
            }
            return new Size((int)scaleWidth, (int)scaleHeight);
        }

        public static void SaveAsPPTFromImages(string imageFolder, string pptName)
        {
            var task = Task.Factory.StartNew(() =>
            {
                Application pptApp = new Application();
                Presentations pptPres = pptApp.Presentations;
                Presentation pptPre = pptPres.Add(MsoTriState.msoFalse);

                var imagesName = GetImageFilesPath(imageFolder);
                for (int i = 0; i < imagesName.Count; i++)
                {
                    // Slide start from 1
                    var slide = pptPre.Slides.Add(i + 1, PpSlideLayout.ppLayoutObject);
                    var imageSize = ImageHelper.GetImageSize(imagesName[i]);
                    var slideSize = new Size((int)slide.Master.Width, (int)slide.Master.Height);
                    var scaleSize = FileUtils.GetScaleSize(imageSize, slideSize);
                    AddPictureShape(slide, imagesName[i], scaleSize);
                }

                // Save it as a ppt file
                pptPre.SaveAs(pptName, PpSaveAsFileType.ppSaveAsDefault, MsoTriState.msoCTrue);
            });
            task.Wait();
        }

        public static void AddPictureShape(Slide slide, string imageName, Size scaleSize)
        {
            // Set picture shape
            var shape = slide.Shapes.AddPicture(imageName, MsoTriState.msoTrue, MsoTriState.msoTrue, 0, 0, scaleSize.Width, scaleSize.Height);
            shape.Width = scaleSize.Width;
            shape.Height = scaleSize.Height;
            shape.Left = 0;
            shape.Top = 0;
        }

        public static void SaveAsPDFFromImages(string imageFolder, string pdfName, double width = 600)
        {
            var task = Task.Factory.StartNew(() =>
            {
                var imagesName = GetImageFilesPath(imageFolder);
                var document = new PdfDocument(pdfName);
                for (int i = 0; i < imagesName.Count; i++)
                {
                    // PdfPage page = document.AddPage();                   
                    PdfPage page = document.AddPage();
                    var image = Image.FromFile(imagesName[i]);
                    using (XImage img = XImage.FromGdiPlusImage(image))
                    {
                        // Calculate new height to keep image ratio
                        var height = (int)(((double)width / (double)img.PixelWidth) * img.PixelHeight);

                        // Change PDF Page size to match image
                        page.Width = width;
                        page.Height = height;

                        XGraphics gfx = XGraphics.FromPdfPage(page);
                        gfx.DrawImage(img, 0, 0, width, height);
                    }
                }
                document.Save(pdfName);
            });
            task.Wait();
        }

        public static void ReplaceFileContent(string filePath, string sourceContent, string targetContent)
        {
            if (File.Exists(filePath))
            {
                var content = File.ReadAllText(filePath);
                if (content.Contains(sourceContent))
                {
                    var finalContent = content.Replace(sourceContent, targetContent);
                    File.WriteAllText(filePath, finalContent);
                }
            }
        }
    }
}
