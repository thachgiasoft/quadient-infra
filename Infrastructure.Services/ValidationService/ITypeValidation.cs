using System;
using System.ServiceModel;

namespace Infrastructure.Services.ValidationService
{
    [ServiceContract]
    public interface ITypeValidation
    {
        [OperationContract]
        bool IsNumeric(object pDeger);
        [OperationContract]
        bool IsInteger(object pDeger);
        [OperationContract]
        bool IsDate(object pDeger);
        [OperationContract]
        bool IsGuid(string pDeger);
        [OperationContract]
        bool IsBase64(string pDeger);
        [OperationContract]
        bool IsPasswordStrong(string pDeger);
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
        [OperationContract]
        bool IsObjectSetToDefault(Type ObjectType, object ObjectValue);
    }
}