using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure.Core.Extensions
{
    public static class TypeValidationExtensions
    {
        public static bool IsNumeric(this object pDeger)
        {
            //TODO pDeger is Int16 || pDeger is Int32 || pDeger is Int64 diger primitive type lar icin unit test yazilacak
            return Microsoft.VisualBasic.Information.IsNumeric(pDeger);
        }

        public static bool IsInteger(this object pDeger)
        {
            //TODO pDeger is Int16 || pDeger is Int32 || pDeger is Int64 diger primitive type lar icin unit test yazilacak
            Int64 num1;
            bool res = Int64.TryParse(pDeger.ToString(), out num1);
            if (res == false)
            {
                return false;
            }
            return true;
        }

        public static bool IsDate(this object pDeger)
        {
            //TODO pDeger is Int16 || pDeger is Int32 || pDeger is Int64 diger primitive type lar icin unit test yazilacak
            return Microsoft.VisualBasic.Information.IsDate(pDeger);
        }

        private static Regex isGuid = new Regex(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$", RegexOptions.Compiled);
        public static bool IsGuid(this string pDeger)
        {
            if (pDeger != null)
            {
                if (isGuid.IsMatch(pDeger))
                {
                    return true;
                }
            }

            return false;
        }

        private static Regex isBase64 = new Regex(@"^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?$", RegexOptions.Compiled);
        public static bool IsBase64(this string pDeger)
        {
            if (pDeger != null)
            {
                if (isBase64.IsMatch(pDeger))
                {
                    return true;
                }
            }

            return false;
        }

        private static Regex isPasswordStrong = new Regex(@"(?=.{8,})(?=.*[a-zA-Z])(?=.*[\d])(?=.*[\W])", RegexOptions.Compiled);
        public static bool IsPasswordStrong(this string pDeger)
        {
            // Gönderilen şifre içinde en az 1 rakam, en az 1 harf, en az 1 özel karakter varsa ve uzunluğu en az 8 karakter is geriye true döner
            if (pDeger != null)
            {
                if (isPasswordStrong.IsMatch(pDeger))
                {
                    return true;
                }
            }

            return false;
        }

        public static T GetDefaultValue<T>(this T pDeger)
        {
            return default(T);
        }

        public static bool IsDefault<T>(this T pDeger)
        {
            return EqualityComparer<T>.Default.Equals(pDeger, default(T));
        }

        public static bool IsDefault<T>(this T pDeger, Type pType)
        {
            if (pDeger != null &&
                    ((pType == typeof(DateTime) && Convert.ToDateTime(pDeger) == DateTime.MinValue)
                            || (pType == typeof(int) && Convert.ToInt32(pDeger) == default(int))
                            || (pType == typeof(uint) && Convert.ToUInt32(pDeger) == default(uint))
                            || (pType == typeof(long) && Convert.ToInt64(pDeger) == default(long))
                            || (pType == typeof(ulong) && Convert.ToUInt64(pDeger) == default(ulong))
                            || (pType == typeof(decimal) && Convert.ToDecimal(pDeger) == default(decimal))
                            || (pType == typeof(double) && Convert.ToDouble(pDeger) == default(double))
                            || (pType == typeof(byte) && Convert.ToByte(pDeger) == default(byte))
                            || (pType == typeof(short) && Convert.ToInt16(pDeger) == default(short))
                            || (pType == typeof(ushort) && Convert.ToUInt16(pDeger) == default(ushort))
                            || (pType == typeof(float) && Convert.ToSingle(pDeger) == default(float))
                            || (pType == typeof(string) && string.IsNullOrEmpty(pDeger.ToString()))
                            || (pType == typeof(Guid) && Guid.Parse(pDeger.ToString()) == new Guid())
                    ))
                return true;

            return false;
        }

        /// <summary>
        /// NOTE: For non-nullable value types, this method introduces a boxing/unboxing performance penalty. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static T GetDefault<T>(T val)
        {
            return (T)GetDefault(typeof(T));
        }

        /// <summary> 
        /// [ <c>public static bool IsObjectSetToDefault(this Type ObjectType, object ObjectValue)</c> ] 
        /// <para></para> 
        /// Reports whether a value of type T (or a null reference of type T) contains the default value for that Type 
        /// </summary> 
        /// <remarks> 
        /// Reports whether the object is empty or unitialized for a reference type or nullable value type (i.e. is null) or  
        /// whether the object contains a default value for a non-nullable value type (i.e. int = 0, bool = false, etc.) 
        /// <para></para> 
        /// NOTE: For non-nullable value types, this method introduces a boxing/unboxing performance penalty. 
        /// </remarks> 
        /// <param name="ObjectType">Type of the object to test</param> 
        /// <param name="ObjectValue">The object value to test, or null for a reference Type or nullable value Type</param> 
        /// <returns> 
        /// true = The object contains the default value for its Type. 
        /// <para></para> 
        /// false = The object has been changed from its default value. 
        /// </returns> 
        public static bool IsObjectSetToDefault(this Type ObjectType, object ObjectValue)
        {
            if (ObjectType == null)
            {
                if (ObjectValue == null)
                {
                    MethodBase currmethod = MethodInfo.GetCurrentMethod();
                    string ExceptionMsgPrefix = currmethod.DeclaringType + " {" + currmethod + "} Error:\n\n";
                    throw new ArgumentNullException(ExceptionMsgPrefix + "Cannot determine the ObjectType from a null Value");
                }

                ObjectType = ObjectValue.GetType();
            }

            object Default = GetDefault(ObjectType);

            if (ObjectValue != null)
                return ObjectValue.Equals(Default);

            return Default == null;
        }

        /// <summary>
        /// NOTE: For non-nullable value types, this method introduces a boxing/unboxing performance penalty. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static object GetDefault(Type type)
        {
            if (type == null || !type.IsValueType || type == typeof(void))
                return null;

            if (type.ContainsGenericParameters)
                throw new ArgumentException("{" + MethodInfo.GetCurrentMethod() + "} Error:\n\nThe supplied value type <" + type +
                        "> contains generic parameters, so the default value cannot be retrieved");

            if (type.IsPrimitive || !type.IsNotPublic)
            {
                try
                {
                    return Activator.CreateInstance(type);
                }
                catch (Exception e)
                {
                    throw new ArgumentException("{" + MethodInfo.GetCurrentMethod() + "} Error:\n\nThe Activator.CreateInstance method could not "
                            + "create a default instance of the supplied value type <" + type
                            + "> (Inner Exception message: \"" + e.Message + "\")"
                            , e
                            );
                }
            }

            throw new ArgumentException("{" + MethodInfo.GetCurrentMethod() + "} Error:\n\nThe supplied value type <"
                    + type + "> is not a publicly-visible type, so the default value cannot be retrieved");
        }
    }
}
