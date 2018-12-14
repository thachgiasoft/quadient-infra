using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Infrastructure.JobScheduling
{
    public static class JobSchedulerHelper
    {
        public static FunctionData DeserializeFunctionData(string functionData)
        {
            return DeserializeFunctionData(Convert.FromBase64String(functionData));
        }

        public static FunctionData DeserializeFunctionData(byte[] functionData)
        {
            using (var stream = new MemoryStream(functionData))
            {
                var formatter = new BinaryFormatter();
                var result = (FunctionData)formatter.Deserialize(stream);
                return result;
            }
        }

        public static object DeserializeHeaderData(string headerData)
        {
            if (headerData == null) return null;

            return DeserializeHeaderData(Convert.FromBase64String(headerData));
        }

        public static object DeserializeHeaderData(byte[] headerData)
        {
            if (headerData == null) return null;

            using (var stream = new MemoryStream(headerData))
            {
                var formatter = new BinaryFormatter();
                var result = formatter.Deserialize(stream);
                return result;
            }
        }
    }
}
