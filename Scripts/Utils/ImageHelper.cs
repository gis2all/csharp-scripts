using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using Utils.Core;

namespace Utils
{
    public static class ImageHelper
    {
        // 计算SSIM指数，范围[0,1]，值越大代表两张图片越相似
        public static double CalculateImageSimilarity(string imageFile1, string imageFile2)
        {
            Mat image1 = Cv2.ImRead(imageFile1, ImreadModes.AnyColor);
            Mat image2 = Cv2.ImRead(imageFile2, ImreadModes.AnyColor);
            // 四个通道，前三通道是红绿蓝计算出来的值，范围[0,1],越高越相似，第四个值为0，不做计算
            var ssim = OpenCvUtils.CalculateSSIM(image1, image2);
            var averageValue = (ssim[0] + ssim[1] + ssim[2]) / 3;
            return averageValue;
        }

        // 模板匹配，导出大图中匹配的矩形       
        public static bool IsMatchSubImage(string wholeFile, string subFile, string matchFile = null, string roiFile = null)
        {
            // 第一个参数是大图，第二个参数是小图(原文件),第三个参数是大图加矩形框，第四个参数是大图中匹配带小图的图
            // 参考 https://www.cnblogs.com/skyfsm/p/6884253.html

            bool isMatch;
            Mat img, templ, result = new Mat();
            try
            {
                img = Cv2.ImRead(wholeFile);
                templ = Cv2.ImRead(subFile);

                int result_cols = img.Cols - templ.Cols + 1;
                int result_rows = img.Rows - templ.Rows + 1;
                result.Create(result_cols, result_rows, MatType.CV_32FC1);

                //这里我们使用的匹配算法是标准平方差匹配 method=CV_TM_SQDIFF_NORMED，数值越小匹配度越好
                Cv2.MatchTemplate(img, templ, result, TemplateMatchModes.SqDiffNormed);

                Cv2.Normalize(result, result, 0, 1, NormTypes.MinMax, -1, new Mat());

                double minVal = -1;
                double maxVal;
                OpenCvSharp.Point minLoc;
                OpenCvSharp.Point maxLoc;
                OpenCvSharp.Point matchLoc;

                Cv2.MinMaxLoc(result, out minVal, out maxVal, out minLoc, out maxLoc, new Mat());
                matchLoc = minLoc;

                // 导出匹配到的模板图像
                Rect roi = new Rect(new OpenCvSharp.Point(matchLoc.X, matchLoc.Y), new OpenCvSharp.Size(templ.Cols, templ.Rows));
                Mat roiImage = new Mat(img, roi);
                roiImage.SaveImage(roiFile);

                if (minVal < 0.001)
                {
                    Cv2.Rectangle(img, matchLoc, new OpenCvSharp.Point(matchLoc.X + templ.Cols, matchLoc.Y + templ.Rows), new Scalar(0, 0, 255), 1, LineTypes.Link8, 0);
                }

                //  导出整张匹配图片
                if (!string.IsNullOrEmpty(matchFile))
                {
                    img.SaveImage(matchFile);
                }

                isMatch = true;
            }
            catch (Exception ex)
            {
                isMatch = false;
            }
            return isMatch;
        }

        // 保存桌面目前截图
        public static void SaveLanguageAreaShot(string screenshotFile)
        {
            var screenshot = new Screenshot();
            var screenSize = Screenshot.GetScreenPhysicalSzie();

            // 语言框出现的大概位置，任何屏幕分辨率电脑都适用
            var sourceX = (screenSize.Width / 4) * 3;
            var sourceY = screenSize.Height / 4;
            var destinationX = screenSize.Width;
            var destinationY = screenSize.Height / 4 * 3;
            var imageSize = new System.Drawing.Size(destinationX - sourceX, destinationY - sourceY);

            // 注意，不能同时设置destinationX，destinationY和image size，否则截图会黑屏
            screenshot.CaptureAreaToFile(screenshotFile, sourceX, sourceY, 0, 0, imageSize, ImageFormat.Png);
        }

        public static System.Drawing.Size GetImageSize(string imageFile)
        {
            var image = Image.FromFile(imageFile);
            return image.Size;
        }
    }
}
