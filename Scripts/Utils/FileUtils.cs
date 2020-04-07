using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
            if (Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
    }
}
