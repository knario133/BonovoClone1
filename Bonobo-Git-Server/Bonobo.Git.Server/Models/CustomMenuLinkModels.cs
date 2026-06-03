using System.Collections.Generic;

namespace Bonobo.Git.Server.Models
{
    public class CustomMenuLinksPageModel
    {
        public List<CustomMenuLinkEditModel> Links { get; set; }
        public string[] AvailableRoles { get; set; }
    }

    public class CustomMenuLinkEditModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string[] Roles { get; set; }
    }
}
