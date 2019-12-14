using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace gbx.parser.core
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AutoStructureAttribute : Attribute
    {
        public int Order { get; private set; }

        public AutoStructureAttribute([CallerLineNumber]int order = 0)
        {
            Order = order;
        }
    }

    public static class AutoStructureExtensions
    {
        private static readonly MethodInfo _delegateHelperMethod;
        private static readonly Dictionary<Type, List<(string name, PropertyGetter<GbxStructure> getter)>> _autoStructureProperties;

        static AutoStructureExtensions()
        {
            _delegateHelperMethod = typeof(AutoStructureExtensions).GetMethod(nameof(MethodToDelegateHelper), BindingFlags.Static | BindingFlags.NonPublic)!;
            _autoStructureProperties = new Dictionary<Type, List<(string name, PropertyGetter<GbxStructure> getter)>>();
        }

        public static IEnumerable<(string name, GbxComponent child)> AutoGenerateChildren(this GbxStructure gbxStructure)
        {
            var type = gbxStructure.GetType();

            if (!_autoStructureProperties.ContainsKey(type))
            {
                GeneratePropertyGetters(type);
            }

            foreach ((var name, var getter) in _autoStructureProperties[type])
            {
                var child = getter(gbxStructure);
                Debug.Assert(child != null);
                //TODO deal with child == null;
                yield return (name, child);
            }
        }

        private static void GeneratePropertyGetters(Type type)
        {
            //TODO better locking mechanism
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

                    //To build the fitting generic delegates
                    var delegateHelper = (Func<MethodInfo, PropertyGetter<GbxStructure>>)_delegateHelperMethod.MakeGenericMethod(type).CreateDelegate(typeof(Func<MethodInfo, PropertyGetter<GbxStructure>>));

                    //Generate delegates to property getters
                    var propertyGetters = new List<(string name, PropertyGetter<GbxStructure> getter)>();
                    foreach (var property in properties)
                    {
                        var methodInfo = property.GetGetMethod()!;
                        var test = methodInfo.GetParameters();
                        var getter = delegateHelper(methodInfo);
                        propertyGetters.Add((property.Name, getter));
                    }

                    _autoStructureProperties.Add(type, propertyGetters);
                }
            }
        }

        private static PropertyGetter<GbxStructure> MethodToDelegateHelper<T>(MethodInfo methodInfo) where T : GbxStructure
        {
            var del = (PropertyGetter<T>)methodInfo.CreateDelegate(typeof(PropertyGetter<T>));

            //Create a more weakly typed delegate which calls the strongly typed delegate
            return (target) => del((T)target);
        }

        private delegate GbxComponent PropertyGetter<T>(T target) where T : GbxStructure;
    }
}
