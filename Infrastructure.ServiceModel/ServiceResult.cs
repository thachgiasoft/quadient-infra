using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ServiceModel
{
    [Serializable]
    public class ServiceResult<T>
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        public T Result { get; set; }
    }
}
