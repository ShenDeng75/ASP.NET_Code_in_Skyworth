using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShenDeng.Framework.Handle;
using ShenDeng.Framework.Domain;
using ShenDeng.application;
using ShenDeng.Framework;
using ShenDeng.Framework.DataBase;
using System.IO;
using System.Web.Script.Serialization;

namespace ShenDeng.Web.Controllers
{
    // ***超期处理***
    [FilterAuthority(Role.OverHandl)]
    public class OverHandlController : Controller
    {
        ExemplarServer server = new ExemplarServer(UnityIoC.Get<IRepository>());
        public ActionResult Index()
        {
            var _exemplars = server.GetAll();
            var exemplars = from exe in _exemplars
                            where exe.Status == 4 && exe.ExemStatus == "超期"
                            select exe;
            return View(exemplars);
        }
        // 查找
        public ActionResult Find(string fclosetime, string fwlno, string fwlname,
            string fwllclass, string fsup, string fver, string fvertime)
        {
            string command = string.Format("select * from Exemplar where SealedTime like '%{0}%' and Code like '%{1}%' and MaterialName like '%{2}%' and MaterialClass like '%{3}%' and Supplier like '%{4}%' and ValidTime like '%{5}%' and SignDate like '%{6}%' and Status = '4';",
                                      fclosetime, fwlno, fwlname, fwllclass, fsup, fver, fvertime);
            var _exemplars = server.Find2SQL(command);
            var exemplars = from exe in _exemplars
                            where exe.ExemStatus == "超期"
                            select exe;
            return View("Index", exemplars);
        }
        // 提交报废
        public JsonResult SubmitOver()
        {
            try
            {
                var stream = new StreamReader(Request.InputStream);
                string str = stream.ReadToEnd();
                JavaScriptSerializer js = new JavaScriptSerializer();
                var datas = js.Deserialize<List<string>>(str);
                server.OverH(datas);

                var result = new { Result = "成功" };
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            catch (Exception err)
            {
                var result = new { Result = err.Message };
                return Json(result, JsonRequestBehavior.DenyGet);
            }
        }
    }
}