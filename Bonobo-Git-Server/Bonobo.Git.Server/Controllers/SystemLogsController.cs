using Bonobo.Git.Server.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Bonobo.Git.Server.Controllers
{
    public class SystemLogsController : Controller
    {
        private static readonly Regex LogEntryRegex = new Regex(
            @"^(?<time>\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2}(?:\.\d+)?(?:\s+[+-]\d{2}:\d{2})?)\s+(?:\[(?<level>[^\]]+)\]\s+)?(?<message>.*)$",
            RegexOptions.Compiled);

        [WebAuthorize(Roles = Definitions.Roles.Administrator)]
        public ActionResult Index(string file = null)
        {
            var logDirectory = ResolveLogDirectory();
            var files = GetLogFiles(logDirectory);
            var selected = SelectLogFile(files, file);
            var content = selected == null ? string.Empty : ReadLogFile(selected.FullName);

            var model = new SystemLogsViewModel
            {
                Files = files.Select(x => new SystemLogFileViewModel
                {
                    Name = x.Name,
                    LastWriteTime = x.LastWriteTime,
                    Length = x.Length
                }).ToList(),
                SelectedFile = selected == null ? null : selected.Name,
                LogDirectory = logDirectory,
                XmlLikeContent = ToXmlLikeLog(content)
            };

            return View(model);
        }

        [WebAuthorize(Roles = Definitions.Roles.Administrator)]
        public ActionResult Download(string file)
        {
            var logDirectory = ResolveLogDirectory();
            var selected = SelectLogFile(GetLogFiles(logDirectory), file, allowFallback: false);
            if (selected == null)
            {
                return HttpNotFound();
            }

            var stream = new FileStream(selected.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            return File(stream, "text/plain", selected.Name);
        }

        private static string ResolveLogDirectory()
        {
            var configuredPath = ConfigurationManager.AppSettings["LogDirectory"];
            if (string.IsNullOrWhiteSpace(configuredPath))
            {
                configuredPath = @"~\App_Data\Logs";
            }

            if (Path.IsPathRooted(configuredPath))
            {
                return configuredPath;
            }

            var mappedPath = HostingEnvironment.MapPath(configuredPath);
            if (!string.IsNullOrWhiteSpace(mappedPath))
            {
                return mappedPath;
            }

            return System.Web.HttpContext.Current.Server.MapPath(configuredPath);
        }

        private static List<FileInfo> GetLogFiles(string logDirectory)
        {
            if (string.IsNullOrWhiteSpace(logDirectory) || !Directory.Exists(logDirectory))
            {
                return new List<FileInfo>();
            }

            return new DirectoryInfo(logDirectory)
                .GetFiles("*.txt", SearchOption.TopDirectoryOnly)
                .OrderByDescending(x => x.LastWriteTimeUtc)
                .ToList();
        }

        private static FileInfo SelectLogFile(IEnumerable<FileInfo> files, string file, bool allowFallback = true)
        {
            var list = files.ToList();
            if (!list.Any())
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(file))
            {
                var safeFileName = Path.GetFileName(file);
                var selected = list.FirstOrDefault(x => string.Equals(x.Name, safeFileName, StringComparison.OrdinalIgnoreCase));
                if (selected != null)
                {
                    return selected;
                }
            }

            return allowFallback ? list.First() : null;
        }

        private static string ReadLogFile(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            using (var reader = new StreamReader(stream, Encoding.UTF8, true))
            {
                return reader.ReadToEnd();
            }
        }

        private static string ToXmlLikeLog(string content)
        {
            var builder = new StringBuilder();
            builder.AppendLine("&lt;log&gt;");

            var entryOpen = false;
            using (var reader = new StringReader(content ?? string.Empty))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var match = LogEntryRegex.Match(line);
                    if (match.Success)
                    {
                        if (entryOpen)
                        {
                            builder.AppendLine("  &lt;/entry&gt;");
                        }

                        builder.Append("  &lt;entry time=\"");
                        builder.Append(HttpUtility.HtmlEncode(match.Groups["time"].Value));
                        if (match.Groups["level"].Success)
                        {
                            builder.Append("\" level=\"");
                            builder.Append(HttpUtility.HtmlEncode(match.Groups["level"].Value));
                        }
                        builder.AppendLine("\"&gt;");
                        builder.Append("    &lt;message&gt;");
                        builder.Append(HttpUtility.HtmlEncode(match.Groups["message"].Value));
                        builder.AppendLine("&lt;/message&gt;");
                        entryOpen = true;
                    }
                    else if (entryOpen)
                    {
                        builder.Append("    &lt;trace&gt;");
                        builder.Append(HttpUtility.HtmlEncode(line));
                        builder.AppendLine("&lt;/trace&gt;");
                    }
                }
            }

            if (entryOpen)
            {
                builder.AppendLine("  &lt;/entry&gt;");
            }

            builder.AppendLine("&lt;/log&gt;");
            return builder.ToString();
        }
    }

    public class SystemLogsViewModel
    {
        public List<SystemLogFileViewModel> Files { get; set; }
        public string SelectedFile { get; set; }
        public string LogDirectory { get; set; }
        public string XmlLikeContent { get; set; }
    }

    public class SystemLogFileViewModel
    {
        public string Name { get; set; }
        public DateTime LastWriteTime { get; set; }
        public long Length { get; set; }
    }
}
