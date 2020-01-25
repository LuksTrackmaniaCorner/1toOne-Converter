using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Gbx.Util;

namespace Gbx.Parser.Core
{
    /// <summary>
    /// Used to signal properties of a structure, which can be generated automatically..
    /// Also make sure that the property is public, or it will now be used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AutoStructureAttribute : Attribute
    {
        private static readonly MethodInfo _propertyGetterHelper;
        private static readonly Dictionary<Type, PropertyGetters> _autoStructureProperties;

        static AutoStructureAttribute()
        {
            _propertyGetterHelper = typeof(AutoStructureAttribute).GetMethod(nameof(BuildPropertyGetters), BindingFlags.Static | BindingFlags.NonPublic)!;
            _autoStructureProperties = new Dictionary<Type, PropertyGetters>();
        }

        public int Order { get; }

        public AutoStructureAttribute([CallerLineNumber]int order = 0)
        {
            Order = order;
        }

        public static IEnumerable<(string name, GbxComponent child)> AutoGetNamedChildren(GbxStructure structure)
        {
            var type = structure.GetType();

            if (!_autoStructureProperties.ContainsKey(type))
            {
                GeneratePropertyGetters(type);
            }

            return _autoStructureProperties[type](structure);
        }

        private static void GeneratePropertyGetters(Type type)
        {
            //I don't think this method needs to be synchronized.
            //It should be thread-safe anyway because this method is idempotent.
            //Worst case, the property getters would be generated multiple times
            //if multiple threads enter this method in the same time.
            //Should be an acceptable risk though, and a good locking mechanism would be difficult.

            //Generate the property order for this class
            var properties = from property in type.GetProperties()
                                let autoStructureAttribute = property.GetCustomAttribute<AutoStructureAttribute>()
                                where autoStructureAttribute != null //filter properties with no AutoStructureAttribute
                                orderby autoStructureAttribute.Order
                                select property;

            //Getting the correct generic variant of the MethodToDelegate() Helper Method.
            var propertyGetterHelper = _propertyGetterHelper.MakeGenericMethod(type)
                .CreateDelegate<Func<IEnumerable<PropertyInfo>, PropertyGetters>>();

            //Storing the results
            _autoStructureProperties[type] = propertyGetterHelper(properties);
        }

        /// <summary>
        /// This method allows to avoid calling the getter of a autoproperty through reflection.
        /// It does so by creating a strongly typed delegate of the getter, and makes a more weakly typed delegate out
        /// of it, so that this delegate-getter can then be cached in te autoStructureProperties Dictionary.
        /// 
        /// This means that once the delegate is in the cache, it runs much faster because we don't have to call
        /// the property getter through reflection, but only have to check if the upcast works.
        /// </summary>
        /// <typeparam name="T">The type that has the property, a subclass of GbxStructure</typeparam>
        /// <param name="methodInfo">The propertygetter as methodinfo</param>
        /// <returns>The propertygetter as weakly typed delegate</returns>
        private static PropertyGetters BuildPropertyGetters<T>(IEnumerable<PropertyInfo> propertyInfos) where T : GbxStructure
        {
            var childrenGetters = new List<(string name, PropertyGetter<T> childGetter)>();

            foreach(var propertyInfo in propertyInfos)
            {
                var getter = propertyInfo.GetGetMethod()!.CreateDelegate<PropertyGetter<T>>();
                childrenGetters.Add((propertyInfo.Name, getter));
            }

            return GetChildren;

            IEnumerable<(string name, GbxComponent child)> GetChildren(GbxComponent weakTarget)
            {
                var strongTarget = (T)weakTarget;

                foreach(var (name, childGetter) in childrenGetters)
                {
                    yield return (name, childGetter(strongTarget));
                }
            }
        }

        private delegate GbxComponent PropertyGetter<T>(T strongTarget) where T : GbxStructure;

        private delegate IEnumerable<(string name, GbxComponent child)> PropertyGetters(GbxStructure weakTarget);
    }

    public static class AutoStructureExtensions
    {
        public static IEnumerable<(string name, GbxComponent child)> AutoGetNamedChildren(this GbxStructure structure)
        {
            return AutoStructureAttribute.AutoGetNamedChildren(structure);
        }
    }
}
