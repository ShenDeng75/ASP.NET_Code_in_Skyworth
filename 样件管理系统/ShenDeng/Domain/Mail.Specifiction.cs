using LinqSpecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ShenDeng.Domain
{
    public partial class Mail
    {
        public class By : Specification<Mail>
        {
            public readonly Guid _dbid;
            public By(string dbid)
            {
                _dbid = new Guid(dbid);
            }
            public override Expression<Func<Mail, bool>> ToExpression()
            {
                return x => x.DBID == _dbid;
            }
        }
    }
}
