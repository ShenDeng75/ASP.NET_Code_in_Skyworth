using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using ShenDeng.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Mail;
using ShenDeng.application;

namespace ShenDeng
{
    public static class Tools
    {
        // 根据字符串形式的时间，返回天数
        public static int FormatDays(string time)
        {
            int days = 0;
            string dw = time[time.Length - 1].ToString();
            var t = int.Parse(time.Substring(0, time.Length - 1));
            if (dw == "月")
                days = t * 30;
            else if (dw == "年")
                days = t * 365;
            return days;
        }
        // 写入数据到Excel
        public static string Output2Excel(List<Exemplar> exemplars, string path)
        {
            //创建工作薄
            HSSFWorkbook wk = new HSSFWorkbook();
            //创建一个名称为mySheet的表
            ISheet tb = wk.CreateSheet("样件");
            TableHead(tb); // 设置表头
            int i = 1;
            foreach (var exe in exemplars)   // 添加数据
            {
                IRow r = tb.CreateRow(i);    // 第几行
                r.CreateCell(0).SetCellValue(exe.SealedTime.ToString().Split(new[] { ' ' })[0]);
                r.CreateCell(1).SetCellValue(exe.Id.Code);
                r.CreateCell(2).SetCellValue(exe.ModelNo);
                r.CreateCell(3).SetCellValue(exe.MaterialName);
                r.CreateCell(4).SetCellValue(exe.MaterialClass);
                r.CreateCell(5).SetCellValue(exe.CloseName);
                r.CreateCell(6).SetCellValue(exe.Supplier);
                r.CreateCell(7).SetCellValue(exe.ValidTime);
                r.CreateCell(8).SetCellValue(exe.ExemManager);
                r.CreateCell(9).SetCellValue(exe.SignDate.ToString().Split(new[] { ' ' })[0]);
                r.CreateCell(10).SetCellValue(exe.Verifier);
                r.CreateCell(11).SetCellValue(exe.VerResult);
                r.CreateCell(12).SetCellValue(exe.NGDes);
                r.CreateCell(13).SetCellValue(exe.ExemStatus);
                r.CreateCell(14).SetCellValue(exe.Signer);
                r.CreateCell(15).SetCellValue(exe.SaveSpace);
                r.CreateCell(16).SetCellValue(exe.LimitMonth);
                r.CreateCell(17).SetCellValue(exe.BackReason);
                i++;
            }
            using (FileStream fs = File.OpenWrite(path)) //打开一个xls文件，如果没有则自行创建，如果存在myxls.xls文件则在创建是不要打开该文件！
            {
                wk.Write(fs);   //向打开的这个xls文件中写入mySheet表并保存。
            }
            string[] filenames = path.Split(new[] { '/', '\\' });
            string filename = "/File/" + filenames[filenames.Length - 1];
            return filename;
        }
        // 设置表头
        public static void TableHead(ISheet tb)
        {
            List<string> heads = new List<string> { "封样日期", "物料编号", "模穴号", "物料名称", "物料大类", "封样人",
                        "供应商", "样件有效期", "样件管理员", "签收日期", "审核人", "审核结果", "不良描述", "样件状态",
                        "签收人", "存放位置", "超期月数", "备注"};
            IRow r0 = tb.CreateRow(0);   // 第一行
            int i = 0;
            foreach (var h in heads)
            {
                ICell cell = r0.CreateCell(i); // 列
                cell.SetCellValue(h);
                i++;
            }
        }
        // 序列化
        public static string getJsonByObject(Exemplar exe)
        {
            //实例化DataContractJsonSerializer对象，需要待序列化的对象类型
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(exe.GetType());
            //实例化一个内存流，用于存放序列化后的数据
            MemoryStream stream = new MemoryStream();
            //使用WriteObject序列化对象
            serializer.WriteObject(stream, exe);
            //写入内存流中
            byte[] dataBytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(dataBytes, 0, (int)stream.Length);
            //通过UTF8格式转换为字符串
            string result = Encoding.UTF8.GetString(dataBytes);
            // 将Json字符串转为Json数据
            JObject jobj = (JObject)JsonConvert.DeserializeObject(result);
            return result;
        }
        // 发送 NG 邮件
        public static void SendMail(Exemplar exe, string emailaddre, string password, MailServer mailServer)
        {
            SmtpClient client = new SmtpClient("maila.skyworth.com", 25);  // ***SMTP协议

            client.DeliveryMethod = SmtpDeliveryMethod.Network;//指定电子邮件发送方式
            client.UseDefaultCredentials = true;
            string username = emailaddre.Split(new[] { '@' })[0];
            client.Credentials = new System.Net.NetworkCredential(username, password);//用户名、密码

            MailMessage msg = new MailMessage();   // ***邮件
            msg.From = new MailAddress(emailaddre, "样件管理系统");   //发件人地址
            msg.Subject = "样件审核结果 NG 通知";    //邮件标题
            msg.Body = SendContent(exe);    //邮件内容
            msg.BodyEncoding = Encoding.UTF8;    //邮件内容编码
            msg.IsBodyHtml = true;   //是否是HTML邮件
            msg.Priority = MailPriority.Normal;    //邮件优先级
            Tos(mailServer, exe.MaterialClass, msg);   // 接收者

            client.Send(msg);     // 发送
        }
        // 添加接收邮箱
        public static void Tos(MailServer mailServer, string mateclass, MailMessage msg)
        {
            var duty = "";
            if (mateclass[0] == '数')
                duty = "数字";
            else if (mateclass[0] == '无')
                duty = "无线";
            else
                duty = "新世界";
            var yf = mailServer.Find2Duty(duty);
            var iqc = mailServer.Find2Duty("IQC");
            foreach (var i in yf)
                msg.To.Add(i.MailAddre);
            foreach (var i in iqc)
                msg.To.Add(i.MailAddre);
        }
        // 发送内容
        public static string SendContent(Exemplar exe)
        {
            string now = DateTime.Now.ToLocalTime().ToString("yyyy/MM/dd");
            string result = "";
            result += "<div><h3>您好！以下是 " + now + " 来自IQC的检验结果</h3>";
            result += "物料编号：<strong>" + exe.Id.Code + "</strong><br />";
            result += "模穴号：<strong>" + exe.ModelNo + "</strong><br />";
            result += "物料大类：<strong>" + exe.MaterialClass + "</strong><br />";
            result += "供应商：<strong>" + exe.Supplier + "</strong><br />";
            result += "不良描述：<strong>" + exe.NGDes + "</strong><br />";
            result += "签收日期：<strong>" + exe.SignDate.ToString("yyyy/MM/dd") + "</strong><br /></div>";
            return result;
        }
    }
}
