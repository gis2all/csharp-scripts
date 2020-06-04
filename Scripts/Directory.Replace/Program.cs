using System;
using System.IO;
using System.Linq;
using Utils;

namespace Directory.Replace
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ReplaceStandardWorkspaceDir();
            ReplaceJenkinsAntProperties();
            ReplaceTestScriptHelper();
        }

        private static void ReplaceStandardWorkspaceDir()
        {
            var sourceDir = @"\\\\vm-3d-data\\storage\\ArcGISEarth\\Data";
            var targetDir = @"\\\\DESKTOP-QO5V6UK\\ArcGISEarth\\data";
            var workspaceRootDir = @"\\DESKTOP-QO5V6UK\ArcGISEarth\data\WinAppDriverTest\Standard\Worksapce";

            var rootDirInfo = System.IO.Directory.GetDirectories(workspaceRootDir);

            foreach (var issueWorkspace in rootDirInfo)
            {
                var bookmarkFile = issueWorkspace + @"\workspace\bookmarks.json";
                var toclayerFile = issueWorkspace + @"\workspace\operational_layers.json";
                FileUtils.ReplaceFileContent(bookmarkFile, sourceDir, targetDir);
                FileUtils.ReplaceFileContent(toclayerFile, sourceDir, targetDir);
            }
        }

        private static void ReplaceJenkinsAntProperties()
        {
            // Automated Testing Dir
            var sourceUserConfigContent = @"//dory/users/chaowang/Files/ArcGIS_Earth/user_config";
            var tartgetUserConfigContent = @"//DESKTOP-QO5V6UK/ArcGISEarth/data/WinAppDriverTest/Files/ArcGIS_Earth/user_config";
            var sourceWorkspaceRootContent = @"//dory/users/chaowang/ArcGISEarth/WinAppDriverTest/Standard/Worksapce";
            var targetWorkspaceRootContent = @"//DESKTOP-QO5V6UK/ArcGISEarth/data/WinAppDriverTest/Standard/Worksapce";

            // Build Dir
            var sourceLcoalBuildContent = @"//dory/users/chaowang/ArcGISEarth/Builds";
            var tartgetLcoalBuildContent = @"//DESKTOP-QO5V6UK/ArcGISEarth/Builds";

            var jenkinsRootDir = @"D:\AutoTest\jenkins-scripts\Jenkins";
            var rootDirInfo = System.IO.Directory.GetDirectories(jenkinsRootDir);
            foreach (var jenkinsJobDir in rootDirInfo)
            {
                var propertiesFile = System.IO.Directory.GetFiles(jenkinsJobDir, "*.properties").ToList().FirstOrDefault();
                FileUtils.ReplaceFileContent(propertiesFile, sourceUserConfigContent, tartgetUserConfigContent);
                FileUtils.ReplaceFileContent(propertiesFile, sourceWorkspaceRootContent, targetWorkspaceRootContent);
                FileUtils.ReplaceFileContent(propertiesFile, sourceLcoalBuildContent, tartgetLcoalBuildContent);
            }
        }

        private static void ReplaceTestScriptHelper()
        {
            var sourceTestDataContent = @"\\dory\users\chaowang\ArcGISEarth\WinAppDriverTest\";
            var targetTestDataContent = @"\\DESKTOP-QO5V6UK\ArcGISEarth\data\WinAppDriverTest\";
            var testHelperFile = @"D:\arcgis-earth-test\Automated.Testing\Utils\TestHelper.cs";
            FileUtils.ReplaceFileContent(testHelperFile, sourceTestDataContent, targetTestDataContent);
        }
    }
}
