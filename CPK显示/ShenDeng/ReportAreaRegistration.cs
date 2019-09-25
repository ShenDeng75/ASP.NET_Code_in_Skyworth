using System.Web.Mvc;

namespace ShenDeng
{
    public class ShenDengAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ShenDeng";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ShenDeng_default",
                "ShenDeng/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new string[] { "ShenDeng.Controllers" }
            );
        }
    }
}