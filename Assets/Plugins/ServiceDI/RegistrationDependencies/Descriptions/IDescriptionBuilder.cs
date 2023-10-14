namespace MainCore.Maintenance.ServiceDI.RegistrationDependencies.Descriptions
{
    public interface IDescriptionBuilder
    {
        public ServiceDescription Description { get; set;}
        public ServiceDescription Build();
        public IDescriptionBuilder Done();
    }
}
