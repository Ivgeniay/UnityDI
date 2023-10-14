using System;

namespace MainCore.Maintenance.ServiceDI.RegistrationDependencies.Descriptions
{
    public class DescriptionBuilder : IDescriptionBuilder
    {
        public ServiceDescription Description { get; set; }
        private DI di;

        public DescriptionBuilder(DI dI) 
        { 
            this.Description = new();
            di = dI;
        }

        public ServiceDescription Build()
        { 
            return Description;
        }

        public IDescriptionBuilder Done()
        {
            di.DescriptionRegistry(this);
            return this;
        }
    }
}
