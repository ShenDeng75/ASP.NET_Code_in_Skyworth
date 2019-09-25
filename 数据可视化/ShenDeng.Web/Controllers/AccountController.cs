using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

        #region *登录*
        public ActionResult Login(string message = "")
        {
            ViewData[Keys.ErrorMessage] = message;

            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection FC)
        {
            if (string.IsNullOrEmpty(FC[Keys.UserName]) ||
                string.IsNullOrEmpty(FC[Keys.PassWord]))
            {
                ViewData[Keys.ErrorMessage] = "用户名或密码不能为空！";
                return View();
            }
            if (string.IsNullOrEmpty(FC[Keys.VefCode]))
            {
                ViewData[Keys.ErrorMessage] = "验证码不能为空！";
                return View();
            }
            if (sessionService.GetVefCode().ToLower() != FC[Keys.VefCode].ToLower())
            {
                ViewData[Keys.ErrorMessage] = "验证码错误！";
                return View();
            }
            try
            {
                if (!service.Verify(FC[Keys.UserName], FC[Keys.PassWord])) //如果密码不正确，或用户名不存在
                    throw new Exception("");
                var account = service.GetOneAccount(AccountIdentifier.of(FC[Keys.UserName]));
                sessionService.Login(FC[Keys.UserName], false);
                sessionService.SaveAccount(account);
                if ((account.Roles & (int)Role.Admin) == (int)Role.Admin)
                    return RedirectToAction("Index", "Admin", new { Area = "Admin" });
                //return RedirectToAction("Index", "User", new {Area = "User"});
                return RedirectToAction("Index", "Home");
            }
            catch (Exception e)
            {
                ViewData[Keys.ErrorMessage] = "用户名或密码错误！";
                return View();
            }
        }
        #endregion

        #region *注册*
        public ActionResult Regist()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Regist(FormCollection FC)
        {
            if (string.IsNullOrEmpty(FC[Keys.UserName]) ||
                string.IsNullOrEmpty(FC[Keys.PassWord]))
            {
                ViewData[Keys.ErrorMessage] = "用户名或密码不能为空！";
                return View();
            }
            if (FC[Keys.PassWord] != FC[Keys.ConPassWord])
            {
                ViewData[Keys.ErrorMessage] = "两次密码不一致！";
                return View();
            }
            if (string.IsNullOrEmpty(FC[Keys.VefCode]))
            {
                ViewData[Keys.ErrorMessage] = "验证码不能为空！";
                return View();
            }
            if (sessionService.GetVefCode().ToLower() != FC[Keys.VefCode].ToLower())
            {
                ViewData[Keys.ErrorMessage] = "验证码错误！";
                return View();
            }
            try
            {
                service.CreateAccount(FC[Keys.UserName])
                    .SetPassWord(FC[Keys.PassWord])
                    .SetRole(Role.User)
                    .Commit();
            }
            catch (Exception e)
            {
                ViewData[Keys.ErrorMessage] = e.Message;
                return View();
            }
            return RedirectToAction("Login", new { message = "注册成功" });
        }
        #endregion

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