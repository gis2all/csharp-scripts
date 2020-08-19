using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading;
using Utils;

namespace Highight.Element
{
    public class Program
    {
        static void Main(string[] args)
        {
            // chromewebdriver.exe的存放文件夹, 以及最大化设置
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("start-maximized");
            var webDriver = new ChromeDriver(@"C:\Program Files (x86)\Google\Chrome\Application", options);

            // 跳转到百度页面
            webDriver.Navigate().GoToUrl("http://www.baidu.com/");
            Thread.Sleep(1000);

            // 找到搜索框，输入文字
            var searchTextBox = webDriver.FindElementById("kw");
            searchTextBox.SendKeys("Selenium百度百科");
            Thread.Sleep(2000);
            HighlightElement(searchTextBox, webDriver, FileUtils.GetProjectPath() + @"images\1.png", "Step 1 - 寻找搜索框并输入文字");
            searchTextBox.SendKeys(Keys.Enter);

            // 点击第一条搜索结果
            Thread.Sleep(10000);
            var resultItem = webDriver.FindElementById("1");
            var firstSelenium = resultItem.FindElement(By.TagName("em"));
            HighlightElement(firstSelenium, webDriver, FileUtils.GetProjectPath() + @"images\2.png", "Step 2 - 点击搜索结果中的第一项");
            firstSelenium.Click();
        }

        private static void HighlightElement(IWebElement element, ChromeDriver webDriver, string imagePath, string testInfo = null)
        {
            if (element == null || webDriver == null)
            {
                return;
            }
            Screenshot screenshot = webDriver.GetScreenshot();
            Byte[] byteArr = screenshot.AsByteArray;

            using (var stream = new MemoryStream(byteArr))
            {
                Image image = Bitmap.FromStream(stream);
                var bitmap = new Bitmap(image);

                // 绘制控件矩形框
                var pen = new Pen(Color.Red, 2);
                Graphics graphics = Graphics.FromImage(bitmap);
                var rectangle = new Rectangle(element.Location.X - 4, element.Location.Y - 4, element.Size.Width + 8, element.Size.Height + 8);
                graphics.DrawRectangle(pen, rectangle);

                // 绘制说明信息
                RectangleF textRec = new RectangleF(40, 40, 500, 500);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.DrawString(testInfo, new Font("Tahoma", 12, FontStyle.Bold), Brushes.OrangeRed, textRec);
                graphics.Flush();

                // 保存修改后的图片
                bitmap.Save(imagePath);
            }
        }
    }
}
