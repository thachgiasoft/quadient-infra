using System;

namespace Infrastructure.JobScheduling
{
    [Serializable]
    public sealed class FunctionData
    {
        public string MethodName { get; set; }
        public string DeclaringTypeName { get; set; }
        public object[] Parameters { get; set; }
    }
}
