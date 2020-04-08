using System;

namespace Nuget.Last
{
    // 使用DotNet as script插件可以在Jenkins使用C#脚本
    public class Program
    {
        public static void Main(string[] args)
        {
            var file = @"E:\Applications\DotNet\WinDesktop\Apps\arcgis-earth\source\Directory.Build.props";
            var targetNugetDir = @"C:\Users\chao9441\.nuget\packages\esri.arcgisruntime.wpf\";
            if (System.IO.File.Exists(file))
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
                        System.IO.Directory.Delete(targetNugetDir, true);
                    }
                }
            }
            else
            {
                System.IO.Directory.Delete(targetNugetDir, true);
            }
        }
    }
}
