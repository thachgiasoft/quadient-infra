using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Core.Extensions
{
    public static class SerializationExtensions
    {
        public static ObjectIDGenerator ObjectIdGen = new ObjectIDGenerator();
        public static long GetObjectId(this object obj)
        {
            bool firstTime;
            return ObjectIdGen.GetId(obj, out firstTime);
        }
    }
}
