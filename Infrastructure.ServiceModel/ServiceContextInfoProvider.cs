using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core.Infrastructure;
using Infrastructure.Core.TypeFinders;

namespace Infrastructure.ServiceModel
{
    public abstract class ServiceContextInfoProvider
    {
        public abstract ServiceContextInfo GetServiceContextInfo();

        public virtual object GetServiceContextInfo(string serializedData)
        {
            return null;
        }
    }
}
