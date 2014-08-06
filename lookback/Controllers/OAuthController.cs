using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json;
using lookback.Models;

namespace lookback.Controllers
{
    public class OAuthController : Controller
    {
        /*
         * 自定义验证和授权的博文
         * http://www.dotnet-tricks.com/Tutorial/mvc/G54G220114-Custom-Authentication-and-Authorization-in-ASP.NET-MVC.html
         * 
         */

        public ActionResult Weibo()
        {
            string url = "https://api.weibo.com/oauth2/authorize?client_id='your-client-id'&redirect_uri=http://lookback.apphb.com/OAuth/LoginByWeibo&response_type=code";
            
            return Redirect(url);
        }

        public ActionResult LoginByWeibo(string code)
        {
            byte[] byteResp = null;
            string json;
            string access_token;
            string uid;
            string url = "https://api.weibo.com/oauth2/access_token";
            
            /*
             * Get access_token
             */
            using (var client = new WebClient())
            {
                NameValueCollection nvc = new NameValueCollection();
                nvc.Add("client_id", "your-client-id");
                nvc.Add("client_secret", "your-client-secret");
                nvc.Add("grant_type", "authorization_code");
                nvc.Add("redirect_uri", "http://lookback.apphb.com/OAuth/LoginByWeibo");
                nvc.Add("code", code);
                byteResp = client.UploadValues(url, "POST", nvc);
            }

            json = Encoding.Default.GetString(byteResp);
            Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            if (dict.ContainsKey("access_token"))
            {
                access_token = dict["access_token"];
                Session.Add("access_token", access_token);
                uid = dict["uid"];
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }

            using (var db = new AppContext())
            {
                AccountModel user = db.Accounts.Where(a => a.WeiboId == uid).FirstOrDefault();
                if (user != null)
                {
                    HttpContext.Session.Add("currentUserName", user.UserName);
                    HttpContext.Session.Add("currentUserId", user.WeiboId);
                    return RedirectToAction("Index", "Home");
                }
            }

            /*
             * Get user weibo profile
             */
            url = "https://api.weibo.com/2/users/show.json?access_token=" + access_token + "&uid=" + uid;

            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                json = client.DownloadString(url);
            }

            dynamic obj = JsonConvert.DeserializeObject(json);

            using (var db = new AppContext())
            {
                AccountModel user = new AccountModel();
                user.WeiboId = obj.id;
                user.UserName = obj.screen_name;
                user.NickName = obj.screen_name;
                user.Avatar50Url = obj.profile_image_url;
                user.Avatar180Url = obj.avatar_large;
                string d = obj.created_at;
                DateTime date = DateTime.ParseExact(d, "ddd MMM d HH:mm:ss zzzz yyyy", System.Globalization.CultureInfo.InvariantCulture);
                user.CreateDate = date.ToShortDateString();
                
                db.Accounts.Add(user);
                db.SaveChanges();

                HttpContext.Session.Add("currentUserName", user.UserName);
                HttpContext.Session.Add("currentUserId", user.WeiboId);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}

