using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ServiceModel
{
    [ServiceContract(Namespace = "http://Infrastructure.ServiceModel/")]
    public interface IService
    {
        [OperationContract]
        string Ping();
    }
}
