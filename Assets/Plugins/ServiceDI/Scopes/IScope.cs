using System; 

namespace MainCore.Maintenance.ServiceDI
{
    public interface IScope: IDisposable, IAsyncDisposable
    {
        public object Resolve(Type service);
        public T Instantiate<T>(UnityEngine.Object origin);
    }
}
