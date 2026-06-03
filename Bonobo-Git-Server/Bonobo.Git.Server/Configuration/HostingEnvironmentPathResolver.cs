using System.Configuration;
using System.IO;
using System.Web.Hosting;

namespace Bonobo.Git.Server.Configuration
{
    internal class HostingEnvironmentPathResolver : IPathResolver
    {
        public string Resolve(string path) => Path.IsPathRooted(path) ? path : HostingEnvironment.MapPath(path);

        public string ResolveWithConfiguration(string configKey)
        {
            var configuredPath = ConfigurationManager.AppSettings[configKey];

            if (configKey == "UserConfiguration" && Path.GetExtension(configuredPath) == ".xml")
            {
                configuredPath = Path.ChangeExtension(configuredPath, ".json");
            }

            return Resolve(configuredPath);
        }
    }
}
