using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;
using XPCF;

#pragma warning disable IDE1006 // Styles d'affectation de noms
namespace SolAR
{
    public static class Extensions
    {
        public static bool CanRead(this IProperty.AccessSpecifier specifier) { return specifier != IProperty.AccessSpecifier.IProperty_OUT; }
        public static bool CanWrite(this IProperty.AccessSpecifier specifier) { return specifier != IProperty.AccessSpecifier.IProperty_IN; }

        public static object Get(this IProperty property, uint itemIndex = 0)
        {
            switch (property.getType())
            {
                case IProperty.PropertyType.IProperty_CHARSTR:
                    return property.getStringValue(itemIndex);
                case IProperty.PropertyType.IProperty_DOUBLE:
                    return property.getDoubleValue(itemIndex);
                case IProperty.PropertyType.IProperty_FLOAT:
                    return property.getFloatingValue(itemIndex);
                case IProperty.PropertyType.IProperty_INTEGER:
                    return property.getIntegerValue(itemIndex);
                case IProperty.PropertyType.IProperty_LONG:
                    return property.getLongValue(itemIndex);
                case IProperty.PropertyType.IProperty_STRUCTURE:
                    return property.getStructureValue(itemIndex);
                case IProperty.PropertyType.IProperty_UINTEGER:
                    return property.getUnsignedIntegerValue(itemIndex);
                case IProperty.PropertyType.IProperty_ULONG:
                    return property.getUnsignedLongValue(itemIndex);
                case IProperty.PropertyType.IProperty_UNICODESTR:
                    return property.getUnicodeStringValue(itemIndex);
                default:
                    throw new System.Exception();
            }
        }

        public static void Set(this IProperty property, object value, uint itemIndex = 0)
        {
            switch (property.getType())
            {
                case IProperty.PropertyType.IProperty_CHARSTR:
                    property.setStringValue((string)value, itemIndex);
                    break;
                case IProperty.PropertyType.IProperty_DOUBLE:
                    property.setDoubleValue((double)value, itemIndex);
                    break;
                case IProperty.PropertyType.IProperty_FLOAT:
                    property.setFloatingValue((float)value, itemIndex);
                    break;
                case IProperty.PropertyType.IProperty_INTEGER:
                    property.setIntegerValue((int)value, itemIndex);
                    break;
                case IProperty.PropertyType.IProperty_LONG:
                    property.setLongValue((long)value, itemIndex);
                    break;
                case IProperty.PropertyType.IProperty_STRUCTURE:
                    property.setStructureValue((IPropertyMap)value, itemIndex);
                    break;
                case IProperty.PropertyType.IProperty_UINTEGER:
                    property.setUnsignedIntegerValue((uint)value, itemIndex);
                    break;
                case IProperty.PropertyType.IProperty_ULONG:
                    property.setUnsignedLongValue((ulong)value, itemIndex);
                    break;
                case IProperty.PropertyType.IProperty_UNICODESTR:
                    property.setUnicodeStringValue((string)value, itemIndex);
                    break;
                default:
                    throw new System.Exception();
            }
        }

        public static object Default(this IProperty.PropertyType propertyType)
        {
            switch (propertyType)
            {
                case IProperty.PropertyType.IProperty_CHARSTR:
                case IProperty.PropertyType.IProperty_UNICODESTR:
                    return default(string);
                case IProperty.PropertyType.IProperty_DOUBLE:
                    return default(double);
                case IProperty.PropertyType.IProperty_FLOAT:
                    return default(float);
                case IProperty.PropertyType.IProperty_INTEGER:
                    return default(int);
                case IProperty.PropertyType.IProperty_LONG:
                    return default(long);
                case IProperty.PropertyType.IProperty_STRUCTURE:
                    return default(IPropertyMap);
                case IProperty.PropertyType.IProperty_UINTEGER:
                    return default(uint);
                case IProperty.PropertyType.IProperty_ULONG:
                    return default(ulong);
                default:
                    throw new System.Exception();
            }
        }

        public static object OnGUI(this IProperty.PropertyType propertyType, object value)
        {
            switch (propertyType)
            {
                case IProperty.PropertyType.IProperty_CHARSTR:
                case IProperty.PropertyType.IProperty_UNICODESTR:
                    return GUILayout.TextField((string)value);
                    /*
                case IProperty.PropertyType.IProperty_DOUBLE:
                    GUILayout.TextField(value.ToString());
                    return value;
                case IProperty.PropertyType.IProperty_FLOAT:
                    GUILayout.TextField(value.ToString());
                    return value;
                case IProperty.PropertyType.IProperty_INTEGER:
                    return default(int);
                case IProperty.PropertyType.IProperty_LONG:
                    return default(long);
                case IProperty.PropertyType.IProperty_STRUCTURE:
                    return default(IPropertyMap);
                case IProperty.PropertyType.IProperty_UINTEGER:
                    return default(uint);
                case IProperty.PropertyType.IProperty_ULONG:
                    return default(ulong);
                    */
                default:
                    GUILayout.TextField(value.ToString());
                    return value;
            }
        }

