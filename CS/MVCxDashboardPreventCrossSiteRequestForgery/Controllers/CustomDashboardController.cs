using DevExpress.DashboardWeb.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace MVCxDashboardPreventCrossSiteRequestForgery.Controllers
{
    [DashboardValidateAntiForgeryTokenAttribute]
    public class CustomDashboardController : DashboardController
    {
        
    }


    public sealed class DashboardValidateAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException(nameof(filterContext));
            }

            HttpContextBase httpContext = filterContext.HttpContext;
            HttpRequestBase request = httpContext.Request;
            HttpCookie cookie = request.Cookies[AntiForgeryConfig.CookieName];
            string token = request.Headers["__RequestVerificationToken"];
            if (string.IsNullOrEmpty(token)) {
                token = request.Form["__RequestVerificationToken"];
            }
            AntiForgery.Validate(cookie?.Value, token);
        }
    }
}