using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShenDeng.Domain
{
    public struct ExemplarIdentifier
    {
        public string Code { get; }

        public ExemplarIdentifier(string code)
        {
            this.Code = code;
        }

        public static ExemplarIdentifier of(string code)
        {
            return new ExemplarIdentifier(code);
        }

        public override string ToString()
        {
            return string.Format("Exemplar/{0}", Code);
        }
        //隐士转换
        public static implicit operator string(ExemplarIdentifier identifier)
        {
            return identifier.ToString();
        }

        public static implicit operator ExemplarIdentifier(string identifier)
        {
            var sub = identifier.Split(new[] { '/' }, 2);
            return of(sub[1]);
        }
    }
}
