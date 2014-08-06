using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace lookback
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //return base.AuthorizeCore(httpContext);
            
            // 验证是否登录
            if (httpContext.Session["currentUserName"] == null)
            {
                return false;
            }

            // 验证token是否过期
            if (AccessSecurity.IsWeiboTokenExpired(Convert.ToString(httpContext.Session["access_token"])))
            {
                return false;
            }

            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //base.HandleUnauthorizedRequest(filterContext);
            filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(
                    new { controller = "Account", action = "Login" }
                ));
            
            // OR
            // filterContext.Result = new RedirectResult("/Account/Login");
        }

    }
}