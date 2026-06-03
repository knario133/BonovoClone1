using Bonobo.Git.Server.Configuration;
using Bonobo.Git.Server.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Bonobo.Git.Server.Controllers
{
    public class CustomMenuLinksController : Controller
    {
        private static readonly string[] AllowedRoles = { Definitions.Roles.Member, Definitions.Roles.Administrator };

        [WebAuthorize(Roles = Definitions.Roles.Administrator)]
        public ActionResult Index()
        {
            var model = new CustomMenuLinksPageModel
            {
                Links = UserConfiguration.Current.ParsedCustomMenuLinks
                    .Select(x => new CustomMenuLinkEditModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Url = x.Url,
                        Roles = x.Roles == null ? new string[0] : x.Roles
                    })
                    .ToList(),
                AvailableRoles = AllowedRoles
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [WebAuthorize(Roles = Definitions.Roles.Administrator)]
        public JsonResult Save(CustomMenuLinkEditModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Title) || string.IsNullOrWhiteSpace(model.Url))
            {
                Response.StatusCode = 400;
                return Json(new { success = false, message = "Titulo y link son obligatorios." });
            }

            if (!CustomMenuLinkConfiguration.IsSafeUrl(model.Url))
            {
                Response.StatusCode = 400;
                return Json(new { success = false, message = "Use un link http(s) o una ruta local que comience con /." });
            }

            var roles = (model.Roles ?? new string[0])
                .Where(x => AllowedRoles.Contains(x))
                .Distinct()
                .ToArray();

            if (roles.Length == 0)
            {
                Response.StatusCode = 400;
                return Json(new { success = false, message = "Seleccione al menos un perfil." });
            }

            var links = UserConfiguration.Current.ParsedCustomMenuLinks;
            var id = string.IsNullOrWhiteSpace(model.Id) ? Guid.NewGuid().ToString("N") : model.Id;
            var existing = links.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));

            if (existing == null)
            {
                existing = new CustomMenuLinkConfiguration { Id = id };
                links.Add(existing);
            }

            existing.Title = model.Title.Trim();
            existing.Url = model.Url.Trim();
            existing.Icon = "fa-link";
            existing.Roles = roles;

            UserConfiguration.Current.CustomMenuLinks = UserConfiguration.SerializeCustomMenuLinks(links);
            UserConfiguration.Current.Save();

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [WebAuthorize(Roles = Definitions.Roles.Administrator)]
        public JsonResult Delete(string id)
        {
            var links = UserConfiguration.Current.ParsedCustomMenuLinks;
            var removed = links.RemoveAll(x => string.Equals(x.Id, id, StringComparison.OrdinalIgnoreCase));

            if (removed == 0)
            {
                Response.StatusCode = 404;
                return Json(new { success = false, message = "Link no encontrado." });
            }

            UserConfiguration.Current.CustomMenuLinks = UserConfiguration.SerializeCustomMenuLinks(links);
            UserConfiguration.Current.Save();

            return Json(new { success = true });
        }
    }
}
