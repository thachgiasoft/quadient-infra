using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Exceptions.Enums
{
    [DataContract]
    public enum EnumExceptionType
    {
        [EnumMember]
        SystemException = 0,
        [EnumMember]
        ApplicationException = 1,
        [EnumMember]
        CanceledException = 2
    }
}
