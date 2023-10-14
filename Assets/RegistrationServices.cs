using MainCore.Maintenance.ServiceDI;
using MainCore.Maintenance.ServiceDI.RegistrationDependencies;
using MainCore.Maintenance.ServiceDI.RegistrationDependencies.Descriptions;
using UnityEngine;

public class RegistrationServices : RegistrationServicesProxyBase
{
    private GlobalSingleton component;
    private Bootstraper bootstraper;
    public override void RegisterServices(DI di)
    {
        di.RegisterService().ByInstance<GlobalSingleton>(component).AsSingleton().AsProject().Done();
        di.RegisterService().ByInstance<Bootstraper>(bootstraper).AsTransient().AsScene(0).Done();
        di.RegisterService().TypeByType<IBullet, Bullet>().AsTransient().AsProject().Done();
        di.Build(); 
    }
}

public class Loader : MonoBehaviour
{
    private Bootstraper bootstraper;
    public void Construct(Bootstraper bootstraper) =>
        this.bootstraper = bootstraper;

    private void Start()
    {
        bootstraper.StartScenario();
    }
}

public class Gun : MonoBehaviour
{
    private IBullet bullet;
    public void Construct(IBullet bullet) =>
        this.bullet = bullet;
    
}

