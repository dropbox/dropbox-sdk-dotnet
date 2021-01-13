// <copyright file="HomeController.cs" company="Dropbox Inc">
// Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>

namespace SimpleBlogDemo.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Dropbox.Api;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using SimpleBlogDemo.Helpers;
    using SimpleBlogDemo.Models;

    [RequireHttpsOrXForwarded]
    public partial class HomeController : Controller
    {
        private readonly SignInManager<UserProfile> signInManager;
        private readonly UserManager<UserProfile> userManager;
        private IConfiguration configuration;

        public HomeController(IConfiguration configuration, UserManager<UserProfile> userManager, SignInManager<UserProfile> signInManager)
        {
            this.configuration = configuration;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        private string RedirectUri
        {
            get
            {
                if (this.Request.Host.Host.ToLowerInvariant() == "localhost")
                {
                    return string.Format("http://{0}:{1}/Home/Auth", this.Request.Host.Host, this.Request.Host.Port);
                }

                var builder = new UriBuilder(
                    Uri.UriSchemeHttps,
                    this.Request.Host.ToString());

                builder.Path = "/Home/Auth";

                return builder.ToString();
            }
        }

        // GET: /Home
        public async Task<ActionResult> IndexAsync()
        {
            if (this.userManager.GetUserId(this.User) != null)
            {
                return this.RedirectToAction("Profile");
            }

            var blogs = new List<Blog>();

            foreach (var user in this.userManager.Users)
            {
                var blog = await Blog.FromUserAsync(user);

                if (blog != null)
                {
                    blogs.Add(blog);
                }
            }

            blogs.Sort((l, r) => r.MostRecent.CompareTo(l.MostRecent));

            return this.View(blogs);
        }

        // GET: /Home/Register
        public ActionResult Register()
        {
            if (this.userManager.GetUserId(this.User) != null)
            {
                return this.RedirectToAction("Profile");
            }

            return this.View();
        }

        // POST: /Home/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(string username, string password1, string password2, string blogname, bool remember = false)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                this.Flash("Username must not be empty.", FlashLevel.Warning);
            }
            else if (string.IsNullOrWhiteSpace(password1) ||
                string.IsNullOrWhiteSpace(password2) ||
                !string.Equals(password1, password2, StringComparison.Ordinal))
            {
                this.Flash("Password must not be empty and both passwords must match.", FlashLevel.Warning);
            }
            else if (await this.userManager.FindByNameAsync(username) != null)
            {
                this.Flash(
                    string.Format(
                    CultureInfo.InvariantCulture,
                    "Username '{0}' already exists, choose something else.",
                    username),
                    FlashLevel.Warning);
            }
            else
            {
                var user = new UserProfile
                {
                    UserName = username,
                    BlogName = blogname,
                };
                await this.userManager.CreateAsync(user, password1);
                await this.signInManager.PasswordSignInAsync(user, password1, remember, false);
                return this.RedirectToAction("Profile");
            }

            return this.View();
        }

        // Get /Home/Profile
        [Authorize]
        public async Task<ActionResult> ProfileAsync()
        {
            await Task.Delay(0);

            return this.View(await this.GetCurrentUser(this.HttpContext));
        }

        // Post /Home/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> ProfileAsync(UserProfile profile)
        {
            var currentUser = await this.GetCurrentUser(this.HttpContext);
            if (currentUser.Id != profile.Id)
            {
                return this.RedirectToAction("Index");
            }

            currentUser.BlogName = profile.BlogName;
            await this.userManager.UpdateAsync(currentUser);

            return this.View(currentUser);
        }

        // POST : /Home/Disconnect
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> DisconnectAsync()
        {
            var currentUser = await this.GetCurrentUser(this.HttpContext);
            if (!string.IsNullOrWhiteSpace(currentUser.BlogName))
            {
                this.FlushCache(currentUser.BlogName);
            }

            currentUser.DropboxAccessToken = string.Empty;
            await this.userManager.UpdateAsync(currentUser);

            this.Flash("This account has been disconnected from Dropbox.", FlashLevel.Success);
            return this.RedirectToAction("Profile");
        }

        // GET: /Home/Connect
        [Authorize]
        public async Task<ActionResult> Connect()
        {
            var currentUser = await this.GetCurrentUser(this.HttpContext);
            if (!string.IsNullOrWhiteSpace(currentUser.DropboxAccessToken))
            {
                return this.RedirectToAction("Index");
            }

            currentUser.ConnectState = Guid.NewGuid().ToString("N");
            await this.userManager.UpdateAsync(currentUser);

            var redirect = DropboxOAuth2Helper.GetAuthorizeUri(
                OAuthResponseType.Code,
                this.configuration["AppKey"],
                this.RedirectUri,
                currentUser.ConnectState);

            return this.Redirect(redirect.ToString());
        }

        // GET: /Home/Auth
        [Authorize]
        public async Task<ActionResult> AuthAsync(string code, string state)
        {
            Console.WriteLine("auth");
            var currentUser = await this.GetCurrentUser(this.HttpContext);
            try
            {
                if (currentUser.ConnectState != state)
                {
                    this.Flash("There was an error connecting to Dropbox.");
                    return this.RedirectToAction("Index");
                }

                var response = await DropboxOAuth2Helper.ProcessCodeFlowAsync(
                    code,
                    this.configuration["AppKey"],
                    this.configuration["AppSecret"],
                    this.RedirectUri);

                currentUser.DropboxAccessToken = response.AccessToken;
                currentUser.ConnectState = string.Empty;
                await this.userManager.UpdateAsync(currentUser);

                this.Flash("This account has been connected to Dropbox.", FlashLevel.Success);
                return this.RedirectToAction("Profile");
            }
            catch (Exception e)
            {
                var message = string.Format(
                    "code: {0}\nAppKey: {1}\nAppSecret: {2}\nRedirectUri: {3}\nException : {4}",
                    code,
                    this.configuration["AppKey"],
                    this.configuration["AppSecret"],
                    this.RedirectUri,
                    e);
                this.Flash(message, FlashLevel.Danger);
                return this.RedirectToAction("Profile");
            }
        }

        // POST: /Home/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogOff()
        {
            if (this.userManager.GetUserId(this.User) != null)
            {
                await this.signInManager.SignOutAsync();
            }

            return this.RedirectToAction("Index");
        }

        // GET: /Home/SignIn
        public ActionResult SignIn(string returnUrl = null)
        {
            if (this.userManager.GetUserId(this.User) != null)
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return this.Redirect(returnUrl);
                }
                else
                {
                    return this.RedirectToAction("Profile");
                }
            }

            this.ViewBag.ReturnUrl = returnUrl;
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignIn(string username, string password, string returnUrl = null, bool remember = false)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                this.Flash("Incorrect username or password.", FlashLevel.Danger);
                this.ViewBag.ReturnUrl = returnUrl;
                return this.View();
            }

            if (this.userManager.GetUserId(this.User) == null)
            {
                var result = await this.signInManager.PasswordSignInAsync(username, password, remember, false);
                if (!result.Succeeded)
                {
                    this.Flash("Incorrect username or password.", FlashLevel.Warning);
                    this.ViewBag.ReturnUrl = returnUrl;
                    return this.View();
                }
                else if (remember)
                {
                    // TODO: fixme
                    // Response.Cookies.Get(".ASPXAUTH").Expires = DateTime.UtcNow.AddDays(30);
                }
            }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return this.Redirect(returnUrl);
            }
            else
            {
                return this.RedirectToAction("Profile");
            }
        }

        private async Task<UserProfile> GetCurrentUser(HttpContext context)
        {
            return await this.userManager.GetUserAsync(context.User);
        }
    }
}
