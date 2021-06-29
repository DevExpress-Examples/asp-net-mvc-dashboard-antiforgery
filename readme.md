<!-- default file list -->
*Files to look at*:

* [Global.asax.cs](./CS/MVCxDashboardPreventCrossSiteRequestForgery/Global.asax.cs)
* [Index.cshtml](./CS/MVCxDashboardPreventCrossSiteRequestForgery/Views/Home/Index.cshtml)
* [CustomDashboardController.cs](./CS/MVCxDashboardPreventCrossSiteRequestForgery/Controllers/CustomDashboardController.cs)
<!-- default file list end -->

# ASP.NET MVC Dashboard - How to Prevent Cross-Site Request Forgery (CSRF) Attacks

The following example shows how to apply antiforgery request validation to the ASP.NET MVC Dashboard control.

## Example Overview

Follow the steps below to apply antiforgery request validation.

### Configure a custom dashboard controller

1. Create a custom dashboard controller. If you already have a custom controller, you can skip this step.

```cs
namespace MVCxDashboardPreventCrossSiteRequestForgery.Controllers {
    public class CustomDashboardController : DashboardController { 
    }
}
```

2. Change a default dashboard route to use the created controller.

```cs
routes.MapDashboardRoute("dashboardControl", "CustomDashboard", new string[] { "MVCxDashboardPreventCrossSiteRequestForgery.Controllers" });
```
3. Specify the controller name in the Web Dashboard settings.

```razor
@Html.DevExpress().Dashboard(settings => {
    ...
    settings.ControllerName = "CustomDashboard";
}).GetHtml()
```

###  Add validation for AntiforgeryToken

1. Add `@Html.AntiForgeryToken()` if you do not have this token on the page.

```cs
@Html.AntiForgeryToken()
@Html.DevExpress().Dashboard(settings => { .... }).GetHtml()
```

2. Implement the `DashboardValidateAntiForgeryTokenAttribute` attribute.

```cs
public sealed class DashboardValidateAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter {
	public void OnAuthorization(AuthorizationContext filterContext) {
		if (filterContext == null) {
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
 ```


3. Add the `DashboardValidateAntiForgeryTokenAttribute` attribute to the custom controller.

```cs
[DashboardValidateAntiForgeryTokenAttribute]
public class CustomDashboardController : DashboardController {   
}
``` 

4. Handle the `BeforeRender` event and configure the Web Dashboard control's backend options.

```razor
<script type="text/javascript">
    function onBeforeRender(sender) {
        var control = sender.GetDashboardControl();
        control.option('ajaxRemoteService.headers', { "__RequestVerificationToken": document.querySelector('input[name=__RequestVerificationToken]').value })
    }
</script>

...
@Html.DevExpress().Dashboard(settings => {
    ...
    settings.ClientSideEvents.BeforeRender = "onBeforeRender";
}).GetHtml()
```


## Documentation

- [Web Dashboard - Security Considerations](https://docs.devexpress.com/Dashboard/118651/web-dashboard/general-information/security-considerations)
- [CA3147: Mark verb handlers with ValidateAntiForgeryToken](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca3147)
- [ASP.NET MVC Security Best Practices - Preventing Cross-Site Request Forgery (CSRF)](https://github.com/DevExpress/aspnet-security-bestpractices/tree/master/SecurityBestPractices.Mvc#4-preventing-cross-site-request-forgery-csrf)


## More Examples

- [ASP.NET Core Dashboard - How to Prevent Cross-Site Request Forgery (CSRF) Attacks](https://github.com/DevExpress-Examples/asp-net-core-dashboard-antiforgery)
