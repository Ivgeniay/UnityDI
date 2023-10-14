# UnityDI

![Unity Version](https://img.shields.io/badge/Unity-2021.1%2B-blue.svg)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

> Simple multi-scene DI container

## Installation

1. Clone this repository or download the archive and extract it. (Or you can download .unitypackage in the "Release" section)
2. Move the contents of Assets to the root folder of your Unity project.

## Usage

Sample usage:

```csharp
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

    private void Start() =>
        bootstraper.StartScenario();
}

public class Gun : MonoBehaviour
{
    private IBullet bullet;

    public void Construct(IBullet bullet) =>
        this.bullet = bullet;
}

public class Bullet : IBullet { }
public interface IBullet { }
```
