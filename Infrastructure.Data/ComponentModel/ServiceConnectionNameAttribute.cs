using System;

namespace Infrastructure.Data.ComponentModel
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ServiceConnectionNameAttribute : Attribute
    {
        public string mConnectionName;
        public ServiceConnectionNameAttribute(string pConnectionName)
        {
            mConnectionName = pConnectionName;
        }
    }
}
