using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShenDeng.Application;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Data;
using System.Web.Script.Serialization;
using System.IO;

namespace ShenDeng.Controllers
{
    // CPK表格显示
    public class ShowCPKController : Controller
    {
        ShowCPK dbserver = new ShowCPK();  // 数据库服务

        // 显示默认
        public ActionResult Index()
        {
            return View();
        }
        // Ajax查询
        public JsonResult Find()
        {
            try
            {
                var stream = new StreamReader(Request.InputStream);
                string str = stream.ReadToEnd();
                JavaScriptSerializer js = new JavaScriptSerializer();
                var datas = js.Deserialize<Dictionary<string, string>>(str);

                DataTable dt = dbserver.GetCPKs(datas["time"].Trim(), datas["field"].Trim());
                var jsondata = dbserver.GetResult(dt, datas["field"].Trim());
                var result = new { code = true, result = jsondata };

                return Json(result, JsonRequestBehavior.DenyGet);
            }
            catch(Exception err)
            {
                var result = new { code = false, result = err.Message };
                return Json(result, JsonRequestBehavior.DenyGet);
            }
        }
    }
}