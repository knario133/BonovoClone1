using Bonobo.Git.Server.App_GlobalResources;
using Newtonsoft.Json;
using System;
using System.Configuration;

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
    }
}
