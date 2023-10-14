using MainCore.Maintenance.ServiceDI.RegistrationDependencies.Descriptions;
using System.Collections.Generic;

namespace MainCore.Maintenance.ServiceDI
{
    public class ContainerBuilder : IContainerBuilder
    { 
        private List<IDescriptionBuilder> descriptionsBuilders = new();
        private List<ServiceDescription> descriptions = new();
        public IContainer Build()
        {
            foreach (var builders in descriptionsBuilders)
                descriptions.Add(builders.Build());
            
            return new Container(descriptions); 
        }

        public void Register(IDescriptionBuilder descriptionBuilders)
        {
            descriptionsBuilders.Add(descriptionBuilders);
        }
         
    }
}
