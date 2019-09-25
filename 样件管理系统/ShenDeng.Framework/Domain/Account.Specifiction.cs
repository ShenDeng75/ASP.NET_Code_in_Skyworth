using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LinqSpecs;

namespace ShenDeng.Framework.Domain
{
    public partial class Account
    {
        // 规约模式
        public class By : Specification<Account>   // 通过用户名查找
        {
            public readonly AccountIdentifier _id;

            public By(AccountIdentifier id)
            {
                this._id = id;
            }
            public override Expression<Func<Account, bool>> ToExpression()
            {
                return x => x.Id.UserName == _id.UserName;
            }
        }
        public class ByJobNumber : Specification<Account> // 通过工号查找
        {
            public readonly string _jobnumber;
            public ByJobNumber(string jobnumber)
            {
                _jobnumber = jobnumber;
            }
            public override Expression<Func<Account, bool>> ToExpression()
            {
                return x => x.Job_Numner == _jobnumber;
            }
        }
    }
}
