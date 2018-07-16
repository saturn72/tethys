using System.IO;

namespace Tethys.Server
{
    public class ConfigManager
    {
        public static string GetConfigFile(string relativeOrAbsolutePath)
        {
            return Path.IsPathRooted(relativeOrAbsolutePath) ? relativeOrAbsolutePath : Path.Combine(Directory.GetCurrentDirectory(), relativeOrAbsolutePath);
        }
    }
}