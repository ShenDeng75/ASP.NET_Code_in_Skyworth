using ShenDeng.Domain;
using ShenDeng.Framework.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShenDeng.Map
{
    public class MailMap : BaseClassMap<Mail>
    {
        public MailMap()
        {
            Map(x => x.Name);
            Map(x => x.MailAddre);
            Map(x => x.Duty);
            Map(x => x.PassWord);
        }
    }
}
