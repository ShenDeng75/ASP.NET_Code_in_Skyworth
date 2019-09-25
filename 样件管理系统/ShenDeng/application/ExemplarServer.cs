using ShenDeng.Domain;
using ShenDeng.Framework.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShenDeng.Framework.Tools;
using ShenDeng.Framework.Base;
using ShenDeng.Framework.Domain;
using ShenDeng.Framework;
using ShenDeng;
using System.Text.RegularExpressions;

namespace ShenDeng.application
{
    public class ExemplarServer
    {
        public IRepository repository;
        public IMySessionService sessionService;
        public MailServer mailServer;
        public ExemplarServer(IRepository repository)
        {
            this.repository = repository;
            this.sessionService = UnityIoC.Get<IMySessionService>();
            this.mailServer = new MailServer();
        }
        // 添加
        public ExemplarCommand Create(string wlno)
        {
            Exemplar exemplar = new Exemplar(ExemplarIdentifier.of(wlno));
            repository.Save(exemplar);
            return new ExemplarCommand(exemplar, repository);
        }
        // 初始数据Excel批量导入
        public void ExcelImport(string filepath)
        {
             DataTable data = tool.Excel2DataTable(filepath);
             for (int i = 1; i < data.Rows.Count; i++)
             {
                string validtime = data.Rows[i][7].ToString().Trim();
                string patt = @"^\d+[月,年]$";
                if (!Regex.IsMatch(validtime.Trim(), patt))
                    throw new Exception("\"样件有效期\" 格式不正确！");
                try
                {
                    DateTime time = DateTime.Parse(data.Rows[i][0].ToString().Trim());
                }
                catch
                {
                    throw new Exception("\"封样日期\" 格式不正确！");
                }
             }
            try
            {
                for (int i = 1; i < data.Rows.Count; i++)
                {
                    var row = data.Rows[i];
                    var wlno = row[1].ToString();
                    Create(wlno)
                        .SealTime(row[0].ToString().Trim())
                        .ModelNo(row[2].ToString().Trim())
                        .MaterialName(row[3].ToString().Trim())
                        .MaterialClass(row[4].ToString().Trim())
                        .CloseName(row[5].ToString().Trim())
                        .Supplier(row[6].ToString().Trim())
                        .ValidTime(row[7].ToString().Trim())
                        .Status(1)
                        .Auto();
                }
                Commit();
            }
            catch(Exception err)
            {
                RollBack();
                throw new Exception(err.Message);
            }
        }
        // 台账数据Excel导入
        public void LedgerImport(string filepath)
        {
            DataTable data = tool.Excel2DataTable(filepath);
            for (int i = 1; i < data.Rows.Count; i++)
            {
                string validtime = data.Rows[i][7].ToString().Trim();
                string patt = @"^\d+[月,年]$";
                if (!Regex.IsMatch(validtime.Trim(), patt))
                    throw new Exception("\"样件有效期\" 格式不正确！");
                try
                {
                    DateTime time = DateTime.Parse(data.Rows[i][0].ToString().Trim());
                    DateTime time2 = DateTime.Parse(data.Rows[i][9].ToString().Trim());
                }
                catch
                {
                    throw new Exception("\"封样日期或签收日期\" 格式不正确！");
                }
            }
            try
            {
                for (int i = 1; i < data.Rows.Count; i++)
                {
                    var row = data.Rows[i];
                    var wlno = row[1].ToString().Trim();
                    Create(wlno)
                        .SealTime(row[0].ToString().Trim())
                        .ModelNo(row[2].ToString().Trim())
                        .MaterialName(row[3].ToString().Trim())
                        .MaterialClass(row[4].ToString().Trim())
                        .CloseName(row[5].ToString().Trim())
                        .Supplier(row[6].ToString().Trim())
                        .ValidTime(row[7].ToString().Trim())
                        .ExempManage(row[8].ToString().Trim())
                        .SignTime(row[9].ToString().Trim())
                        .Verfiler(row[10].ToString().Trim())
                        .Verresult(row[11].ToString().Trim())
                        .NGDes(row[12].ToString().Trim())
                        .Signer(row[13].ToString().Trim())
                        .SaveSpace(row[14].ToString().Trim())
                        .BackReason(row[15].ToString().Trim())
                        .OverTime(row[0].ToString().Trim(), row[7].ToString().Trim())
                        .Status(4);
                }
                Commit();
            }
            catch (Exception err)
            {
                RollBack();
                throw new Exception(err.Message);
            }
        }
        // 修改
        public ExemplarCommand Edit(string dbid, string newno)
        {
            Exemplar exemplar = GetOne(dbid);
            exemplar.Id = ExemplarIdentifier.of(newno);
            repository.Save(exemplar);
            return new ExemplarCommand(exemplar, repository);
        }
        // 查找所有
        public IEnumerable<Exemplar> GetAll()
        {
            return repository.FindAll<Exemplar>();
        }
        // 查找单个
        public Exemplar GetOne(string dbid)
        {
            return repository.FindOne<Exemplar>(new Exemplar.ByDBID(dbid));
        }
        // 通过SQL查找
        public List<Exemplar> Find2SQL(string command)
        {
            List<Exemplar> exemplar = repository.Find2SQL<Exemplar>(command);
            return exemplar;
        }
        // 删除
        public void Delete(string id)
        {
            var exemp = GetOne(id);
            repository.Delete<Exemplar>(exemp);
        }
        // 撤销
        public void Back(string dbid, string back)
        {
            var exemp = GetOne(dbid);
            exemp.Status = 0;
            exemp.BackReason = back;
            Commit();
        }
        // 提交审核
        public string SubmitCheck(Dictionary<string, string> dict)
        {
            Account account = sessionService.GetAccount();
            string err = "成功";
            foreach(var d in dict)
            {
                Exemplar exemplar = GetOne(d.Key);
                if (d.Value == "") // OK
                {
                    exemplar.Status = 3;
                    exemplar.Verifier = account.Id.UserName;
                    exemplar.VerResult = "OK";
                    exemplar.BackReason = "";
                }
                else  // NG
                {
                    exemplar.Status = 1;
                    exemplar.NGDes = d.Value;
                    exemplar.Verifier = account.Id.UserName;
                    exemplar.VerResult = "NG";
                    exemplar.BackReason = "";
                    try
                    {
                        var sender = mailServer.FindSender();
                        Tools.SendMail(exemplar, sender.MailAddre, sender.PassWord, mailServer);  // 发送邮件
                    }
                    catch(Exception e)
                    {
                        err = e.Message;
                    }
                }
            }
            Commit();
            return err;
        }
        // 提交保存
        public void SubmitSave(Dictionary<string, string> dict)
        {
            Account account = sessionService.GetAccount();
            foreach(var d in dict)
            {
                Exemplar exemplar = GetOne(d.Key);
                exemplar.Signer = account.Id.UserName;
                var space_back = d.Value.Split(new[] { '&' });
                exemplar.SaveSpace = space_back[0];
                exemplar.BackReason = space_back[1]; // 备注
                exemplar.Status = 4;
            }
            Commit();
        }
        // 退回
        public void BackSave(string dbid, string backrea)
        {
            var exemplar = GetOne(dbid);
            exemplar.BackReason = backrea;
            exemplar.Status = 2;
            Commit();
        }
        // 提交报废
        public void OverH(List<string> dbids)
        {
            foreach (var n in dbids)
            {
                var exe = repository.FindOne<Exemplar>(new Exemplar.ByDBID(n));
                exe.Status = 5;
                exe.ExemStatus = "报废";
                exe.SaveSpace = "";
                repository.Save(exe);
            }
            Commit();
        }
        // 导出到Excel
        public string Output(List<string> dbids, string path)
        {
            List<Exemplar> exemplars = new List<Exemplar>();
            foreach(var d in dbids)
            {
                var exe = GetOne(d);
                exemplars.Add(exe);
            }
            path += sessionService.GetAccount().Id.UserName + "_导出样件.xlsx";
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
            string filepath = Tools.Output2Excel(exemplars, path);
            return filepath;
        }
        // 改变状态
        public void ChangeStatus(List<string> nos, int status)
        {
            foreach(var n in nos)
            {
                var exe = repository.FindOne<Exemplar>(new Exemplar.ByDBID(n));
                exe.Status = status;
                exe.BackReason = "";
                repository.Save(exe);
            }
            Commit();
        }
        // 验证样件的状态
        public bool VerStatus(List<string> dbid, int status)
        {
            foreach (var d in dbid)
            {
                var exemplar = GetOne(d);
                if (exemplar.Status != status)
                    return false;
            }
            return true;
        }
        // 提交本次数据库操作
        public void Commit()
        {
            var trancation = repository.session.Transaction;
            trancation.Commit();  //提交本次数据库操作。
        }
        // 回滚
        public void RollBack()
        {
            var trancation = repository.session.Transaction;
            trancation.Rollback();   // 取消本次数据库操作
        }
    }
}
