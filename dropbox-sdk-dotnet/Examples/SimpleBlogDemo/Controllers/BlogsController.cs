// <copyright file="BlogsController.cs" company="Dropbox Inc">
// Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>

namespace SimpleBlogDemo.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Dropbox.Api;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using SimpleBlogDemo.Helpers;
    using SimpleBlogDemo.Models;

    [RequireHttpsOrXForwarded]
    public partial class BlogsController : Controller
    {
        private readonly UserManager<UserProfile> userManager;

        public BlogsController(UserManager<UserProfile> userManager)
        {
            this.userManager = userManager;
        }

        // GET: Blogs
        public async Task<ActionResult> DisplayAsync(string blogname, string id = null)
        {
            var user = this.GetBlogUser(blogname);
            if (user == null)
            {
                return this.RedirectToAction("Index", "Home");
            }

            using (var client = this.GetClient(user))
            {
                if (client == null)
                {
                    return this.RedirectToAction("Index", "Home");
                }

                var articles = new List<ArticleMetadata>(await client.GetArticleList());
                bool isEditable = (await this.userManager.GetUserAsync(this.User)).Id == user.Id;

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
                    return this.View("Index", Tuple.Create(articles, blogname, isEditable));
                }
                else
                {
                    return this.View("Display", Tuple.Create(article, articles, blogname, isEditable));
                }
            }
        }
    }

    public partial class BlogsController
    {
        private UserProfile GetBlogUser(string blogname)
        {
            if (string.IsNullOrWhiteSpace(blogname))
            {
                return null;
            }

            return this.userManager.Users.First(user => user.BlogName == blogname);
        }

        private DropboxClient GetClient(UserProfile user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.DropboxAccessToken))
            {
                return null;
            }

            return new DropboxClient(user.DropboxAccessToken, new DropboxClientConfig("SimpleBlogDemo"));
        }
    }
}
