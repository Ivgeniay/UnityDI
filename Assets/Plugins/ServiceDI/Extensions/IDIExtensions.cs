using System;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace MainCore.Maintenance.ServiceDI
{
    public static class IDIExtensions
    {
        public static object Resolve(this DI di, Type type)
        {
            if (di.scope == null) throw new NullReferenceException($"Scope is null");
            return di.scope.Resolve(type);
        }

        public static T Resolve<T>(this DI di)
        {
            if (di.scope == null) throw new NullReferenceException($"Scope is null");
            return di.scope.Resolve<T>();
        }

        private static T ResolveFromInstance<T>(this DI di, Object type)
        {
            if (di.scope == null) throw new NullReferenceException($"Scope is null");
            return di.scope.Instantiate<T>(type);
        }


        public static T Instantiate<T>(this DI di, T original) where T : Object
        {
            return Instantiate<T>(di, original, Vector3.zero, Quaternion.identity, null);
        }
        public static T Instantiate<T>(this DI di, T original, Transform parent) where T : Object
        {
            return Instantiate<T>(di, original, Vector3.zero, Quaternion.identity, parent);
        } 
        public static T Instantiate<T>(this DI di, T original, Vector3 position, Quaternion rotation) where T : Object
        {
            return Instantiate<T>(di, original, position, rotation, null);
        } 
        public static T Instantiate<T>(this DI di, T original, Vector3 position, Quaternion rotation, Transform parent) where T : Object
        {
            var _object = di.ResolveFromInstance<T>(original);

            Transform objectTransform = (_object as GameObject)?.transform;
            if (objectTransform == null) objectTransform = (_object as MonoBehaviour)?.transform;

            objectTransform.position = position;
            objectTransform.rotation = rotation;
            objectTransform.parent = parent;

            return (T)_object;
        }

        //public static object Instantiate(this DI di, Object original)
        //{
        //    var _object = Resolve(di, original.GetType());
        //    return _object; //GameObject.Instantiate(original);
        //}
        //public static object Instantiate(this DI di, Object original, Transform parent)
        //{
        //    return GameObject.Instantiate(original, parent);
        //}
        //public static object Instantiate(this DI di, Object original, Transform parent, bool worldPositionStays = true)
        //{
        //    return GameObject.Instantiate(original, parent, worldPositionStays);
        //}
        //public static object Instantiate(this DI di, Object original, Vector3 position, Quaternion rotation)
        //{
        //    return GameObject.Instantiate(original, position, rotation);
        //}
        //public static object Instantiate(this DI di, Object original, Vector3 position, Quaternion rotation, Transform parent)
        //{
        //    Object _object = (Object)Resolve(di, original.GetType());
        //    return _object;//GameObject.Instantiate(original, position, rotation, parent);
        //}
    }
}
