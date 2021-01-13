// <copyright file="EditController.cs" company="Dropbox Inc">
// Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>

namespace SimpleBlogDemo.Controllers
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Dropbox.Api;
    using Dropbox.Api.Files;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using SimpleBlogDemo.Helpers;
    using SimpleBlogDemo.Models;

    [Authorize]
    [RequireHttpsOrXForwarded]
    public class EditController : Controller
    {
        private readonly UserManager<UserProfile> userManager;

        public EditController(UserManager<UserProfile> userManager)
        {
            this.userManager = userManager;
        }

        // GET: Edit/{id}
        public async Task<ActionResult> IndexAsync(string id)
        {
            using (var client = await this.GetAuthenticatedClient(this.HttpContext))
            {
                if (client == null)
                {
                    return this.RedirectToAction("Profile", "Home");
                }

                var articles = await client.GetArticleList();
                var filtered = from a in articles
                               where a.DisplayName == id
                               select a;
                var selected = filtered.FirstOrDefault();
                if (selected == null)
                {
                    return this.RedirectToAction("Display", "Blogs", new { blogname = (await this.GetCurrentUser(this.HttpContext)).BlogName });
                }

                using (var download = await client.Files.DownloadAsync("/" + selected.Filename))
                {
                    var markdown = await download.GetContentAsStringAsync();

                    return this.View(Tuple.Create(markdown, selected));
                }
            }
        }

        // POST: /Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> IndexAsync(
            string filename,
            string content,
            string rev)
        {
            using (var client = await this.GetAuthenticatedClient(this.HttpContext))
            {
                if (client == null)
                {
                    return this.RedirectToAction("Profile", "Home");
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

                            var message = string.Format("Unable to update {0}. Reason: {1}", id, reason);

                            this.Flash(message, FlashLevel.Warning);
                            return this.RedirectToAction("Index", new { id = id });
                        }
                    }
                }

                var metadata = ArticleMetadata.Parse(filename, rev);

                await client.GetArticle((await this.GetCurrentUser(this.HttpContext)).BlogName, metadata, bypassCache: true);

                this.Flash(string.Format(
                    "Updated '{0}'.", metadata.Name));
                return this.Redirect(string.Format(
                    "/Blogs/{0}/{1}",
                    (await this.GetCurrentUser(this.HttpContext)).BlogName,
                    metadata.DisplayName));
            }
        }

        // GET: /Add
        public ActionResult Add()
        {
            return this.View();
        }

        // POST: /Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAsync(string name, DateTime date, string content)
        {
            var filename = string.Format(
                CultureInfo.InvariantCulture,
                "{0}.{1:yyyyMMdd}.md",
                name,
                date);

            using (var client = await this.GetAuthenticatedClient(this.HttpContext))
            {
                if (client == null)
                {
                    return this.RedirectToAction("Profile", "Home");
                }

                using (var mem = new MemoryStream(Encoding.UTF8.GetBytes(content)))
                {
                    var upload = await client.Files.UploadAsync("/" + filename, body: mem);

                    var metadata = ArticleMetadata.Parse(upload.Name, upload.Rev);

                    return this.Redirect(string.Format(
                        CultureInfo.InvariantCulture,
                        "/Blogs/{0}/{1}",
                        (await this.GetCurrentUser(this.HttpContext)).BlogName,
                        metadata.DisplayName));
                }
            }
        }

        // POST: /Preview
        // This is called by an ajax request in preview.js
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Preview(string markdown)
        {
            return this.Content(markdown.ParseMarkdown().ToString());
        }

        private async Task<UserProfile> GetCurrentUser(HttpContext context)
        {
            return await this.userManager.GetUserAsync(context.User);
        }

        private async Task<DropboxClient> GetAuthenticatedClient(HttpContext context)
        {
            var user = await this.GetCurrentUser(context);
            return new DropboxClient(user.DropboxAccessToken, new DropboxClientConfig("SimpleBlogDemo"));
        }
    }
}
