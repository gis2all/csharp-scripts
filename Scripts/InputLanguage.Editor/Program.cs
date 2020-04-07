using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using Utils;

namespace InputLanguage.Editor
{
    public class Program
    {
        /// <summary>
        /// 这个脚本用来设置默认输入法，一般为英文状态输入法
        /// 运行期间不要有点击桌面操作
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //try
            //{
            var sourceImage = FileUtils.GetProjectPath() + @"images\0.png";
            // 系统默认已安装输入法的数量
            int languageCount = 4;
            // 获取所有语言的屏幕截图
            var screenshots = GegerateLanguageScreenshots(languageCount);
            // 获取和标准文件匹配的新图像
            var rois = GetLanguageROIs(sourceImage, screenshots);
            // 对比设置默认语言
            SetDaultLanguage(sourceImage, rois, languageCount);
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //finally
            //{
            //    // 为避免出现Win键一直按着，最后总要释放
            //    Win32Utils.OperateKey(KeyBoardConstant.vKeyLeftWin, KeyBoardConstant.KeyStatus.Release);
            //}
        }

        private static List<string> GegerateLanguageScreenshots(int languageCount)
        {
            // 按住Win键
            Win32Utils.OperateKey(KeyBoardConstant.vKeyLeftWin, KeyBoardConstant.KeyStatus.Press);

            // 截图List
            var screenshotList = new List<string>();
            for (int i = 0; i < languageCount; i++)
            {
                // 点击空格键 
                Win32Utils.OperateKey(KeyBoardConstant.vKeySpace, KeyBoardConstant.KeyStatus.Click);
                // 获取屏幕语言区域截图
                var screenshot = FileUtils.GetProjectPath() + @$"output\screenshot_{i + 1}.png";
                ImageHelper.SaveLanguageAreaShot(screenshot);
                // 加入List中用于后续比较
                screenshotList.Add(screenshot);
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
            // 释放Win键
            Win32Utils.OperateKey(KeyBoardConstant.vKeyLeftWin, KeyBoardConstant.KeyStatus.Release);
            return screenshotList;
        }

        private static List<string> GetLanguageROIs(string sourceFile, List<string> screenshotList)
        {
            var count = screenshotList.Count();
            var roiFiles = new List<string>();
            for (int i = 0; i < count; i++)
            {
                string roiFile = FileUtils.GetProjectPath() + @$"output\roi_{i + 1}.png";
                string matchRectFile = FileUtils.GetProjectPath() + @$"output\matchrect_{i + 1}.png";
                bool matched = ImageHelper.IsMatchSubImage(screenshotList[i], sourceFile, matchRectFile, roiFile);
                roiFiles.Add(roiFile);
            }
            return roiFiles;
        }

        public static void SetDaultLanguage(string sourceImage, List<string> roiImages, int languageCount)
        {
            if (roiImages == null || roiImages.Count < 1)
            {
                return;
            }
            Dictionary<int, double> dic = new Dictionary<int, double>();
            // 计算所有对比图像的相似性
            for (int i = 0; i < roiImages.Count; i++)
            {
                var similarity = ImageHelper.CalculateImageSimilarity(sourceImage, roiImages[i]);
                dic.Add(i, similarity);
            }
            // 找出相似度最高的图像的值
            int index = -1;
            foreach (var key in dic.Keys)
            {
                double value = -1;
                if (dic.TryGetValue(key, out value) && value == dic.Values.Max())
                {
                    index = key;
                    break;
                }
            }

            // 这说明在第index次时，所选的语言和预期一样，所以需要按住Win+Space键选择一次
            Win32Utils.OperateKey(KeyBoardConstant.vKeyLeftWin, KeyBoardConstant.KeyStatus.Press);
            for (int i = 0; i < languageCount; i++)
            {
                Win32Utils.OperateKey(KeyBoardConstant.vKeySpace, KeyBoardConstant.KeyStatus.Click);
                Thread.Sleep(TimeSpan.FromSeconds(2));
                if (i == index)
                {
                    break;
                }
            }
            Win32Utils.OperateKey(KeyBoardConstant.vKeyLeftWin, KeyBoardConstant.KeyStatus.Release);
        }
    }
}
