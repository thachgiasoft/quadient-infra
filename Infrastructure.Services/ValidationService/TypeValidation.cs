using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Infrastructure.Core.Extensions;

namespace Infrastructure.Services.ValidationService
{
    public class TypeValidation : ITypeValidation
    {
        public bool IsNumeric(object pDeger)
        {
            return pDeger.IsNumeric();
        }

        public bool IsInteger(object pDeger)
        {
            return pDeger.IsInteger();
        }

        public bool IsDate(object pDeger)
        {
            return pDeger.IsDate();
        }

        public bool IsGuid(string pDeger)
        {
            return pDeger.IsGuid();
        }

        public bool IsBase64(string pDeger)
        {
            return pDeger.IsBase64();
        }

        public bool IsPasswordStrong(string pDeger)
        {
            return pDeger.IsPasswordStrong();
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
        public bool IsObjectSetToDefault(Type ObjectType, object ObjectValue)
        {
            return ObjectType.IsObjectSetToDefault(ObjectValue);
        }

    }
}
