using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.Text;

namespace lookback
{
    public class AccessSecurity
    {
        /// <summary>
        /// 验证微博的access_token是否过期
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool IsWeiboTokenExpired(string token)
        {
            byte[] byteResp = null;
            string url = "https://api.weibo.com/oauth2/get_token_info";
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("access_token", token);
            using (var client = new WebClient())
            {
                byteResp = client.UploadValues(url, "POST", nvc);
            }

            string json = Encoding.Default.GetString(byteResp);
            dynamic obj = JsonConvert.DeserializeObject(json);

            if (obj.expire_in < 1000)
            {
                return true;
            }

            return false;
        }
    }
}