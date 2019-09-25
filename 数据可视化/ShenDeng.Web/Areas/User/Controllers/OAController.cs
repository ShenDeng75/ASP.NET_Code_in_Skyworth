using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShenDeng.Framework.App_Start;
using ShenDeng.Framework.Domain;
using ShenDeng.Framework.Handle;

namespace ShenDeng.Web.Areas.User.Controllers
{
    [FilterAuthority(Role.User)]
    public class OAController : Controller
    {
        // GET: User/OA
        public ActionResult Index()
        {
            return View();
        }
    }
}