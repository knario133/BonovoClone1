using Bonobo.Git.Server.App_GlobalResources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Bonobo.Git.Server.Configuration
{

    [JsonObject("Configuration")]
    public class UserConfiguration : ConfigurationEntry<UserConfiguration>
    {
        [JsonProperty("AllowAnonymousPush")]
        public bool AllowAnonymousPush { get; set; }

        [JsonProperty("Repositories")]
        public string RepositoryPath { get; set; }

        [JsonProperty("AllowUserRepositoryCreation")]
        public bool AllowUserRepositoryCreation { get; set; }

        [JsonProperty("AllowPushToCreate")]
        public bool AllowPushToCreate { get; set; }

        [JsonProperty("AllowAnonymousRegistration")]
        public bool AllowAnonymousRegistration { get; set; }

        [JsonProperty("DefaultLanguage")]
        public string DefaultLanguage { get; set; }

        [JsonProperty("SiteTitle")]
        public string SiteTitle { get; set; }

        [JsonProperty("SiteLogoUrl")]
        public string SiteLogoUrl { get; set; }

        [JsonProperty("SiteFooterMessage")]
        public string SiteFooterMessage { get; set; }

        [JsonProperty("SiteCssUrl")]
        public string SiteCssUrl { get; set; }

        [JsonProperty("IsCommitAuthorAvatarVisible")]
        public bool IsCommitAuthorAvatarVisible { get; set; }

        [JsonProperty("LinksRegex")]
        public string LinksRegex { get; set; }

        [JsonProperty("LinksUrl")]
        public string LinksUrl { get; set; }

        [JsonProperty("CustomMenuLinks")]
        public string CustomMenuLinks { get; set; }

        [JsonIgnore]
        public List<CustomMenuLinkConfiguration> ParsedCustomMenuLinks => ParseCustomMenuLinks(CustomMenuLinks);

        [JsonIgnore]
        public string Repositories => PathResolver.Resolve(RepositoryPath);

        [JsonIgnore]
        public bool HasSiteFooterMessage => !string.IsNullOrWhiteSpace(this.SiteFooterMessage);

        [JsonIgnore]
        public bool HasCustomSiteLogo => !string.IsNullOrWhiteSpace(this.SiteLogoUrl);

        [JsonIgnore]
        public bool HasCustomSiteCss => !string.IsNullOrWhiteSpace(SiteCssUrl);

        [JsonIgnore]
        public bool HasLinks => !string.IsNullOrWhiteSpace(this.LinksRegex);

        public string GetSiteTitle() => !string.IsNullOrWhiteSpace(this.SiteTitle) ? this.SiteTitle : Resources.Layout_Title;

        public static void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }

            Current.RepositoryPath = ConfigurationManager.AppSettings["DefaultRepositoriesDirectory"];
            Current.Save();
        }

        private static bool IsInitialized => !String.IsNullOrEmpty(Current.RepositoryPath);

        public static List<CustomMenuLinkConfiguration> ParseCustomMenuLinks(string rawLinks)
        {
            if (string.IsNullOrWhiteSpace(rawLinks))
            {
                return new List<CustomMenuLinkConfiguration>();
            }

            try
            {
                var links = JsonConvert.DeserializeObject<List<CustomMenuLinkConfiguration>>(rawLinks);
                if (links != null)
                {
                    return links.Where(x => x != null && x.IsValid).ToList();
                }
            }
            catch
            {
            }

            return rawLinks.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(ParseLegacyCustomMenuLink)
                .Where(x => x != null && x.IsValid)
                .ToList();
        }

        public static string SerializeCustomMenuLinks(IEnumerable<CustomMenuLinkConfiguration> links)
        {
            return JsonConvert.SerializeObject(
                (links ?? Enumerable.Empty<CustomMenuLinkConfiguration>()).Where(x => x != null && x.IsValid).ToList(),
                Formatting.Indented);
        }

        private static CustomMenuLinkConfiguration ParseLegacyCustomMenuLink(string line)
        {
            var parts = (line ?? string.Empty).Split('|');
            var title = parts.Length > 0 ? parts[0].Trim() : string.Empty;
            var url = parts.Length > 1 ? parts[1].Trim() : string.Empty;
            var icon = parts.Length > 2 ? parts[2].Trim() : "fa-link";

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            return new CustomMenuLinkConfiguration
            {
                Id = StableId(title + "|" + url),
                Title = title,
                Url = url,
                Icon = icon,
                Roles = new[] { Definitions.Roles.Member, Definitions.Roles.Administrator }
            };
        }

        private static string StableId(string value)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(value ?? string.Empty));
                return new Guid(hash).ToString("N");
            }
        }
    }

    public class CustomMenuLinkConfiguration
    {
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Url")]
        public string Url { get; set; }

        [JsonProperty("Icon")]
        public string Icon { get; set; }

        [JsonProperty("Roles")]
        public string[] Roles { get; set; }

        [JsonIgnore]
        public bool IsValid => !string.IsNullOrWhiteSpace(Title) && IsSafeUrl(Url);

        public bool IsVisibleTo(System.Security.Principal.IPrincipal user)
        {
            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
            {
                return false;
            }

            if (Roles == null || Roles.Length == 0)
            {
                return true;
            }

            return Roles.Any(user.IsInRole);
        }

        public string GetIconClass()
        {
            var icon = string.IsNullOrWhiteSpace(Icon) ? "fa-link" : Icon.Trim();
            if (icon.StartsWith("fa ", StringComparison.OrdinalIgnoreCase) ||
                icon.StartsWith("fa-solid ", StringComparison.OrdinalIgnoreCase) ||
                icon.StartsWith("fa-brands ", StringComparison.OrdinalIgnoreCase))
            {
                return icon;
            }

            if (icon.StartsWith("fa-", StringComparison.OrdinalIgnoreCase))
            {
                return "fa-solid " + icon;
            }

            return "fa-solid fa-link";
        }

        public static bool IsSafeUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return false;
            }

            if (url.StartsWith("/", StringComparison.Ordinal))
            {
                return true;
            }

            Uri absoluteUri;
            return Uri.TryCreate(url, UriKind.Absolute, out absoluteUri) &&
                (absoluteUri.Scheme == Uri.UriSchemeHttp || absoluteUri.Scheme == Uri.UriSchemeHttps);
        }
    }
}
