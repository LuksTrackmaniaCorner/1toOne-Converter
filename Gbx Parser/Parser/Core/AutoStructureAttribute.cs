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
        private static readonly MethodInfo _delegateHelperMethod;
        private static readonly Dictionary<Type, List<(string name, PropertyGetter getter)>> _autoStructureProperties;

        static AutoStructureAttribute()
        {
            _delegateHelperMethod = typeof(GbxStructure).GetMethod(nameof(MethodToDelegateHelper), BindingFlags.Static | BindingFlags.NonPublic)!;
            _autoStructureProperties = new Dictionary<Type, List<(string name, PropertyGetter getter)>>();
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

            foreach ((var name, var getter) in _autoStructureProperties[type])
            {
                var child = getter(structure);
                Debug.Assert(child != null);
                yield return (name, child);
            }
        }

        private static void GeneratePropertyGetters(Type type)
        {
            lock (_autoStructureProperties)
            {
                if (!_autoStructureProperties.ContainsKey(type))
                {
                    //Generate the property order for this class
                    var properties = from property in type.GetProperties()
                                     let autoStructureAttribute = property.GetCustomAttribute<AutoStructureAttribute>()
                                     where autoStructureAttribute != null //filter properties with no AutoStructureAttribute
                                     orderby autoStructureAttribute.Order
                                     select property;

                    //Getting the correct generic variant of the MethodToDelegate() Helper Method.
                    var delegateHelper = _delegateHelperMethod.MakeGenericMethod(type)
                        .CreateDelegate<Func<MethodInfo, PropertyGetter>>();

                    //Generate delegates for all the property getters
                    var propertyGetters = new List<(string name, PropertyGetter getter)>();
                    foreach (var property in properties)
                    {
                        var methodInfo = property.GetGetMethod()!;
                        var getter = delegateHelper(methodInfo);
                        propertyGetters.Add((property.Name, getter));
                    }

                    //Storing the results
                    _autoStructureProperties.Add(type, propertyGetters);
                }
            }
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
        private static PropertyGetter MethodToDelegateHelper<T>(MethodInfo methodInfo) where T : GbxStructure
        {
            var del = methodInfo.CreateDelegate<Func<T, GbxComponent>>();

            //Create a weakly typed delegate which calls the strongly typed delegate
            return (target) => del((T)target);
        }

        private delegate GbxComponent PropertyGetter(GbxStructure target);
    }
}
