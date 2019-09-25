using ShenDeng.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShenDeng.Web.Help
{
    public static class tools
    {
        // 根据字符串形式的时间，返回天数
        public static int FormatDays(string time)
        {
            int days = 0;
            string dw = time[time.Length - 1].ToString();
            var t = int.Parse(time.Substring(0, time.Length - 1));
            if (dw == "月")
                days = t * 30;
            else if (dw == "年")
                days = t * 365;
            return days;
        }
        // 解析返回的json流数据
        public static Dictionary<string, string> String2Dict(string s)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            var lss = s.Split(new[] { '&' });
            foreach (var ls in lss)
            {
                var kv = ls.Split(new[] { '=' });
                dict[kv[0]] = kv.Length == 2 ? HttpUtility.UrlDecode(HttpUtility.UrlDecode(kv[1])) : "";
            }
            return dict;
        }
        
    }
}