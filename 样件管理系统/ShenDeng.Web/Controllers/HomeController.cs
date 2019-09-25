using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Web.Mvc;
using ShenDeng.application;
using System.Linq;
using System.IO;
using System.Web.Script.Serialization;
using System.Web;
using ShenDeng.Web.Help;
using ShenDeng.Framework;
using ShenDeng.Framework.DataBase;
using System.Text;
using System;
using ShenDeng.Framework.Domain;
using ShenDeng.Framework.Handle;

namespace ShenDeng.Web.Controllers
{
    // ***样件导入***
    [FilterAuthority(Role.Admin)]
    public class HomeController : Controller
    {
        ExemplarServer server = new ExemplarServer(UnityIoC.Get<IRepository>());

        public ActionResult Index()
        {
            return View();
        }
        // 显示导入的样件
        public ActionResult ExepImport()
        {
            var _exemplars = server.GetAll();
            var exemplars = from exe in _exemplars
                            where exe.Status == 1
                            select exe;
            foreach (var item in exemplars)  // 设置样件状态和超期月数
            {
                int days = tools.FormatDays(item.ValidTime);
                var endtime = item.SealedTime.AddDays(days);
                var isover = endtime < DateTime.Now.ToLocalTime();  // 是否超期
                if (isover)
                {
                    item.ExemStatus = "超期";
                    item.LimitMonth = ((int)(DateTime.Now.ToLocalTime() - endtime).TotalDays / 30).ToString();
                }
                else
                {
                    item.ExemStatus = "有效期内";
                    item.LimitMonth = "0";
                }
            }
            server.Commit();
            return View(exemplars);
        }
        // 添加单个样件
        [HttpPost]
        public JsonResult ImportOne()
        {
            var str = new StreamReader(Request.InputStream);
            var stream = str.ReadToEnd();
            var datas = tools.String2Dict(stream);
            
            var wlclass = datas[key.wlclass];
            var validtime = "";
            if (datas.ContainsKey("validtime"))
                validtime = datas[key.validtime];
            else
                validtime = datas[key.other];
            try
            {
                server.Create(datas[key.wlno])
                    .ModelNo(datas[key.modelno])
                    .SealTime(datas[key.closedate])
                    .MaterialName(datas[key.wlname])
                    .MaterialClass(wlclass)
                    .CloseName(datas[key.closepeople])
                    .Supplier(datas[key.supp])
                    .ValidTime(validtime)
                    .Status(1)
                    .Commit();
                var result = new{Result = "成功"};
                return Json(result, JsonRequestBehavior.DenyGet); 
            }
            catch(Exception err)
            {
                var result = new{Result = err.Message};
                return Json(result, JsonRequestBehavior.DenyGet);
            }

        }
        // Excel导入
        [HttpPost]
        public JsonResult ImportExcel(HttpPostedFileBase file)
        {
            try
            {
                var hzs = file.FileName.Split(new[] { '.' });
                var hz = hzs[hzs.Length - 1];
                var filepath = Server.MapPath("~/File/导入的样件."+hz);  // 获得完整路径
                if (System.IO.File.Exists(filepath))
                    System.IO.File.Delete(filepath);
                file.SaveAs(filepath);
                server.ExcelImport(filepath);   // 导入
                var result = new{Result = "成功"};
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            catch (Exception err)
            {
                var result = new{ Result = err.Message};
                return Json(result, JsonRequestBehavior.DenyGet);
            }
        }
        // 查找
        public ActionResult Find(string fclosetime, string fwlno, string fwlname,
            string fwllclass, string fsup, string fver, string fvertime)
        {
            string command = string.Format("select * from Exemplar where SealedTime like '%{0}%' and Code like '%{1}%' and MaterialName like '%{2}%' and MaterialClass like '%{3}%' and Supplier like '%{4}%' and ValidTime like '%{5}%' and SignDate like '%{6}%' and Status = '1';", 
                                      fclosetime, fwlno, fwlname, fwllclass, fsup, fver, fvertime);
            var exemplars = server.Find2SQL(command);
            return View("ExepImport", exemplars);
        }
        // 修改
        [HttpPost]
        public JsonResult EditOne()
        {
            var str = new StreamReader(Request.InputStream);
            var stream = str.ReadToEnd();
            var datas = tools.String2Dict(stream);

            var wlclass = datas["wlclass2"];
            var validtime = "";
            if (datas.ContainsKey("validtime2"))
                validtime = datas["validtime2"];
            else
                validtime = datas["other2"];
            try
            {
                server.Edit(datas["dbid"], datas["wlno2"])
                    .SealTime(datas["closetime2"])
                    .MaterialName(datas["wlname2"])
                    .ModelNo(datas["modelno2"])
                    .MaterialClass(wlclass)
                    .CloseName(datas["closepeople2"])
                    .Supplier(datas["supp2"])
                    .ValidTime(validtime)
                    .Commit();
                var result = new {Result = "成功"};
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            catch (Exception err)
            {
                var result = new{ Result = err.Message};
                return Json(result, JsonRequestBehavior.DenyGet);
            }

        }
        // 删除
        public JsonResult Delete()
        {
            var stream = new StreamReader(Request.InputStream);
            var str = stream.ReadToEnd();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var code = js.Deserialize<string>(str);
            server.Delete(code);

            var results = new{Result = "成功"};
            return Json(results, JsonRequestBehavior.DenyGet);
        }
        // 撤销
        public void Back()
        {
            var stream = new StreamReader(Request.InputStream);
            var str = stream.ReadToEnd();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var codes = js.Deserialize<List<string>>(str);

            server.Back(codes[0], codes[1]);
        }
        // 样件提交到审核 
        public JsonResult Submit()
        {
            var stream = new StreamReader(Request.InputStream);
            var str = stream.ReadToEnd();
            JavaScriptSerializer js = new JavaScriptSerializer();
            var list = js.Deserialize<List<string>>(str);

            server.ChangeStatus(list, 2);
            var result = new{  Result = "成功" };
            return Json(result, JsonRequestBehavior.DenyGet);
        }
    }
}