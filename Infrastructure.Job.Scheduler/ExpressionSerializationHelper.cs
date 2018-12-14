using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Core.Helpers;

namespace Infrastructure.JobScheduling
{
   public static class ExpressionSerializationHelper
    {
        public static byte[] SerializeExpression<TService>(Expression<Action<TService>> callAction)
        {
            object[] parameters;
            if (!SerializationHelper.TryResolveParameters(callAction, out parameters))
                throw new InvalidOperationException("Expression cannot be serialized");

            var mi = ((MethodCallExpression)callAction.Body).Method;
            var methodName = mi.Name;
            var declaringTypeName = mi.DeclaringType.AssemblyQualifiedName;

            var functionData = new FunctionData()
            {
                MethodName = methodName,
                DeclaringTypeName = declaringTypeName,
                Parameters = parameters
            };

            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, functionData);
                return stream.ToArray();
            }
        }

        public static byte[] SerializeExpression(MethodInfo mi, object[] parametervalues)
        {
            var methodName = mi.Name;
            var declaringTypeName = mi.DeclaringType.AssemblyQualifiedName;
            var functionData = new FunctionData()
            {
                MethodName = methodName,
                DeclaringTypeName = declaringTypeName,
                Parameters = parametervalues
            };

            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, functionData);
                return stream.ToArray();
            }
        }
    }
}
