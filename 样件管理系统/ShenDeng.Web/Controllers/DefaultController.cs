using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShenDeng.Framework;
using ShenDeng.Framework.Base;
using ShenDeng.Framework.DataBase;

namespace ShenDeng.Web.Controllers
{
    public class DefaultController : Controller
    {
        public ActionResult Index(string message = "")
        {
            if (!string.IsNullOrEmpty(message))
                ViewData["message"] = message;
            return View();
        }
        // 初始化数据库
        public ActionResult InitDataBase()
        {
            CreateSessionFactory.InitDataBase(true);

            return RedirectToAction("Index", new { message = "数据库初始化成功！" });
        }
    }
}