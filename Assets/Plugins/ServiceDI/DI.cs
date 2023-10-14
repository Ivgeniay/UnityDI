using MainCore.Maintenance.ServiceDI.RegistrationDependencies.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MainCore.Maintenance.ServiceDI
{
    public sealed class DI 
    {
        public static DI Instance;
        public bool IsBuilded = false;
        private List<MonoBehaviour> injected = new List<MonoBehaviour> ();

        private IContainerBuilder builder;
        private IContainer container;
        internal IScope scope;

        public DI ()
        {
            Instance = this;
            builder = new ContainerBuilder();
        }

        public void Build()
        {
            container = builder.Build();
            scope = container.CreateScope();
            IsBuilded = true;
        }

        public IDescriptionBuilder RegisterService() => new DescriptionBuilder(this);
        public void DescriptionRegistry(IDescriptionBuilder descriptionBuilder) => builder.Register(descriptionBuilder);

        public void MonoInject(MonoBehaviour monoScript, bool useParent = false)
        {
            if (!IsBuilded) return;
            if (injected.Contains(monoScript)) return;

            if (useParent)
            {
                var monos = monoScript.GetComponentsInChildren<MonoBehaviour>().ToList();
                foreach (var scr in monos) MonoInject(scr);
            }
            else
            {
                MonoInject(monoScript);
            } 
        }

        private void MonoInject(MonoBehaviour monoScript)
        {
            var type = monoScript.GetType();
            var ctor = type.GetMethod("Construct", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (ctor != null)
            {
                ParameterInfo[] parameters = ctor.GetParameters();

                var argsForCtor = new object[parameters.Length];
                for (int i = 0; i < argsForCtor.Length; i++)
                    argsForCtor[i] = this.Resolve(parameters[i].ParameterType);
                ctor.Invoke(monoScript, argsForCtor);
            } 
            injected.Add(monoScript);
        }
    }
}
