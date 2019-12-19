using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace gbx.util
{
    public static class ReflectionExtensions
    {
        public static T CreateDelegate<T>(this MethodInfo methodInfo) where T : Delegate
        {
            return (T)methodInfo.CreateDelegate(typeof(T));
        }

        public static T CreateDelegate<T>(this MethodInfo methodInfo, object? target) where T : Delegate
        {
            return (T)methodInfo.CreateDelegate(typeof(T), target);
        }
    }
}
