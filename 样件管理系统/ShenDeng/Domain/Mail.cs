using ShenDeng.Framework.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShenDeng.Domain
{
    // 邮件地址
    public partial class Mail : Entity<Mail>
    {
        public Mail() { }
        /// <summary>
        /// 姓名
        /// </summary>
        public virtual string Name { set; get; }
        /// <summary>
        /// 邮箱地址
        /// </summary>
        public virtual string MailAddre { set; get; }
        /// <summary>
        /// 职责
        /// </summary>
        public virtual string Duty { set; get; }
        /// <summary>
        /// 密码(只有发送邮件配置)
        /// </summary>
        public virtual string PassWord { set; get; }
    }
}
