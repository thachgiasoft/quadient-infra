using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Infrastructure.Core.Extensions
{
    public static class ReflectionExtensions
    {
        public static string GetPropertyName<T, TProp>(this T o, Expression<Func<T, TProp>> propertySelector)
        {
            MemberExpression body = (MemberExpression)propertySelector.Body;
            return body.Member.Name;
        }

        public static TValue GetAttributeValue<TAttribute, TValue>(this Type type, Func<TAttribute, TValue> valueSelector) where TAttribute : Attribute
        {
            var att = type.GetCustomAttributes(
                typeof(TAttribute), true
            ).FirstOrDefault() as TAttribute;
            if (att != null)
            {
                return valueSelector(att);
            }
            return default(TValue);
        }

        public static TValue GetAttributeValue<TAttribute, TValue>(this MethodInfo metod, Func<TAttribute, TValue> valueSelector) where TAttribute : Attribute
        {
            var att = metod.GetCustomAttributes(
                typeof(TAttribute), true
            ).FirstOrDefault() as TAttribute;
            if (att != null)
            {
                return valueSelector(att);
            }
            return default(TValue);
        }
    }
}
