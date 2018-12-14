using System;

namespace Infrastructure.Data.ComponentModel
{
    [AttributeUsage (AttributeTargets.Class)]
    public sealed class DefaultDatabaseAttribute : Attribute 
    {
        public string mDatabaseName;
        public DefaultDatabaseAttribute(string pDatabaseName)
        {
            mDatabaseName = pDatabaseName;
        }
    }
}
