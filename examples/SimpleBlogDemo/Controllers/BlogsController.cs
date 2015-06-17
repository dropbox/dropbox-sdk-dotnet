using Dropbox.Api;
using SimpleBlogDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using SimpleBlogDemo.Helpers;
using System.Threading.Tasks;
using WebMatrix.WebData;

namespace SimpleBlogDemo.Controllers
{
    [RequireHttpsOrXForwarded]
    public partial class BlogsController : AsyncController
    {
        // GET: Blogs
        public async Task<ActionResult> DisplayAsync(string blogname, string id = null)
        {
            var user = this.GetBlogUser(blogname);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            using (var client = this.GetClient(user))
            {
                if (client == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                var articles = new List<ArticleMetadata>(await client.GetArticleList());
                bool isEditable = WebSecurity.IsAuthenticated && WebSecurity.CurrentUserId == user.ID;

                Article article = null;

                if (!string.IsNullOrWhiteSpace(id))
                {
                    var filtered = from a in articles
                                   where a.DisplayName == id
                                   select a;
                    var selected = filtered.FirstOrDefault();
                    if (selected != null)
                    {
                        article = await client.GetArticle(blogname, selected);
                    }
                }

                if (article == null)
                {
                    return View("Index", Tuple.Create(articles, blogname, isEditable));
                }
                else
                {
                    return View("Display", Tuple.Create(article, articles, blogname, isEditable));
                }
           }
        }
    }

    public partial class BlogsController
    {
        private UsersStore store = new UsersStore();

        private UserProfile GetBlogUser(string blogname)
        {
            if (string.IsNullOrWhiteSpace(blogname))
            {
                return null;
            }

            var users = from u in store.Users
                        where u.BlogName == blogname
                        select u;

            return users.FirstOrDefault();
        }

        private DropboxClient GetClient(UserProfile user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.DropboxAccessToken))
            {
                return null;
            }

            return new DropboxClient(user.DropboxAccessToken, userAgent: "SimpleBlogDemo");
        }
    }
}
