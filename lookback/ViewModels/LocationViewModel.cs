using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lookback.ViewModels
{
    public class LocationViewModel
    {
        public Int64 Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Thumbnail { get; set; }
        public double Lon { get; set; }
        public double Lat { get; set; }
        public string CreateDate { get; set; }

        public LocationViewModel()
        {
            this.Title = "未知地名";
        }
    }
}