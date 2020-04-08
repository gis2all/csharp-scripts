using System;
using System.IO;

namespace Nuget.Last
{
    // 获取最新的Nuget
    public class Program
    {
        public static void Main(string[] args)
        {
            var file = @"E:\Applications\DotNet\WinDesktop\Apps\arcgis-earth\source\Directory.Build.props";
            var targetNugetDir = @"C:\Users\chao9441\.nuget\packages\esri.arcgisruntime.wpf\";
            if (File.Exists(file))
            {
                var text = System.IO.File.ReadAllText(file);
                var splitStr = text.Split("Version=");
                if (splitStr.Length == 2)
                {
                    // 截取DailyBuid version
                    var dailyBuild = splitStr[1].Substring(1, 17);
                    var currentDailyBuild = targetNugetDir + dailyBuild;
                    // 如果不存在则删除整个文件夹，这样可以获取最新的Nuget包
                    if (!System.IO.Directory.Exists(currentDailyBuild))
                    {
                        Directory.Delete(targetNugetDir, true);
                        Console.WriteLine($"Daily Build changes!");
                        Console.WriteLine($"Delete nuget dir to get the last nuget package: {targetNugetDir}");
                    }
                    else
                    {
                        Console.WriteLine($"Daily Build dosen't change!");
                    }
                }
            }
            else
            {
                Directory.Delete(targetNugetDir, true);
                Console.WriteLine($"Daily Build changes!");
                Console.WriteLine($"Delete nuget dir to get the last nuget package: {targetNugetDir}");
            }
        }
    }
}
