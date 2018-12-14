using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Infrastructure.Core.ComponentModel;
using Infrastructure.Exceptions.TypeExceptions;

namespace Infrastructure.Core.Helpers
{
    public class GenericTypeHelper
    {
        /// <summary>
        /// Sets a property on an object to a valuae.
        /// </summary>
        /// <param name="instance">The object whose property to set.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="value">The value to set the property to.</param>
        public static void SetProperty(object instance, string propertyName, object value)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            Type instanceType = instance.GetType();
            PropertyInfo pi = instanceType.GetProperty(propertyName);
            if (pi == null)
                throw new PropertyException("No property '{0}' found on the instance of type '{1}'.", propertyName, instanceType);
            if (!pi.CanWrite)
                throw new PropertyException("The property '{0}' on the instance of type '{1}' does not have a setter.", propertyName, instanceType);
            if (value != null && !value.GetType().IsAssignableFrom(pi.PropertyType))
                value = To(value, pi.PropertyType);
            pi.SetValue(instance, value, new object[0]);
        }

        public static TypeConverter GetCoreCustomTypeConverter(Type type)
        {
            //we can't use the following code in order to register our custom type descriptors
            //TypeDescriptor.AddAttributes(typeof(List<int>), new TypeConverterAttribute(typeof(GenericListTypeConverter<int>)));
            //so we do it manually here

            if (type == typeof(List<int>))
                return new GenericListTypeConverter<int>();
            if (type == typeof(List<decimal>))
                return new GenericListTypeConverter<decimal>();
            if (type == typeof(List<string>))
                return new GenericListTypeConverter<string>();
            return TypeDescriptor.GetConverter(type);
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <returns>The converted value.</returns>
        public static object To(object value, Type destinationType)
        {
            return To(value, destinationType, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <param name="culture">Culture</param>
        /// <returns>The converted value.</returns>
        public static object To(object value, Type destinationType, CultureInfo culture)
        {
            if (value != null)
            {
                var sourceType = value.GetType();

                var destinationConverter = GetCoreCustomTypeConverter(destinationType);
                var sourceConverter = GetCoreCustomTypeConverter(sourceType);
                if (destinationConverter != null && destinationConverter.CanConvertFrom(value.GetType()))
                    // ReSharper disable AssignNullToNotNullAttribute
                    return destinationConverter.ConvertFrom(context: null, culture: culture, value: value);
                // ReSharper restore AssignNullToNotNullAttribute
                if (sourceConverter != null && sourceConverter.CanConvertTo(destinationType))
                    return sourceConverter.ConvertTo(null, culture, value, destinationType);
                if (destinationType.IsEnum && value is int)
                    return Enum.ToObject(destinationType, (int)value);
                if (!destinationType.IsInstanceOfType(value))
                    return System.Convert.ChangeType(value, destinationType, culture);
            }
            return value;
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <returns>The converted value.</returns>
        public static T To<T>(object value)
        {
            //return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            return (T)To(value, typeof(T));
        }

        /// <summary>
        /// Convert enum for front-end
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Converted string</returns>
        public static string ConvertEnum(string str)
        {
            var result = string.Empty;
            var letters = str.ToCharArray();
            foreach (var c in letters)
                if (c.ToString(CultureInfo.InvariantCulture) != c.ToString(CultureInfo.InvariantCulture).ToLower())
                    result += " " + c;
                else
                    result += c.ToString(CultureInfo.InvariantCulture);
            return result;
        }
        /// <summary>
        ///   Gets properties from type
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetProperties<T>()
        {
            return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite);
        }

        /// <summary>
        ///   Determines whether the specified property name is excluded.
        /// </summary>
        /// <param name = "propertyName">Name of the property.</param>
        /// <param name = "exclusions">The exclusions.</param>
        /// <returns>
        ///   <c>true</c> if the specified property name is excluded; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsExclusion(string propertyName, Exclusions exclusions)
        {
            //check for wildcard
            if (exclusions.Any(s => s.Contains("*")))
            {
                foreach (var exclusion in exclusions)
                {
                    var ex = exclusion.Replace("*", ".*");
                    var regex = new Regex(ex);
                    if (regex.IsMatch(propertyName))
                    {
                        return true;
                    }
                }
            }
            else
            {
                return exclusions.Contains(propertyName);
            }
            return false;
        }

        /// <summary>
        ///   Gets the source property
        /// </summary>
        public static PropertyInfo GetSourceProperty(Aliases aliases, PropertyInfo targetProperty,
                                                       Exclusions exclusions, Type sourceType,
                                                       PropertyInfo sourceProperty)
        {
            if (aliases != null && aliases.Keys.Contains(targetProperty.Name))
            {
                //Excluded properties should not be mapped... even if aliased
                if (!(exclusions != null && ContainsExclusion(aliases[targetProperty.Name], exclusions)))
                    sourceProperty = sourceType.GetProperty(aliases[targetProperty.Name]);
            }
            else
            {
                // If target property is already aliased then do not match on source property of the same name... this will cause a double mapping
                if (aliases == null || (!aliases.Values.Contains(targetProperty.Name)))
                {
                    //do not map excluded properties
                    if (!(exclusions != null && ContainsExclusion(targetProperty.Name, exclusions)))
                    {
                        sourceProperty = sourceType.GetProperty(targetProperty.Name);
                    }
                }
            }
            return sourceProperty;
        }

        public static string GetPropertyNameFromExpression<T>(Expression<Func<T, object>> exp)
        {
            string name = "";

            MemberExpression body = exp.Body as MemberExpression;
            if (body == null)
            {
                UnaryExpression ubody = (UnaryExpression)exp.Body;
                body = ubody.Operand as MemberExpression;
                name = body.Member.Name;
            }
            else
            {
                name = body.Member.Name;
            }

            return name;
        }

        //kontrol et
        static readonly ConcurrentDictionary<object, object> ConverterCache = new ConcurrentDictionary<object, object>();


        public static object Convert<TSource, TTarget>(TSource value, Func<IConvertable<TSource, TTarget>> converterFactory)
        {
            object converter = null;
            ConverterCache.TryGetValue(typeof(TTarget), out converter);
            if (converter == null)
                ConverterCache.TryAdd(typeof(TTarget), converter = converterFactory());

            return ((IConvertable<TSource, TTarget>)converter).Convert(value);
        }

    }
    public class Aliases : Dictionary<string, string>
    {

    }


    /// <summary>
    ///   A list of source object's property names that should be bypassed when mapping
    /// </summary>
    public class Exclusions : List<string>
    {

    }

    /// <summary>
    ///   Data transformation rules based on the source object's property name
    /// </summary>
    public class PropertyNameTriggers : Dictionary<string, Func<object, object>>
    {

    }

    /// <summary>
    ///   A set of rules based on the source object's property types
    /// </summary>
    public class DataTypeTriggers : Dictionary<Type, Func<object, object>>
    {
    }
    #region Nested type: NoValue

    public class NoValue
    {
    }



    #endregion



    public interface IConvertable<in TSource, out TTarget> : IConvertable
    {
        TTarget Convert(TSource value);
    }
    public interface IConvertable
    {
    }

    public class GenericConverter<T> : IConvertable<Object, T>, IDisposable where T : class
    {
        public Guid TypeId { get; set; }

        public GenericConverter()
        {
            TypeId = Guid.NewGuid();
        }

        public T Convert(object value)
        {
            if (value == null)
                return default(T);
            if (value is T)
            {
                //EventLog.WriteEntry("Application", "GenericConverter Value is T ikinci if bloguna girdi" + typeof(T),  EventLogEntryType.Information, 9112);
                return (T)value;
            }

            T instance = Activator.CreateInstance<T>();
            if (instance is IEnumerable)
            {
                //EventLog.WriteEntry("Application", "GenericConverter Value is T ucuncu if bloguna girdi. Ozellestirilmis Convert" + typeof(T), EventLogEntryType.Information, 9113);
                Type generic = typeof(T);
                var itemType = generic.GetGenericArguments()[0];
                if (value.GetType() == typeof(object[]))
                {
                    //object[] to List<T> using Linq
                    //instance
                    //var list = ((object[])value).SelectMany(System.Convert.ChangeType(i, itemType)).ToList();
                    var addMethod = instance.GetType().GetMethod("Add");
                    // instance = ((object[])value).Select(i => System.Convert.ChangeType(i, itemType)).ToList();
                    //var ty = System.Convert.ChangeType(((object[])value).First(), itemType);
                    var list = ((object[])value);//.Select(i => System.Convert.ChangeType(i, itemType)).ToList();
                    //  var dene = list as T;
                    // instance.Add(ty);
                    var enumerator = list.GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        addMethod.Invoke(instance, new object[] { enumerator.Current });
                    }
                    //list.Clear();
                    list = null;
                }
            }
            else
                instance = (T)value;
            return instance;

        }

        public void Dispose()
        {

        }
        ~GenericConverter()
        {

        }
    }

}