        public static T AddTo<T>(this T disposable, IList<IDisposable> list) where T : IDisposable
        {
            list.Add(disposable);
            return disposable;
        }

        public static string GetUUID(string name)
        {
            string res = null;
            bool ok = false;
            ok = ok || modulesDict.TryGetValue(name, out res);
            ok = ok || interfacesDict.TryGetValue(name, out res);
            ok = ok || componentsDict.TryGetValue(name, out res);
            if (!ok) throw new System.Exception(name);
            return res;
        }

        public static Dictionary<string, string> modulesDict;
        public static Dictionary<string, string> interfacesDict;
        public static Dictionary<string, string> componentsDict;

        [Obsolete("Use type parameter")]
        public static IComponentIntrospect create<T>(this IComponentManager xpcfComponentManager)
        {
            return xpcfComponentManager.createComponent(GET_UUID<T>());
        }

        public static IComponentIntrospect create(this IComponentManager xpcfComponentManager, string type)
        {
            var uuid = UUID.Create(GetUUID(type));
            return xpcfComponentManager.createComponent(uuid);
        }

        public static IComponentIntrospect create(this IComponentManager xpcfComponentManager, string type, string name)
        {
            var uuid = UUID.Create(GetUUID(type));
            return xpcfComponentManager.createComponent(name, uuid);
        }

        public static UUID GET_UUID<T>()
        {
            var name = typeof(T).Name;
            return UUID.Create(GetUUID(name));
        }

        /*
        public static T bindTo<T>(this IComponentIntrospect component) where T : class
        {
            var name = typeof(T).Name;
            var type = typeof(IComponentIntrospect);
            var bindTo = type.GetMethod("bindTo_" + name, BindingFlags.Public | BindingFlags.Instance);
            Assert.IsNotNull(bindTo);
            return bindTo.Invoke(component, null) as T;
        }
        */

        public static T bindTo<T>(this IComponentIntrospect component) where T : class
        {
            return (T) component.bindTo(typeof(T).Name);
        }

        public static object bindTo(this IComponentIntrospect component, string name)
        {
            var type = typeof(solar);
            var method = type.GetMethod("bindTo_" + name, BindingFlags.Public | BindingFlags.Static);
            Assert.IsNotNull(method);
            return method.Invoke(null, new[] { component });
        }

        /*
        public static T bindTo<T>(this IComponentIntrospect component) where T : class
        {
            try
            {
                var type = typeof(IComponentIntrospect);
                var cptr = IComponentIntrospect.getCPtr(component);
                FieldInfo swigCMemOwnBase = type.GetField("swigCMemOwnBase", BindingFlags.NonPublic | BindingFlags.Instance);
                Assert.IsNotNull(swigCMemOwnBase);
                //var cMemoryOwn = (bool) swigCMemOwnBase.GetValue(component);
                swigCMemOwnBase.SetValue(component, false);

                return (T)Activator.CreateInstance
                (
                    typeof(T),
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new object[] { cptr.Handle, false },
                    null
                );
            } catch (System.Exception e)
            {
                UnityEngine.Debug.Log(e);
                return null;
            }
        }
        */

        /*
        [Obsolete]
        static IComponentIntrospect bindTo(this IComponentIntrospect component, string name)
        {
            var uuid = UUID.Create(GetUUID(name));
            Assert.IsTrue(component.implements(uuid));
            return component.queryInterface(uuid);
        }
        */

        [Obsolete]
        public static T CastTo<T>(IComponentIntrospect from, bool cMemoryOwn = false)
        {
            var type = typeof(IComponentIntrospect);
            MethodInfo getCPtr = type.GetMethod("getCPtr", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(getCPtr);
            FieldInfo swigCMemOwnBase = type.GetField("swigCMemOwnBase", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(swigCMemOwnBase);
            //cMemoryOwn = (bool) swigCMemOwnBase.GetValue(from);
            swigCMemOwnBase.SetValue(from, false);
            var cptr = (HandleRef)getCPtr.Invoke(null, new object[] { from });
            return (T)Activator.CreateInstance
            (
                typeof(T),
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new object[] { cptr.Handle, cMemoryOwn },
                null
            );
        }
    }
}
#pragma warning restore IDE1006 // Styles d'affectation de noms
