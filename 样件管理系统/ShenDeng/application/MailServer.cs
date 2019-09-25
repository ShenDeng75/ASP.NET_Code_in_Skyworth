using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShenDeng.Domain;
using ShenDeng.Framework.DataBase;
using ShenDeng.Framework;

namespace ShenDeng.application
{
    public class MailServer
    {
        public IRepository repository;
        public MailServer()
        {
            this.repository = UnityIoC.Get<IRepository>();
        }
        // 查找所有
        public IEnumerable<Mail> GetAll()
        {
            return repository.FindAll<Mail>();
        }
        // 查找发送邮箱
        public Mail FindSender()
        {
            var mails = GetAll();
            var mail = from m in mails
                       where m.Duty == "发送邮箱"
                       select m;
            if (mail.Count() != 1)
                throw new Exception("发送邮箱异常！");
            return mail.Single();
        }
        // 根据职责查找
        public List<Mail> Find2Duty(string duty)
        {
            var _mails = GetAll();
            var mails = from m in _mails
                       where m.Duty == duty
                       select m;
            return mails.ToList();
        }
        // 添加接收邮箱
        public void CreateMail(string name, string addre, string duty, string pwd)
        {
            Mail mail = new Mail();
            mail.Name = name;
            mail.MailAddre = addre;
            mail.Duty = duty;
            mail.PassWord = pwd;
            repository.Save(mail);
            Commit();
        }
        // 删除 IQC 邮箱
        public void DeleteMail(string dbid)
        {
            var mail = repository.FindOne<Mail>(new Mail.By(dbid));
            repository.Delete<Mail>(mail);
        }
        public void Commit()
        {
            var tran = repository.session.Transaction;
            tran.Commit();
        }
    }
}
