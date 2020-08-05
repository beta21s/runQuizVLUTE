using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace runQuickVLUTE
{
    public class FireBaseSetting
    {
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public string version { get; set; }

        public static T getData<T>() where T : new()
        {
            using (var w = new WebClient())
            {
                var tmp = string.Empty;
                try
                {
                    w.Encoding = Encoding.UTF8;
                    tmp = w.DownloadString("https://runquizvlute.firebaseio.com/.json");
                }
                catch (Exception) { }
                return !string.IsNullOrEmpty(tmp) ? JsonConvert.DeserializeObject<T>(tmp) : new T();
            }
        }
    }
}
