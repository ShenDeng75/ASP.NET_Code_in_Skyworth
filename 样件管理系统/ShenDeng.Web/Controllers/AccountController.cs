using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ShenDeng.Framework;
using ShenDeng.Framework.Application;
using ShenDeng.Framework.Application.Imp;
using ShenDeng.Framework.Base;
using ShenDeng.Framework.DataBase;
using ShenDeng.Framework.Domain;
using ShenDeng.Framework.Handle;
using ShenDeng.Framework.Tools;

namespace ShenDeng.Web.Controllers
{
    public class AccountController : Controller
    {
        public readonly IAccountService service;   //账户服务
        public readonly MySessionService sessionService;   //http session 服务

        public AccountController(IAccountService service,
            MySessionService sessionService)
        {
            this.service = service;
            this.sessionService = sessionService;
        }
        public ActionResult Index()
        {
            return View();
        }
        #region *登录*
        // 登录
        public JsonResult Ajax_Login()
        {
            var stream = new StreamReader(Request.InputStream);
            var str = stream.ReadToEnd();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var datas = js.Deserialize<Dictionary<string, string>>(str);
            try
            {
                if (!service.Verify(datas["username"], datas["password"])) //如果密码不正确，或用户名不存在
                    throw new Exception("");
                Account account;
                if (datas["username"].ToLower().StartsWith("sdt")) // 如果用工号登录 
                    account = service.GetOneAccountByjobnumber(datas["username"]);
                else
                    account = service.GetOneAccount(AccountIdentifier.of(datas["username"]));
                sessionService.Login(account.Id.UserName, false);
                sessionService.SaveAccount(account);

                var result = new
                {
                    Result = "成功"
                };
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            catch
            {
                var result = new
                {
                    Result = "用户名或密码错误!"
                };
                return Json(result, JsonRequestBehavior.DenyGet);
            }
        }
        #endregion
        // 修改密码
        public JsonResult ResetPwd()
        {
            try
            {
                var stream = new StreamReader(Request.InputStream);
                string str = stream.ReadToEnd();
                JavaScriptSerializer js = new JavaScriptSerializer();
                var datas = js.Deserialize<List<string>>(str);
                service.ChangePassword(datas[0], datas[1]);

                var result = new { Result = "成功" };
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            catch(Exception err)
            {
                var result = new { Result = err.Message };
                return Json(result, JsonRequestBehavior.DenyGet);
            }
        }

        #region *注销*

        public ActionResult SignOut()
        {
            sessionService.SignOut();
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region *得到图片验证码并返回到前端*
        public ActionResult GetImg()
        {
            Image bmp = service.GetImage();  //得到图片
            MemoryStream ms = new MemoryStream();   //流
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);  //将图片保存为流
            return File(ms.ToArray(), "image/jpeg");  //返回类型为 image/jpeg 的流式文件
        }
        #endregion
    }
}