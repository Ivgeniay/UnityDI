using MainCore.Maintenance.ServiceDI; 
using UnityEngine;

namespace MainCore.Maintenance.ServiceDI.ExMonoBehaviour
{
    public class DiMonoBehaviour : MonoBehaviour
    {
        protected virtual void Awake()
        {
            DI.Instance.MonoInject(this, true);
        } 
    }
}
