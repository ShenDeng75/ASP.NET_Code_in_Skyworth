using System.Web.Mvc;
using ShenDeng.Framework.Application;
using ShenDeng.Framework.Base;
using ShenDeng.Framework.Domain;
using ShenDeng.Framework.Handle;
using System.Web.Script.Serialization;
using System.IO;
using System.Collections.Generic;
using ShenDeng.Web.Help;
using System;
using System.Collections;
using ShenDeng.Domain;
using ShenDeng.application;
using ShenDeng.Framework;
using System.Linq;

namespace ShenDeng.Web.Areas.Admin.Controllers
{
    [FilterAuthority(Role.Admin)]
    public class AdminController : Controller
    {
        public readonly IAccountService accountService;
        public readonly ExemplarServer exemplarServer;
        public AdminController(IAccountService accountService)
        {
            this.accountService = accountService;
            exemplarServer = UnityIoC.Get<ExemplarServer>();
        }
        // 撤销台账中的记录
        public ActionResult Backout(string dbid, string backrea)
        {
            exemplarServer.Back(dbid, backrea);
            return RedirectToAction("Index", "Ledger", new {area="" });
        }
        #region *账户管理*
        //显示账户
        public ActionResult ManageAccount()
        {
            var accounts = accountService.GetAllAccount();
            return View(accounts);
        }
        // 添加账户
        public JsonResult CreateAccount()
        {
            var stream = new StreamReader(Request.InputStream);
            string str = stream.ReadToEnd();
            JavaScriptSerializer js = new JavaScriptSerializer();
            try
            {
                var datas = js.Deserialize<Dictionary<string, object>>(str);
                accountService.CreateAccount(datas[key.username].ToString(), datas[key.jobnumber].ToString())
                    .SetPassWord("1")
                    .SetCanDelete(true);
                Account account = accountService.GetOneAccount(AccountIdentifier.of(datas[key.username].ToString()));
                List<string> roles = new List<string>((string[])((ArrayList)datas["roles"]).ToArray(typeof(string)));
                for (var i = 0; i < roles.Count; i++)
                {
                    int role = (int)Enum.Parse(typeof(Role), roles[i]);
                    account.AddRole(role);
                }
                accountService.Commit(); // 一个方法只能Commit一次

                var result = new{ Result = "成功" };
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            catch(Exception err)
            {
                var result = new{ Result = err.Message };
                return Json(result, JsonRequestBehavior.DenyGet);
            }
        }
        //删除账户
        public ActionResult Delete_Account(string id)
        {
            accountService.Delete(AccountIdentifier.of(id));
            return RedirectToAction("ManageAccount");
        }
        // 重置密码
        public JsonResult ResetPassword(string username)
        {
            try
            {
                accountService.ResetPassword(username);
                var result = new { Result = "成功" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch(Exception err)
            {
                var result = new { Result = err.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region *历史记录*
        // 撤销记录
        public ActionResult BackHist()
        {
            var _exemplars = exemplarServer.GetAll();
            var exemplars = from exe in _exemplars
                            where exe.Status == 0
                            select exe;
            return View(exemplars);
        }
        // 报废记录
        public ActionResult OverHist()
        {
            var _exemplars = exemplarServer.GetAll();
            var exemplars = from exe in _exemplars
                            where exe.Status == 5
                            select exe;
            return View(exemplars);
        }
        // 查询
        public ActionResult Find1(string fclosetime, string fwlno, string fwlname,
            string fwllclass, string fsup, string fver, string fvertime, string fstatus)
        {
            string command = string.Format("select * from Exemplar where SealedTime like '%{0}%' and Code like '%{1}%' and MaterialName like '%{2}%' and MaterialClass like '%{3}%' and Supplier like '%{4}%' and ValidTime like '%{5}%' and SignDate like '%{6}%' and ExemStatus like '%{7}%' and Status = '0';",
                                      fclosetime, fwlno, fwlname, fwllclass, fsup, fver, fvertime, fstatus);
            var exemplars = exemplarServer.Find2SQL(command);
            return View("BackHist", exemplars);
        }
        public ActionResult Find2(string fclosetime, string fwlno, string fwlname,
            string fwllclass, string fsup, string fver, string fvertime, string fstatus)
        {
            string command = string.Format("select * from Exemplar where SealedTime like '%{0}%' and Code like '%{1}%' and MaterialName like '%{2}%' and MaterialClass like '%{3}%' and Supplier like '%{4}%' and ValidTime like '%{5}%' and SignDate like '%{6}%' and ExemStatus like '%{7}%' and Status = '5';",
                                      fclosetime, fwlno, fwlname, fwllclass, fsup, fver, fvertime, fstatus);
            var exemplars = exemplarServer.Find2SQL(command);
            return View("OverHist", exemplars);
        }
        #endregion
    }
}