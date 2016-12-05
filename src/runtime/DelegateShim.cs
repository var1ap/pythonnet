using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Python.Runtime
{
    public static class DelegateShim
    {
        public static Delegate CreateDelegate(Type dtype, object obj, string methodName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var methods = obj.GetType().GetTypeInfo().GetMember(methodName, BindingFlags.Default);
            if (!methods.Any())
            {
                throw new InvalidOperationException("Method does not exist");
            }

            return CreateDelegate(dtype, (MethodInfo)methods[0]);
        }

        internal static Delegate CreateDelegate(Type dtype, MethodInfo method)
        {
            return method.CreateDelegate(dtype);
        }
    }
}
