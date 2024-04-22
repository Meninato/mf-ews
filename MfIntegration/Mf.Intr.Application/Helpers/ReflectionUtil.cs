using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Intr.Application.Helpers;

public static class ReflectionUtil
{
    public static IEnumerable<MethodInfo> GetMethods(Type type)
    {
        IEnumerable<MethodInfo> methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        if (type.BaseType != null)
        {
            methods = methods.Concat(GetMethods(type.BaseType));
        }

        return methods;
    }
}
