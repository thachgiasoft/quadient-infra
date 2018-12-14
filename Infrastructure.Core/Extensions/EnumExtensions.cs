using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core.ComponentModelOperations;

namespace Infrastructure.Core.Extensions
{
   public static class EnumExtensions
    {
        public static string GetEnumDescription(this Enum pEnumItem)
        {
            return EnumOperations.GetEnumDescription(pEnumItem);
        }

        public static T ToEnum<T>(this string name)
        {
            return EnumOperations.ToEnum<T>(name);
        }

        public static T GetValue<T>(this Enum pEnumItem)
        {
            return EnumOperations.GetValue<T>(pEnumItem);
        }

      

        public static T ToEnum<T>(this object source, T ifInvalid = default(T))
        {
            var enumString = source.ToStringNullSafe();

            if (Enum.IsDefined(typeof(T), enumString))
                return (T)Enum.Parse(typeof(T), enumString);

            int num;
            if (int.TryParse(enumString, out num))
            {
                if (Enum.IsDefined(typeof(T), num))
                    return (T)Enum.ToObject(typeof(T), num);
            }

            return ifInvalid;
        }

        public static Nullable<T> ToEnum<T>(this object source) where T : struct
        {
            var enumString = source.ToStringNullSafe();

            if (Enum.IsDefined(typeof(T), enumString))
                return (T)Enum.Parse(typeof(T), enumString);

            int num;
            if (int.TryParse(enumString, out num))
            {
                if (Enum.IsDefined(typeof(T), num))
                    return (T)Enum.ToObject(typeof(T), num);
            }

            return null;
        }
    }
}
