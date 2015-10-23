namespace SimpleBlogDemo.Helpers
{
    using System;
    using System.Web.Configuration;
    using System.Web.Mvc;

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true,
		AllowMultiple = false)]
	public class RequireHttpsOrXForwardedAttribute : RequireHttpsAttribute
	{
        private string Environment = WebConfigurationManager.AppSettings["Environment"];
         
		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			if (filterContext == null)
			{
				throw new ArgumentNullException("filterContext");
			}

			if (filterContext.HttpContext.Request.IsSecureConnection)
			{
				return;
			}
 
			if (string.Equals(filterContext.HttpContext.Request.Headers["X-Forwarded-Proto"],
				"https",
				StringComparison.InvariantCultureIgnoreCase))
			{
				return;
			}
 
			if (filterContext.HttpContext.Request.IsLocal)
			{
				return;
			}
 
			HandleNonHttpsRequest(filterContext);
		}
	}
}