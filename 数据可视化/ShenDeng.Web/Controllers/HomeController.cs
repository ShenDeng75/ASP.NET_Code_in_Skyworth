using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Web.Mvc;
using ShenDeng.application;
using System.Linq;
using System.IO;
using System.Web.Script.Serialization;

namespace ShenDeng.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index() 
        {
            return View();
        }
        // 填充下拉表单
        public ActionResult Find()
        {
            DataBaseServer dbserver = new DataBaseServer();
            var proc_items = dbserver.GetProcs();
            ViewData["downData"] = new SelectList(proc_items, "Value", "Text");

            var line_items = new List<SelectListItem>
                    {
                        new SelectListItem{Text="AL-01D-A"},
                        new SelectListItem{Text="AL-02D-B"},
                        new SelectListItem{Text="LL-01D"},
                        new SelectListItem{Text="LL-06D"}
                    };
            ViewData["line_items"] = new SelectList(line_items, "Text", "Text");
            return View();
        }
        // 返回显示的json数据
        [HttpPost]
        public ActionResult Ajax_Find()
        {
            var sr = new StreamReader(Request.InputStream); // 获取post请求中的表单数据流
            var stream = sr.ReadToEnd();
            var dict = String2Dict(stream);

            DataBaseServer dbserver = new DataBaseServer();
            var result = dbserver.GetFPY(dict["proc"], dict["line"], dict["stime"]); // 查询数据
            List<string> x = new List<string>();     // 准备ECharts需要的数据和格式
            List<int> passcount = new List<int>();
            List<int> failcount = new List<int>();
            List<double> rate = new List<double>();
            List<double> stime = new List<double>();
            foreach (var i in result)
            {
                x.Add(i["hour"]+"时");
                passcount.Add(int.Parse(i["passcount"]));
                failcount.Add(int.Parse(i["failcount"]));
                rate.Add(int.Parse(i["passcount"])*100 /int.Parse(i["scount"]));
                stime.Add(double.Parse(i["stime"]));
            }
            var json_data = new  // 合并到一起转json，注：一定要用这种方式，否则js不能直接解析。
            {
                hour = x,
                passcounts = passcount,
                failcounts = failcount,
                rates = rate,
                stimes = stime
            };
            return Json(json_data, JsonRequestBehavior.AllowGet);
        }
        // 解析返回的json流数据
        public Dictionary<string, string> String2Dict(string s)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            var lss = s.Split(new[] { '&' });
            foreach(var ls in lss)
            {
                var kv = ls.Split(new[] { '=' });
                dict[kv[0]] = kv.Length == 2 ? kv[1] : "";
            }
            return dict;
        }
       
    }
}