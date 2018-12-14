using System;

namespace Infrastructure.Data.ComponentModel
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AutoIncColNameAttribute : Attribute 
    {
        public string AutoIncColName;
        public AutoIncColNameAttribute(string pAutoIncColName)
        {
            AutoIncColName = pAutoIncColName;
        }
    }
}
