using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LinqSpecs;

namespace ShenDeng.Domain
{
    public partial class Exemplar
    {
        public class By : Specification<Exemplar> // 物料编号
        {
            public readonly ExemplarIdentifier _id;
			public By(ExemplarIdentifier id)
            {
                _id = id;
            }
            public override Expression<Func<Exemplar, bool>> ToExpression()
            {
                return x => x.Id.Code == _id.Code;
            }
        }
        public class ByDBID : Specification<Exemplar>  // DBID
        {
            public readonly Guid _Dbid;
            public ByDBID(string Dbid)
            {
                _Dbid = new Guid(Dbid);
            }

            public override Expression<Func<Exemplar, bool>> ToExpression()
            {
                return x => x.DBID == _Dbid;
            }
        }
    }
}
