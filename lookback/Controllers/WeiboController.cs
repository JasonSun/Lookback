using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using lookback.ViewModels;

namespace lookback.Controllers
{
    /*
     * JSON online viewer
     * http://jsonviewer.stack.hu/
     * http://www.jsoneditoronline.org/
     */

    [CustomAuthorize]
    public class WeiboController : Controller
    {
        /// <summary>
        /// 自己的足迹
        /// </summary>
        /// <returns></returns>
        public ActionResult Myself()
        {
            string url, queryParams, json;
            List<LocationViewModel> lstLoc = new List<LocationViewModel>();
            JObject jObj;

            url = "https://api.weibo.com/2/place/user_timeline.json?";
            queryParams = "count=50" + "&access_token=" + Session["access_token"] + "&uid=" + Session["currentUserId"];
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                json = client.DownloadString(url + queryParams);
            }
            jObj = JObject.Parse(json);

            AddMarkers(lstLoc, jObj);

            // 用户所有有位置信息的微博数量
            int totalNum = (int)jObj["total_number"];
            // 当前已经处理的微博数
            int cnt = jObj["statuses"].Count();
            int page = 1;
            while (totalNum > cnt)
            {
                page++;
                queryParams = "count=50&page=" + page + "&access_token=" + Session["access_token"] + "&uid=" + Session["currentUserId"];
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    json = client.DownloadString(url + queryParams);
                }
                jObj = JObject.Parse(json);

                cnt += jObj["statuses"].Count();

                AddMarkers(lstLoc, jObj);
            }
            
            return View(lstLoc);
        }

        /// <summary>
        /// 关注朋友的足迹
        /// </summary>
        /// <returns></returns>
        public ActionResult Friend()
        {
            return View();
        }

        /// <summary>
        /// 将分页获取的足迹添加到足迹列表中
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="obj"></param>
        private void AddMarkers(List<LocationViewModel> lst, JObject obj)
        {
            foreach (var status in obj["statuses"])
            {
                LocationViewModel loc = new LocationViewModel();
                loc.Id = (Int64)status["id"];
                loc.Lat = (double)status["geo"]["coordinates"][0];
                loc.Lon = (double)status["geo"]["coordinates"][1];
                loc.Text = (string)status["text"];
                loc.CreateDate = DateTime.ParseExact((string)status["created_at"], "ddd MMM d HH:mm:ss zzz yyyy",
                    System.Globalization.CultureInfo.InvariantCulture).ToShortDateString();

                // set title
                if (status["annotations"] != null && status["annotations"][0]["place"] != null)
                {
                    loc.Title = (string)status["annotations"][0]["place"]["title"];
                }

                // set thumbnail
                if (status["pic_ids"].Count() != 0)
                {
                    string thumbnail = (string)status["pic_ids"].First;
                    string url = "http://ww2.sinaimg.cn/thumbnail/" + thumbnail + ".jpg";
                    loc.Thumbnail = url;
                }

                lst.Add(loc);
            }
        }

        /// <summary>
        /// 根据微博的id获取用户上传的图片信息
        /// </summary>
        /// <param name="statusId"></param>
        /// <returns></returns>
        public ActionResult GetStatusImages(string statusId)
        {
            string url, queryParams, json;
            List<string> pics = new List<string>();
            JObject jObj;
            url = "https://api.weibo.com/2/statuses/show.json?";
            queryParams = "access_token=" + Session["access_token"] + "&id=" + statusId;
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                json = client.DownloadString(url + queryParams);
            }
            jObj = JObject.Parse(json);
            foreach (var p in jObj["pic_urls"])
            {
                string thumbnail = (string)p["thumbnail_pic"];
                string bmiddle = thumbnail.Replace("thumbnail", "bmiddle");
                pics.Add(bmiddle);
            }

            return Json(pics, JsonRequestBehavior.AllowGet);
        }
    }
}
