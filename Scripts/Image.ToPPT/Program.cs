using Utils;

namespace Image.ToPPT
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var imageFolder = FileUtils.GetProjectPath() + @"images";
            var outPPT = FileUtils.GetProjectPath() + @"output\ArcGIS Earth.pptx";
            FileUtils.SaveAsPPTFromImages(imageFolder, outPPT);
        }
    }
}
