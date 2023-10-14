using UnityEngine;
using MainCore.Maintenance.ServiceDI.RegistrationDependencies.Descriptions;

namespace MainCore.Maintenance.ServiceDI.RegistrationDependencies
{
    public abstract class RegistrationServicesProxyBase : MonoBehaviour
    {
        public abstract void RegisterServices(DI di);
    }
}
