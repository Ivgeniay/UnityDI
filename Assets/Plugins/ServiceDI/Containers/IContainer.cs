using System;

namespace MainCore.Maintenance.ServiceDI
{
    public interface IContainer: IDisposable, IAsyncDisposable
    {
        IScope CreateScope();
    }
}
