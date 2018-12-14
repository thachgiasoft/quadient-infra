using System;

namespace Infrastructure.Data.ComponentModel
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ShortNameAttribute : Attribute
    {
        public string ShortName;
        public ShortNameAttribute(string pShortName)
        {
            ShortName = pShortName;
        }
    }
}
