using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShenDeng.Framework;
using ShenDeng.Framework.Handle;
using ShenDeng.Framework.Domain;
using ShenDeng.Domain;
using ShenDeng.application;
using System.Web.Script.Serialization;
using System.IO;
using ShenDeng.Web.Help;

namespace ShenDeng.Web.Areas.Admin.Controllers
{
    // *邮箱管理
    [FilterAuthority(Role.Admin)]
    public class MailController : Controller
    {
        private MailServer mailServer;
        public MailController()
        {
            mailServer = new MailServer();
        }
        // 显示所有
        public ActionResult Index()
        {
            var mails = mailServer.GetAll();
            return View(mails);
        }
        // 添加
        public JsonResult CreateMail()
        {
            try
            {
                var stream = new StreamReader(Request.InputStream);
                string str = stream.ReadToEnd();
                JavaScriptSerializer js = new JavaScriptSerializer();
                var datas = js.Deserialize<Dictionary<string, string>>(str);

                mailServer.CreateMail(datas[key.name], datas[key.mailaddre], datas[key.duty], datas[key.mailpwd]);
                var result = new { Result = "成功" };
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            catch(Exception err)
            {
                var result = new { Result = err.Message };
                return Json(result, JsonRequestBehavior.DenyGet);
            }
        }
        // 删除
        public ActionResult Delete(string dbid)
        {
            mailServer.DeleteMail(dbid);
            return RedirectToAction("Index");
        }
    }
}