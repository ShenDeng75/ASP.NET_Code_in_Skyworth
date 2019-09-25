using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShenDeng.Framework.Domain
{
    [Flags]
    public enum Role
    {
        /// <summary>
        /// 样件台账(查询)
        /// </summary>
        Find = 1,
        /// <summary>
        /// 样件存储
        /// </summary>
        Save = 2,
        /// <summary>
        /// 样件审核
        /// </summary>
        Check = 4,
        /// <summary>
        /// 超期处理
        /// </summary>
        OverHandl = 8,
        /// <summary>
        /// 管理员
        /// </summary>
        Admin = 16,
        /// <summary>
        /// 所有权限
        /// </summary>
        All = 31
    }
}
