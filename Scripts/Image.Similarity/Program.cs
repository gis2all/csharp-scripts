
using PInvoke;
using System;
using System.Linq;
using Utils;

namespace Image.Similarity
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Mouuse.Move项目设置输出框隐藏，这里需要设置回来
            var cmdWindow = Kernel32.GetConsoleWindow();
            User32.ShowWindow(cmdWindow, User32.WindowShowStyle.SW_SHOW);

            // 计算SSIM指数
            var similarity_1 = ImageHelper.CalculateImageSimilarity(FileUtils.GetProjectPath() + @"images\0.png", FileUtils.GetProjectPath() + @"images\1.png");
            var similarity_2 = ImageHelper.CalculateImageSimilarity(FileUtils.GetProjectPath() + @"images\0.png", FileUtils.GetProjectPath() + @"images\2.png");
            var similarity_3 = ImageHelper.CalculateImageSimilarity(FileUtils.GetProjectPath() + @"images\0.png", FileUtils.GetProjectPath() + @"images\3.png");
            var similarity_4 = ImageHelper.CalculateImageSimilarity(FileUtils.GetProjectPath() + @"images\0.png", FileUtils.GetProjectPath() + @"images\4.png");
            var similarity_5 = ImageHelper.CalculateImageSimilarity(FileUtils.GetProjectPath() + @"images\0.png", FileUtils.GetProjectPath() + @"images\5.png");

            // 打印结果
            Console.WriteLine("完全相同时SSIM的值：        " + similarity_1 + "\r\n");
            Console.WriteLine("内容相同质量下降的SSIM值：  " + similarity_2 + "\r\n");
            Console.WriteLine("很小修改时SSIM的值：        " + similarity_3 + "\r\n");
            Console.WriteLine("很大修改时SSIM的值：        " + similarity_4 + "\r\n");
            Console.WriteLine("不同图片的SSIM的值：        " + similarity_5 + "\r\n");
            Console.ReadKey();
        }
    }
}
