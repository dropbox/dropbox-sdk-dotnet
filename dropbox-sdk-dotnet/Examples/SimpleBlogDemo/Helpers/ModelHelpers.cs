using Dropbox.Api;
using SimpleBlogDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMatrix.WebData;

namespace SimpleBlogDemo.Helpers
{
    public static class ModelHelpers
    {
        public static UserProfile CurrentUser(this UsersStore store)
        {
            if (!WebSecurity.IsAuthenticated)
            {
                return null;
            }

            var users = from u in store.Users
                        where u.ID == WebSecurity.CurrentUserId
                        select u;
            return users.Single();
        }

        public static DropboxClient GetAuthenticatedClient(this UserProfile user)
        {
            if (!WebSecurity.IsAuthenticated ||
                user.ID != WebSecurity.CurrentUserId ||
                string.IsNullOrWhiteSpace(user.DropboxAccessToken))
            {
                return null;
            }

            return new DropboxClient(user.DropboxAccessToken, new DropboxClientConfig("SimpleBlogDemo"));
        }
    }
}
