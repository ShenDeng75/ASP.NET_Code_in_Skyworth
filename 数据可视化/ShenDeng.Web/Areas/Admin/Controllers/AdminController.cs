using System.Web.Mvc;
using ShenDeng.Framework.Application;
using ShenDeng.Framework.Base;
using ShenDeng.Framework.Domain;
using ShenDeng.Framework.Handle;


namespace ShenDeng.Web.Areas.Admin.Controllers
{
    [FilterAuthority(Role.Admin)]
    public class AdminController : Controller
    {
        public readonly IAccountService accountService;
        public AdminController(IAccountService accountService)
        {
            this.accountService = accountService;
        }
        //首页
        public ActionResult Index()
        {
            return View();
        }
        #region *账户管理*
        //显示账户
        public ActionResult ManageAccount()
        {
            var accounts = accountService.GetAllAccount();
            return View(accounts);
        }
        //删除账户
        public ActionResult Delete_Account(string id)
        {
            accountService.Delete(AccountIdentifier.of(id));
            return RedirectToAction("ManageAccount");
        }
        #endregion

        #region *工具管理*
        public ActionResult ManageTools()
        {
            return View();
        }

        public ActionResult UpFile()
        {
            return View();
        }
        #endregion
    }
}