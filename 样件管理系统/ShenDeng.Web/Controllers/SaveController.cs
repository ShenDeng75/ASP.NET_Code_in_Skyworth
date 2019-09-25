using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShenDeng.Framework.Handle;
using ShenDeng.Framework.Domain;
using ShenDeng.Domain;
using ShenDeng.application;
using ShenDeng.Framework;
using ShenDeng.Framework.DataBase;
using System.IO;
using System.Web.Script.Serialization;

namespace ShenDeng.Web.Controllers
{
    // ***样件存储***
    [FilterAuthority(Role.Save)]
    public class SaveController : Controller
    {
        ExemplarServer server = new ExemplarServer(UnityIoC.Get<IRepository>());
        // 显示所有
        public ActionResult Index()
        {
            var _exemplars = server.GetAll();
            var exemplars = from exe in _exemplars
                            where exe.Status == 3
                            select exe;
            return View(exemplars);
        }
        // 退回
        public ActionResult Back(string dbid, string backrea)
        {
            server.BackSave(dbid, backrea);
            return RedirectToAction("Index");
        }
        // 查找
        public ActionResult Find(string fclosetime, string fwlno, string fwlname,
            string fwllclass, string fsup, string fver, string fvertime)
        {
            string command = string.Format("select * from Exemplar where SealedTime like '%{0}%' and Code like '%{1}%' and MaterialName like '%{2}%' and MaterialClass like '%{3}%' and Supplier like '%{4}%' and ValidTime like '%{5}%' and SignDate like '%{6}%' and Status = '3';",
                                      fclosetime, fwlno, fwlname, fwllclass, fsup, fver, fvertime);
            var exemplars = server.Find2SQL(command);
            return View("Index", exemplars);
        }
        // 提交
        public JsonResult Submit()
        {
            var stream = new StreamReader(Request.InputStream);
            string str = stream.ReadToEnd();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var datas = js.Deserialize<Dictionary<string, string>>(str);
            server.SubmitSave(datas);

            var result = new { Result = "成功" };
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        // 验证样件是否变更
        public JsonResult Verfi()
        {
            var stream = new StreamReader(Request.InputStream);
            string str = stream.ReadToEnd();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var datas = js.Deserialize<List<string>>(str);
            var msg = "成功";
            var ok = server.VerStatus(datas, 3);
            if (!ok)
                msg = "样件已变更";

            var result = new { Result = msg };
            return Json(result, JsonRequestBehavior.DenyGet);
        }
    }
}