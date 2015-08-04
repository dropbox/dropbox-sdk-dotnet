using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

using SimpleBlogDemo.Helpers;
using System.Globalization;
using SimpleBlogDemo.Models;
using System.Web.Configuration;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Script.Serialization;
using Dropbox.Api;

namespace SimpleBlogDemo.Controllers
{
    [RequireHttpsOrXForwarded]
    public partial class HomeController : AsyncController
    {
        private UsersStore store = new UsersStore();
        private UserProfile currentUser;

        private string RedirectUri
        {
            get
            {
                if (this.Request.Url.Host.ToLowerInvariant() == "localhost")
                {
                    return "http://localhost:5000/Home/Auth";
                }

                var builder = new UriBuilder(
                    Uri.UriSchemeHttps,
                    this.Request.Url.Host);

                builder.Path = "/Home/Auth";

                return builder.ToString();
            }
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            this.currentUser = this.store.CurrentUser();

            base.Initialize(requestContext);
        }

        // GET: /Home
        public async Task<ActionResult> IndexAsync()
        {
            if (WebSecurity.IsAuthenticated)
            {
                return this.RedirectToAction("Profile");
            }

            var blogs = new List<Blog>();

            foreach (var user in store.Users)
            {
                var blog = await Blog.FromUserAsync(user);

                if (blog != null)
                {
                    blogs.Add(blog);
                }
            }

            blogs.Sort((l, r) => r.MostRecent.CompareTo(l.MostRecent));

            return View(blogs);
        }

        // GET: /Home/Register
        public ActionResult Register()
        {
            if (WebSecurity.IsAuthenticated)
            {
                return this.RedirectToAction("Profile");
            }

            return View();
        }

        // POST: /Home/Register
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Register(string username, string password1, string password2, string blogname, bool remember = false)
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
            else if (WebSecurity.UserExists(username))
            {
                this.Flash(string.Format(
                    CultureInfo.InvariantCulture,
                    "Username '{0}' already exists, choose something else.",
                    username),
                    FlashLevel.Warning);
            }
            else
            {
                var parameters = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(blogname))
                {
                    parameters["BlogName"] = blogname;
                }
                WebSecurity.CreateUserAndAccount(username, password1, parameters, requireConfirmationToken: false);
                WebSecurity.Login(username, password1, persistCookie: remember);
                return this.RedirectToAction("Profile");
            }

            return View();
        }

        // Get /Home/Profile
        [Authorize]
        public async Task<ActionResult> ProfileAsync()
        {
            await Task.Delay(0);

            return View(this.currentUser);
        }

        // Post /Home/Profile
        [HttpPost, ValidateAntiForgeryToken, Authorize]
        public async Task<ActionResult> ProfileAsync(UserProfile profile)
        {
            if (this.currentUser.ID != profile.ID)
            {
                return this.RedirectToAction("Index");
            }

            this.currentUser.BlogName = profile.BlogName;
            await this.store.SaveChangesAsync();

            return View(this.currentUser);
        }

        // POST : /Home/Disconnect
        [HttpPost, ValidateAntiForgeryToken, Authorize]
        public async Task<ActionResult> DisconnectAsync()
        {
            if (!string.IsNullOrWhiteSpace(this.currentUser.BlogName))
            {
                this.FlushCache(this.currentUser.BlogName);
            }

            this.currentUser.DropboxAccessToken = string.Empty;
            await store.SaveChangesAsync();

            this.Flash("This account has been disconnected from Dropbox.", FlashLevel.Success);
            return this.RedirectToAction("Profile");
        }

        // GET: /Home/Connect
        [Authorize]
        public ActionResult Connect()
        {
            if (!string.IsNullOrWhiteSpace(this.currentUser.DropboxAccessToken))
            {
                return this.RedirectToAction("Index");
            }

            this.currentUser.ConnectState = Guid.NewGuid().ToString("N");
            store.SaveChanges();

            var redirect = DropboxOAuth2Helper.GetAuthorizeUri(
                OAuthResponseType.Code,
                MvcApplication.AppKey,
                RedirectUri,
                this.currentUser.ConnectState);

            return Redirect(redirect.ToString());
        }

        // GET: /Home/Auth
        [Authorize]
        public async Task<ActionResult> AuthAsync(string code, string state)
        {
            try
            {
                if (this.currentUser.ConnectState != state)
                {
                    this.Flash("There was an error connecting to Dropbox.");
                    return this.RedirectToAction("Index");
                }

                var response = await DropboxOAuth2Helper.ProcessCodeFlowAsync(
                    code,
                    MvcApplication.AppKey,
                    MvcApplication.AppSecret,
                    this.RedirectUri);

                this.currentUser.DropboxAccessToken = response.AccessToken;
                this.currentUser.ConnectState = string.Empty;
                await this.store.SaveChangesAsync();

                this.Flash("This account has been connected to Dropbox.", FlashLevel.Success);
                return RedirectToAction("Profile");
            }
            catch(Exception e)
            {
                var message = string.Format(
                    "code: {0}\nAppKey: {1}\nAppSecret: {2}\nRedirectUri: {3}\nException : {4}",
                    code,
                    MvcApplication.AppKey,
                    MvcApplication.AppSecret,
                    this.RedirectUri,
                    e);
                this.Flash(message, FlashLevel.Danger);
                return RedirectToAction("Profile");
            }
        }

        // POST: /Home/LogOff
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            if (WebSecurity.IsAuthenticated)
            {
                WebSecurity.Logout();
            }

            return RedirectToAction("Index");
        }

        // GET: /Home/SignIn
        public ActionResult SignIn(string returnUrl=null)
        {
            if (WebSecurity.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Profile");
                }
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SignIn(string username, string password, string returnUrl = null, bool remember = false)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                this.Flash("Incorrect username or password.", FlashLevel.Danger);
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            if (!WebSecurity.IsAuthenticated)
            {
                var result = WebSecurity.Login(username, password, persistCookie: remember);
                if (!result)
                {
                    this.Flash("Incorrect username or password.", FlashLevel.Warning);
                    ViewBag.ReturnUrl = returnUrl;
                    return View();
                }
                else if (remember)
                {
                    Response.Cookies.Get(".ASPXAUTH").Expires = DateTime.UtcNow.AddDays(30);
                }
           }

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Profile");
            }
        }
    }
}
