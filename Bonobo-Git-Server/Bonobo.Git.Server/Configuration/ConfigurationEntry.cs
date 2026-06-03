using System.IO;
using Newtonsoft.Json;

namespace Bonobo.Git.Server.Configuration
{
    public abstract class ConfigurationEntry<Entry> where Entry : ConfigurationEntry<Entry>, new()
    {
        private static Entry _current = null;
        private static IPathResolver pathResolver = new HostingEnvironmentPathResolver();
        private static readonly object _sync = new object();
        public static IPathResolver PathResolver { get => pathResolver; set => pathResolver = value; }
        private static string ConfigPath { get => PathResolver.ResolveWithConfiguration("UserConfiguration"); }


        public static Entry Current { get { return _current ?? Load(); } }


        private static Entry Load()
        {
            lock (_sync)
            {
                if (_current == null)
                {
                    try
                    {
                        var json = File.ReadAllText(ConfigPath);
                        _current = JsonConvert.DeserializeObject<Entry>(json) ?? new Entry();
                    }
                    catch (FileNotFoundException)
                    {
                        _current = new Entry();
                    }
                }
            }

            return _current;
        }

        public void Save()
        {
            lock (_sync)
            {
                if (_current != null)
                {
                    var configDirectory = Path.GetDirectoryName(ConfigPath);
                    if (!string.IsNullOrEmpty(configDirectory) && !Directory.Exists(configDirectory))
                    {
                        Directory.CreateDirectory(configDirectory);
                    }

                    var json = JsonConvert.SerializeObject(_current, Formatting.Indented);
                    File.WriteAllText(ConfigPath, json);
                }
            }
        }

        public static void InitialiseForTest()
        {
            _current = new Entry();
        }
    }
}
