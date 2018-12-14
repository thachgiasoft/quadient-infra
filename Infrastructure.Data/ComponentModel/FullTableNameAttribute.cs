using System;

namespace Infrastructure.Data.ComponentModel
{
    [AttributeUsage(AttributeTargets.Class )  ]
    public sealed class FullTableNameAttribute : Attribute 
    {
        public string FullTableName;
        public FullTableNameAttribute(string pFullTableName)
        {
            FullTableName  = pFullTableName ;
        }
    }
}
