using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShenDeng.Framework.Base;
using ShenDeng.Domain;

namespace ShenDeng.Map
{
    public class ExemplarMap : BaseClassMap<Exemplar>
    {
        public ExemplarMap()
        {
            Map(x => x.SealedTime);
            Component(x => x.Id, y => y.Map(m => m.Code));
            Map(x => x.ModelNo);
            Map(x => x.MaterialName);
            Map(x => x.MaterialClass);
            Map(x => x.CloseName);
            Map(x => x.Supplier);
            Map(x => x.ValidTime);
            Map(x => x.ExemManager);
            Map(x => x.SignDate);
            Map(x => x.Verifier);
            Map(x => x.VerResult);
            Map(x => x.NGDes);
            Map(x => x.ExemStatus);
            Map(x => x.Signer);
            Map(x => x.SaveSpace);
            Map(x => x.LimitMonth);
            Map(x => x.BackReason);
            Map(x => x.Status);
        }
    }
}
