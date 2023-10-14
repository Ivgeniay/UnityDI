using System;

namespace MainCore.Maintenance.ServiceDI.RegistrationDependencies.Descriptions
{
    public sealed class ServiceDescription
    {
        public TypeDescription TypeDescription;
        public Type ByType { get; set; }
        public Type ServiceType { get; set; }
        public ActionPlace ActionPlace { get; set; } = ActionPlace.Project;
        public LifeTime LifeTime { get; set; } = LifeTime.Singleton;
        public int SceneId { get; set; }
        public Func<IScope, object> Fabric { get; set; }
        public object Instance { get; set; }
        public bool IsInstantiate { get; set; }



        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != this.GetType())
                return false;

            ServiceDescription other = (ServiceDescription)obj;

            return TypeDescription == other.TypeDescription &&
                   ByType == other.ByType &&
                   ServiceType == other.ServiceType &&
                   ActionPlace == other.ActionPlace &&
                   LifeTime == other.LifeTime &&
                   SceneId == other.SceneId &&
                   Equals(Fabric, other.Fabric) &&
                   Equals(Instance, other.Instance);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 9;
                hash = hash * 10 + TypeDescription.GetHashCode();
                if (ByType != null) hash = hash * 11 + ByType.GetHashCode();
                if (ServiceType != null) hash = hash * 12 + ServiceType.GetHashCode();
                hash = hash * 13 + ActionPlace.GetHashCode();
                hash = hash * 14 + LifeTime.GetHashCode();
                hash = hash * 15 + SceneId.GetHashCode();
                if (Fabric != null) hash = hash * 16 + Fabric.GetHashCode();
                if (Instance != null) hash = hash * 17 + Instance.GetHashCode(); 
                return hash;
            }
        }
    }
}
