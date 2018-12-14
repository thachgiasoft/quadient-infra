using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ServiceModel.ServiceHosting.Attributes
{
    /// <summary>
    /// Veri değişikliklerinin loglanması istenen operasyonlara konulacak attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class AuditDataChanges : Attribute
    {
    }
}
