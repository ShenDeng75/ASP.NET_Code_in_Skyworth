using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShenDeng.Framework.Base;

namespace ShenDeng.Domain
{
    public partial class Exemplar : Entity<Exemplar>
    {
        public Exemplar()
        {
        }
        public Exemplar(ExemplarIdentifier id)
        {
            Id = id;
        }
        /// <summary>
        /// 封样日期
        /// </summary>
        public virtual DateTime SealedTime { get; set; }
        /// <summary>
        /// 物料编号
        /// </summary>
        public virtual ExemplarIdentifier Id { get; set; }
        /// <summary>
        /// 模具编号
        /// </summary>
        public virtual string ModelNo { set; get; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public virtual string MaterialName { get; set; }
        /// <summary>
        /// 物料大类
        /// </summary>
        public virtual string MaterialClass { get; set; }
        /// <summary>
        /// 封样人
        /// </summary>
        public virtual string CloseName { get; set; }
        /// <summary>
        /// 供应商
        /// </summary>
        public virtual string Supplier { get; set; }
        /// <summary>
        /// 样件有效期
        /// </summary>
        public virtual string ValidTime { get; set; }
        /// <summary>
        /// 样件管理员
        /// </summary>
        public virtual string ExemManager { get; set; }
        /// <summary>
        /// 签收日期
        /// </summary>
        public virtual DateTime SignDate { get; set; }
        /// <summary>
        /// 审核人
        /// </summary>
        public virtual string Verifier { get; set; }
        /// <summary>
        /// 审核结果
        /// </summary>
        public virtual string VerResult { get; set; }
        /// <summary>
        /// 不良描述
        /// </summary>
        public virtual string NGDes { get; set; }
        /// <summary>
        /// 样件状态
        /// </summary>
        public virtual string ExemStatus { get; set; }
        /// <summary>
        /// 签收人
        /// </summary>
        public virtual string Signer { get; set; }
        /// <summary>
        /// 存放位置
        /// </summary>
        public virtual string SaveSpace { get; set; }
        /// <summary>
        /// 超期月数
        /// </summary>
        public virtual string LimitMonth { get; set; }
        /// <summary>
        /// 撤销理由
        /// </summary>
        public virtual string BackReason { get; set; }
        /// <summary>
        /// 总状态
        /// </summary>
        public virtual int Status { get; set; }

    }
}
