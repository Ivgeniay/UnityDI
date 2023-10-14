namespace MainCore.Maintenance.ServiceDI
{
    public static class IScopeExtensions
    {
        public static T Resolve<T>(this IScope scope)
        {
            return (T)scope.Resolve(typeof(T));
        }
    }
}
