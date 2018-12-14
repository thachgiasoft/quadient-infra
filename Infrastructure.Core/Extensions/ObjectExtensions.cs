using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsNull<T>(this T obj) where T : class
        {
            return obj == null;
        }

        public static bool IsNull<T>(this T? obj) where T : struct
        {
            return !obj.HasValue;
        }

        public static bool IsNullOrDefault<T>(this object source) where T : struct
        {
            return source == null || source.Equals(default(T));
        }

        public static T DefaultIfNull<T>(this object source) where T : struct
        {
            if (source == null)
                return default(T);

            return source.ChangeType<T>();
        }

        public static bool IsNullOrDBNull(this object source)
        {
            return source == null || source == DBNull.Value;
        }

        public static string ToStringNullSafe(this object source)
        {
            return (source == null || source == DBNull.Value) ? string.Empty : source.ToString();
        }

       

        public static T ChangeType<T>(this object value, CultureInfo cultureInfo)
        {
            var toType = typeof(T);

            if (value == null) return default(T);

            if (value is string)
            {
                if (toType == typeof(Guid))
                {
                    return ChangeType<T>(new Guid(Convert.ToString(value, cultureInfo)), cultureInfo);
                }
                if ((string)value == string.Empty && toType != typeof(string))
                {
                    return ChangeType<T>(null, cultureInfo);
                }
            }
            else
            {
                if (typeof(T) == typeof(string))
                {
                    return ChangeType<T>(Convert.ToString(value, cultureInfo), cultureInfo);
                }
            }

            if (toType.IsGenericType &&
                toType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                toType = Nullable.GetUnderlyingType(toType); ;
            }

            bool canConvert = toType is IConvertible || (toType.IsValueType && !toType.IsEnum);
            if (canConvert)
            {
                return (T)Convert.ChangeType(value, toType, cultureInfo);
            }
            return (T)value;
        }

        public static T ChangeType<T>(this object value)
        {
            return ChangeType<T>(value, CultureInfo.CurrentCulture);
        }
    }
}
