using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using lookback.Models;
using Newtonsoft.Json;

namespace lookback.Controllers
{
    [CustomAuthorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (Session["currentUserId"] != null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            Session.Clear();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult MyProfile()
        {
            string url, queryParams, json;

            // user info
            url = "https://api.weibo.com/2/users/show.json?";
            queryParams = "access_token=" + Session["access_token"] + "&uid=" + Session["currentUserId"];
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                json = client.DownloadString(url + queryParams);
            }
            dynamic user = JsonConvert.DeserializeObject(json);
            ViewBag.User = user;

            // user checkins
            // 默认选出前六个checkins（相同地点只显示一次）展示在用户个人信息页面中
            url = "https://api.weibo.com/2/place/users/checkins.json?";
            queryParams = "access_token=" + Session["access_token"] + "&uid=" + Session["currentUserId"] + "&count=6";
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                json = client.DownloadString(url + queryParams);
            }
            dynamic checkins = JsonConvert.DeserializeObject(json);
            HashSet<string> hash = new HashSet<string>();
            foreach (var poi in checkins.pois)
            {
                hash.Add((string)poi.title);
            }
            ViewBag.Checkins = hash;

            return View();
        }

        public ActionResult GetAvatar(int size)
        {
            string uid = Session["currentUserId"].ToString();
            AccountModel user = null;
            byte[] avatar = null;

            using (var db = new AppContext())
            {
                user = db.Accounts.Where(a => a.WeiboId == uid).ToList().First();
            }
            
            using (var client = new WebClient())
            {
                switch (size)
                {
                    case 50:
                        avatar = client.DownloadData(user.Avatar50Url);
                        break;
                    case 180:
                    default:
                        avatar = client.DownloadData(user.Avatar180Url);
                        break;
                }
            }
            
            return File(avatar, "image/jpeg");
        }
    }
}
