using MainCore.Maintenance.ServiceDI.RegistrationDependencies.Descriptions;

namespace MainCore.Maintenance.ServiceDI
{
    public interface IContainerBuilder
    {
        public void Register(IDescriptionBuilder descriptionBuilders);
        public IContainer Build();
    }
}
