using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dependee
{
    /// <summary>
    /// Mark this property as dependent on the given property name
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=true)]
    public sealed class Dependency : System.Attribute
    {
        public readonly string PreRequisite;

        public Dependency(string preReq)
        {
            this.PreRequisite = preReq;
        }
    }
}
