namespace SimpleBlogDemo.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    using SimpleBlogDemo.Helpers;
    using SimpleBlogDemo.Models;
    using System.IO;
    using System.Text;
    using Dropbox.Api.Files;
    using Dropbox.Api;
    using System;
    using System.Globalization;
    using System.Web;

    [Authorize, RequireHttpsOrXForwarded]
    public class EditController : AsyncController
    {
        private UsersStore store = new UsersStore();
        private UserProfile currentUser;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            this.currentUser = this.store.CurrentUser();

            base.Initialize(requestContext);
        }

        // GET: Edit/{id}
        public async Task<ActionResult> IndexAsync(string id)
        {
            using (var client = this.currentUser.GetAuthenticatedClient())
            {
                if (client == null)
                {
                    return RedirectToAction("Profile", "Home");
                }

                var articles = await client.GetArticleList();
                var filtered = from a in articles
                               where a.DisplayName == id
                               select a;
                var selected = filtered.FirstOrDefault();
                if (selected == null)
                {
                    return RedirectToAction("Display", "Blogs", new { blogname = this.currentUser.BlogName });
                }

                using (var download = await client.Files.DownloadAsync("/" + selected.Filename))
                {
                    var markdown = await download.GetContentAsStringAsync();

                    return View(Tuple.Create(markdown, selected));
                }
            }
        }

        // POST: /Edit
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(enableValidation: false)]
        public async Task<ActionResult> IndexAsync(
            string filename,
            string content,
            string rev)
        {
            using (var client = this.currentUser.GetAuthenticatedClient())
            {
                if (client == null)
                {
                    return RedirectToAction("Profile", "Home");
                }

                using (var mem = new MemoryStream(Encoding.UTF8.GetBytes(content)))
                {
                    try
                    {
                        var updated = await client.Files.UploadAsync(
                            "/" + filename,
                            WriteMode.Overwrite.Instance,
                            body: mem);
                        rev = updated.Rev;
                    }
                    catch (ApiException<UploadError> e)
                    {
                        var uploadError = e.ErrorResponse.AsPath;
                        if (uploadError != null)
                        {
                            var reason = uploadError.Value.Reason;
                            var id = filename.Split('.')[0];

                            var message = string.Format( "Unable to update {0}. Reason: {1}", id, reason);

                            this.Flash(message, FlashLevel.Warning);
                            return RedirectToAction("Index", new { id = id });
                        }
                    }
                }

                var metadata = ArticleMetadata.Parse(filename, rev);

                await client.GetArticle(this.currentUser.BlogName, metadata, bypassCache: true);

                this.Flash(string.Format(
                    "Updated '{0}'.", metadata.Name));
                return Redirect(string.Format(
                    "/Blogs/{0}/{1}",
                    this.currentUser.BlogName,
                    metadata.DisplayName));
            }
        }

        // GET: /Add
        public ActionResult Add()
        {
            return View();
        }

        // POST: /Add
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(enableValidation: false)]
        public async Task<ActionResult> AddAsync(string name, DateTime date, string content)
        {
            var filename = string.Format(
                CultureInfo.InvariantCulture,
                "{0}.{1:yyyyMMdd}.md",
                name,
                date);

            using (var client = this.currentUser.GetAuthenticatedClient())
            {
                if (client == null)
                {
                    return RedirectToAction("Profile", "Home");
                }

                using (var mem = new MemoryStream(Encoding.UTF8.GetBytes(content)))
                {
                    var upload = await client.Files.UploadAsync("/" + filename, body: mem);

                    var metadata = ArticleMetadata.Parse(upload.Name, upload.Rev);

                    return Redirect(string.Format(
                        CultureInfo.InvariantCulture,
                        "/Blogs/{0}/{1}",
                        this.currentUser.BlogName,
                        metadata.DisplayName));
                }
            }
        }

        // POST: /Preview
        // This is called by an ajax request in preview.js
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(enableValidation: false)]
        public ActionResult Preview(string markdown)
        {
            return Content(markdown.ParseMarkdown().ToString());
        }
    }
}
