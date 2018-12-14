using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Infrastructure.Core.Helpers
{
    public static class SerializationHelper
    {
        public static string XmlSerializeToString(this object objectInstance)
        {
            var serializer = new XmlSerializer(objectInstance.GetType());
            var sb = new StringBuilder();

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, objectInstance);
            }

            return sb.ToString();
        }

        public static T XmlDeserializeFromString<T>(this string objectData)
        {
            return (T)XmlDeserializeFromString(objectData, typeof(T));
        }

        public static object XmlDeserializeFromString(this string objectData, Type type)
        {
            var serializer = new XmlSerializer(type);
            object result;

            using (var reader = new StringReader(objectData))
            {
                result = serializer.Deserialize(reader);
            }

            return result;
        }

        public static bool TryResolveParameters<TService>(Expression<Action<TService>> operation, out object[] parameters)
        {
            parameters = null;

            var lambdaExpression = operation as LambdaExpression;
            if (lambdaExpression == null)
                return false;

            var methodCall = lambdaExpression.Body as MethodCallExpression;
            if (methodCall == null)
                return false;

            parameters = new object[methodCall.Arguments.Count];
            var methodParameters = methodCall.Method.GetParameters();

            for (int i = 0; i < parameters.Length; i++)
            {
                if (methodParameters[i].ParameterType.IsByRef)
                {
                    FieldInfo field;
                    object target;
                    MemberExpression me;

                    if (operation.NodeType == ExpressionType.MemberAccess
                        && (field = (me = (MemberExpression)(Expression)operation).Member as FieldInfo) != null)
                    {
                        if (!TryEvaluate(methodCall.Arguments[i], out target))
                        {
                            target = Expression.Lambda(methodCall.Arguments[i]).Compile().DynamicInvoke();
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (!TryEvaluate(methodCall.Arguments[i], out parameters[i]))
                    {
                        parameters[i] = Expression.Lambda(methodCall.Arguments[i]).Compile().DynamicInvoke();
                    }
                }
            }
            return true;
        }

        private static bool TryEvaluate(Expression operation, out object value)
        {
            if (operation == null)
            {
                value = null;
                return true;
            }

            switch (operation.NodeType)
            {
                case ExpressionType.Constant:
                    value = ((ConstantExpression)operation).Value;
                    return true;
                case ExpressionType.MemberAccess:
                    MemberExpression me = (MemberExpression)operation;
                    object target;
                    if (TryEvaluate(me.Expression, out target))
                    {
                        switch (me.Member.MemberType)
                        {
                            case MemberTypes.Field:
                                value = ((FieldInfo)me.Member).GetValue(target);
                                return true;
                            case MemberTypes.Property:
                                value = ((PropertyInfo)me.Member).GetValue(target, null);
                                return true;
                        }
                    }
                    break;
            }
            value = null;
            return false;
        }
    }


    /// <summary>
    /// Extension methods for the dynamic object.
    /// </summary>
    public static class DynamicHelper
    {
        /// <summary>
        /// Defines the simple types that is directly writeable to XML.
        /// </summary>
        private static readonly Type[] _writeTypes = new[] { typeof(string), typeof(DateTime), typeof(Enum), typeof(decimal), typeof(Guid) };

        /// <summary>
        /// Determines whether [is simple type] [the specified type].
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        /// 	<c>true</c> if [is simple type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSimpleType(this Type type)
        {
            return type.IsPrimitive || _writeTypes.Contains(type);
        }

        /// <summary>
        /// Converts the specified dynamic object to XML.
        /// </summary>
        /// <param name="dynamicObject">The dynamic object.</param>
        /// <returns>Returns an Xml representation of the dynamic object.</returns>
        public static XElement ConvertToXml(dynamic dynamicObject)
        {
            return ConvertToXml(dynamicObject, null);
        }

        /// <summary>
        /// Converts the specified dynamic object to XML.
        /// </summary>
        /// <param name="dynamicObject">The dynamic object.</param>
        /// /// <param name="element">The element name.</param>
        /// <returns>Returns an Xml representation of the dynamic object.</returns>
        public static XElement ConvertToXml(dynamic dynamicObject, string element)
        {
            if (String.IsNullOrWhiteSpace(element))
            {
                element = "object";
            }

            element = XmlConvert.EncodeName(element);
            var ret = new XElement(element);

            Dictionary<string, object> members = new Dictionary<string, object>(dynamicObject);

            var elements = from prop in members
                           let name = XmlConvert.EncodeName(prop.Key)
                           let val = prop.Value.GetType().IsArray ? "array" : prop.Value
                           let value = prop.Value.GetType().IsArray ? GetArrayElement(prop.Key, (Array)prop.Value) : (prop.Value.GetType().IsSimpleType() ? new XElement(name, val) : val.ToXml(name))
                           where value != null
                           select value;

            ret.Add(elements);

            return ret;
        }

        /// <summary>
        /// Generates an XML string from the dynamic object.
        /// </summary>
        /// <param name="dynamicObject">The dynamic object.</param>
        /// <returns>Returns an XML string.</returns>
        public static string ToXmlString(dynamic dynamicObject)
        {
            XElement xml = DynamicHelper.ConvertToXml(dynamicObject);

            return xml.ToString();
        }

        /// <summary>
        /// Converts an anonymous type to an XElement.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Returns the object as it's XML representation in an XElement.</returns>
        public static XElement ToXml(this object input)
        {
            return input.ToXml(null);
        }

        /// <summary>
        /// Converts an anonymous type to an XElement.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="element">The element name.</param>
        /// <returns>Returns the object as it's XML representation in an XElement.</returns>
        public static XElement ToXml(this object input, string element)
        {
            if (input == null)
            {
                return null;
            }

            if (String.IsNullOrWhiteSpace(element))
            {
                element = "object";
            }

            element = XmlConvert.EncodeName(element);
            var ret = new XElement(element);

            if (input != null)
            {
                var type = input.GetType();
                var props = type.GetProperties();

                var elements = from prop in props
                               let name = XmlConvert.EncodeName(prop.Name)
                               let val = prop.PropertyType.IsArray ? "array" : prop.GetValue(input, null)
                               let value = prop.PropertyType.IsArray ? GetArrayElement(prop, (Array)prop.GetValue(input, null)) : (prop.PropertyType.IsSimpleType() ? new XElement(name, val) : val.ToXml(name))
                               where value != null
                               select value;

                ret.Add(elements);
            }

            return ret;
        }

        /// <summary>
        /// Parses the specified XML string to a dynamic.
        /// </summary>
        /// <param name="xmlString">The XML string.</param>
        /// <returns>A dynamic object.</returns>
        public static dynamic ParseDynamic(this string xmlString)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the array element.
        /// </summary>
        /// <param name="info">The property info.</param>
        /// <param name="input">The input object.</param>
        /// <returns>Returns an XElement with the array collection as child elements.</returns>
        private static XElement GetArrayElement(PropertyInfo info, Array input)
        {
            return GetArrayElement(info.Name, input);
        }

        /// <summary>
        /// Gets the array element.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="input">The input object.</param>
        /// <returns>Returns an XElement with the array collection as child elements.</returns>
        private static XElement GetArrayElement(string propertyName, Array input)
        {
            var name = XmlConvert.EncodeName(propertyName);

            XElement rootElement = new XElement(name);

            var arrayCount = input.GetLength(0);

            for (int i = 0; i < arrayCount; i++)
            {
                var val = input.GetValue(i);
                XElement childElement = val.GetType().IsSimpleType() ? new XElement(name + "Child", val) : val.ToXml();

                rootElement.Add(childElement);
            }

            return rootElement;
        }
    }
}
