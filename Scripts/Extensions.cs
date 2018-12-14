using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine.Assertions;

#pragma warning disable IDE1006 // Styles d'affectation de noms
namespace SolAR
{
    using UUID = SWIGTYPE_p_org__bcom__xpcf__uuids__uuid;

    public static class Extensions
    {
        public static T AddTo<T>(this T disposable, IList<IDisposable> list) where T : IDisposable
        {
            list.Add(disposable);
            return disposable;
        }

        public static string GET_UUID(string name)
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
            var uuid = SolARWrapper.toUUID(GET_UUID(type));
            return xpcfComponentManager.createComponent(uuid);
        }

        public static IComponentIntrospect create(this IComponentManager xpcfComponentManager, string type, string name)
        {
            var uuid = SolARWrapper.toUUID(GET_UUID(type));
            return xpcfComponentManager.createComponent(name, uuid);
        }

        public static UUID GET_UUID<T>()
        {
            var name = typeof(T).Name;
            return SolARWrapper.toUUID(GET_UUID(name));
        }

        public static T bindTo<T>(this IComponentIntrospect component) where T : class
        {
            var name = typeof(T).Name;
            var type = typeof(IComponentIntrospect);
            var bindTo = type.GetMethod("bindTo_" + name, BindingFlags.Public | BindingFlags.Instance);
            Assert.IsNotNull(bindTo);
            return bindTo.Invoke(component, null) as T;
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

        [Obsolete]
        static IComponentIntrospect bindTo(this IComponentIntrospect component, string name)
        {
            var uuid = SolARWrapper.toUUID(GET_UUID(name));
            Assert.IsTrue(component.implements(uuid));
            return component.queryInterface(uuid);
        }

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
