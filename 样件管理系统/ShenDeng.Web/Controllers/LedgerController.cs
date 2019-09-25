using ShenDeng.application;
using ShenDeng.Framework;
using ShenDeng.Framework.DataBase;
using ShenDeng.Framework.Domain;
using ShenDeng.Framework.Handle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ShenDeng.Framework.Base;

namespace ShenDeng.Web.Controllers
{
    // ***样件台账***
    [FilterAuthority(Role.Find)]
    public class LedgerController : Controller
    {
        ExemplarServer server = new ExemplarServer(UnityIoC.Get<IRepository>());
        IMySessionService sessionService = UnityIoC.Get<IMySessionService>();
        public ActionResult Index()
        {
            ViewData["account"] = sessionService.GetAccount();
            string nowYear = DateTime.Now.ToLocalTime().ToString("yyyy");
            string command = string.Format("select * from Exemplar where SealedTime like '%{0}%' and Status = '4';", nowYear);
            var exemplars = server.Find2SQL(command);
            return View(exemplars);
        }
        // 查找
        public ActionResult Find(string fclosetime, string fwlno, string fwlname,
            string fwllclass, string fsup, string fver, string fvertime, string fstatus)
        {
            if (fclosetime == "")
                fclosetime = DateTime.Now.ToLocalTime().ToString("yyyy");
            string command = string.Format("select * from Exemplar where SealedTime like '%{0}%' and Code like '%{1}%' and MaterialName like '%{2}%' and MaterialClass like '%{3}%' and Supplier like '%{4}%' and ValidTime like '%{5}%' and SignDate like '%{6}%' and ExemStatus like '%{7}%' and Status = '4';",
                                      fclosetime, fwlno, fwlname, fwllclass, fsup, fver, fvertime, fstatus);
            var exemplars = server.Find2SQL(command);
            return View("Index", exemplars);
        }
        // 导入历史数据
        public ActionResult HistoryData(HttpPostedFileBase file)
        {
            var hzs = file.FileName.Split(new[] { '.' });
            var hz = hzs[hzs.Length - 1];
            string filepath = Server.MapPath("~/File/历史数据."+hz);
            if (System.IO.File.Exists(filepath))
                System.IO.File.Delete(filepath);
            file.SaveAs(filepath);
            server.LedgerImport(filepath);
            return RedirectToAction("Index");
        }
        // 导出
        public JsonResult Output2Excel()
        {
            try
            {
                var stream = new StreamReader(Request.InputStream);
                string str = stream.ReadToEnd();
                JavaScriptSerializer js = new JavaScriptSerializer();
                var datas = js.Deserialize<List<string>>(str);

                var now = DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd hh-mm-ss");
                var _path = Server.MapPath("~/File/");
                var path = server.Output(datas, _path);  // 导出

                var result = new { Result = "成功" , filepath = path};
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