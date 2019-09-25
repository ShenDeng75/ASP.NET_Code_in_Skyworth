using ShenDeng.Domain;
using ShenDeng.Framework.Base;
using ShenDeng.Framework.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShenDeng.Framework;
using System.Text.RegularExpressions;

namespace ShenDeng.application
{
    public class ExemplarCommand
    {
        public IRepository repository;
        public Exemplar exemplar;
        public IMySessionService sessionService;
        public ExemplarCommand(Exemplar exemplar, IRepository repository)
        {
            this.repository = repository;
            this.exemplar = exemplar;
            this.sessionService = UnityIoC.Get<IMySessionService>();
        }
        // 封样日期
        public ExemplarCommand SealTime(string time)
        {
            exemplar.SealedTime = DateTime.Parse(time);
            return this;
        }
        // 模具号
        public ExemplarCommand ModelNo(string modelno)
        {
            exemplar.ModelNo = modelno;
            return this;
        }
        // 物料名称
        public ExemplarCommand MaterialName(string materialName)
        {
            exemplar.MaterialName = materialName;
            return this;
        }
        // 物料大类
        public ExemplarCommand MaterialClass(string mclass)
        {
            exemplar.MaterialClass = mclass;
            return this;
        }
        // 封样人
        public ExemplarCommand CloseName(string cname)
        {
            exemplar.CloseName = cname;
            return this;
        }
        // 供应商
        public ExemplarCommand Supplier(string sup)
        {
            exemplar.Supplier = sup;
            return this;
        }
        // 样件有效期
        public ExemplarCommand ValidTime(string vtime)
        {
            string patt = @"^\d+[月, 年]$";
            if (!Regex.IsMatch(vtime.Trim(), patt))
                throw new Exception("\"样件有效期\"格式不正确！");
            exemplar.ValidTime = vtime;
            return this;
        }
        // 样件管理员
        public ExemplarCommand ExempManage(string exemanage)
        {
            exemplar.ExemManager = exemanage;
            return this;
        }
        // 签收日期
        public ExemplarCommand SignTime(string signtime)
        {
            exemplar.SignDate = DateTime.Parse(signtime);
            return this;
        }
        // 审核人
        public ExemplarCommand Verfiler(string verfiler)
        {
            exemplar.Verifier = verfiler;
            return this;
        }
        // 审核结果
        public ExemplarCommand Verresult(string verresult)
        {
            exemplar.VerResult = verresult;
            return this;
        }
        // 不良描述
        public ExemplarCommand NGDes(string ngdes)
        {
            exemplar.NGDes = ngdes;
            return this;
        }
        // 签收人
        public ExemplarCommand Signer(string signer)
        {
            exemplar.Signer = signer;
            return this;
        }
        // 存放位置
        public ExemplarCommand SaveSpace(string savespace)
        {
            exemplar.SaveSpace = savespace;
            return this;
        }
        // 备注
        public ExemplarCommand BackReason(string backreason)
        {
            exemplar.BackReason = backreason;
            return this;
        }
        // 设置超期
        public ExemplarCommand OverTime(string time, string validtime)
        {
            int days = Tools.FormatDays(validtime);
            var endtime = DateTime.Parse(time).AddDays(days);
            var isover = endtime < DateTime.Now.ToLocalTime();  // 是否超期
            if (isover)
            {
                exemplar.ExemStatus = "超期";
                exemplar.LimitMonth = ((int)(DateTime.Now.ToLocalTime() - endtime).TotalDays / 30).ToString();
            }
            else
            {
                exemplar.ExemStatus = "有效期内";
                exemplar.LimitMonth = "0";
            }
            return this;
        }
        // 总状态
        public ExemplarCommand Status(int status)
        {
            exemplar.Status = status;
            return this;
        }
        //提交事务
        public void Commit()
        {
            Auto();
            var trancation = repository.session.Transaction;
            trancation.Commit();  //提交本次数据库操作。
        }
        // 完善自动赋值的字段
        public ExemplarCommand Auto()
        {
            exemplar.ExemManager = sessionService.GetAccount().Id.UserName;
            exemplar.SignDate = DateTime.Now.ToLocalTime();
            return this;
        }
        // 提交多个事务
        public void Commits()
        {
            var trancation = repository.session.Transaction;
            trancation.Commit();  //提交本次数据库操作。
        }
    }
}
