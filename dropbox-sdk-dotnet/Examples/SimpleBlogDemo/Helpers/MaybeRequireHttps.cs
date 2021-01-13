// <copyright file="MaybeRequireHttps.cs" company="Dropbox Inc">
// Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>

namespace SimpleBlogDemo.Helpers
{
    using System;
    using System.Net;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Method,
        Inherited = true,
        AllowMultiple = false)]
    public class RequireHttpsOrXForwardedAttribute : RequireHttpsAttribute
    {
        public override void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (filterContext.HttpContext.Request.IsHttps)
            {
                return;
            }

            if (string.Equals(
                filterContext.HttpContext.Request.Headers["X-Forwarded-Proto"],
                "https",
                StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            if (IPAddress.IsLoopback(filterContext.HttpContext.Connection.RemoteIpAddress))
            {
                return;
            }

            this.HandleNonHttpsRequest(filterContext);
        }
    }
}
