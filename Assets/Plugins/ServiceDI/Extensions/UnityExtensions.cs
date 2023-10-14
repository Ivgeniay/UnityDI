using MainCore.Maintenance.ServiceDI;
using UnityEngine;
using Object = UnityEngine.Object;
 
public static class UnityExtensions
{
    public static T InstantiateFromContainer<T>(this Object obj, T original) where T : Object
    {
        return DI.Instance.Instantiate<T>(original, Vector3.zero, Quaternion.identity, null);
    }
    public static T InstantiateFromContainer<T>(this Object obj, T original, Transform parent) where T : Object
    {
        return DI.Instance.Instantiate<T>(original, Vector3.zero, Quaternion.identity, parent);
    }
    public static T InstantiateFromContainer<T>(this Object obj, T original, Vector3 position, Quaternion rotation) where T : Object
    {
        return DI.Instance.Instantiate<T>(original, position, rotation, null);
    }
    public static T InstantiateFromContainer<T>(this Object obj, T original, Vector3 position, Quaternion rotation, Transform parent) where T : Object
    {
        return DI.Instance.Instantiate(original, position, rotation, parent);
    }
}

