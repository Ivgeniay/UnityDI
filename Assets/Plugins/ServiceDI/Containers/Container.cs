using MainCore.Maintenance.ServiceDI.RegistrationDependencies.Descriptions;
using MainCore.Maintenance.ServiceDI.RegistrationDependencies;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection; 
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEngine;

namespace MainCore.Maintenance.ServiceDI
{
    internal class Container : IContainer
    {
        public Dictionary<ServiceDescription, Type> descriptions = new Dictionary<ServiceDescription, Type>();
        public Dictionary<ServiceDescription, Type> instantiateDescriptions = new Dictionary<ServiceDescription, Type>();
        private ConcurrentDictionary<Type, Func<IScope, object>> buildActivators = new();
        private Scope projectScope;
        private Scope sceneScope;

        public Container(IEnumerable<ServiceDescription> descriptions)
        {
            this.descriptions = descriptions.ToDictionary(key => key, value => value.ByType); 

            projectScope = new(this);
            sceneScope = new(this);
        }
        public IScope CreateScope()
        {
            return new Scope(this);
        }

        private void AddDescription(ServiceDescription serviceDescription)
        {
            if (!instantiateDescriptions.ContainsKey(serviceDescription))
            {
                this.instantiateDescriptions.Add(serviceDescription, serviceDescription.ByType);
            } 
        }

        private Func<IScope, object> BuildActivation(Type service)
        { 
            var description = FindDescriptor(service);

            if (description.TypeDescription == TypeDescription.ByInstance)
                return _ => description.Instance;

            if (description.TypeDescription == TypeDescription.ByLambda)  
                return description.Fabric;

            var type = description.ServiceType;

            if (IsSubclassOfMonoBehaviour(type))
            {
                return MonoBehaviourActivation(description);
            }
            else
            {
                var ctor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Single();
                var args = ctor.GetParameters();

                return scope =>
                {
                    var argsForCtor = new object[args.Length];
                    for (int i = 0; i < argsForCtor.Length; i++)
                    {
                        argsForCtor[i] = CreateInstance(args[i].ParameterType, scope);
                    }

                    return ctor.Invoke(argsForCtor);
                }; 
            }

        }

        private object CallCtor(MethodInfo ctor, ParameterInfo[] parameters, IScope scope, object instance)
        {
            var argsForCtor = new object[parameters.Length];
            for (int i = 0; i < argsForCtor.Length; i++)
                argsForCtor[i] = CreateInstance(parameters[i].ParameterType, scope);

            ctor.Invoke(instance, argsForCtor);
            return instance;
        }

        private Func<IScope, object> MonoBehaviourActivation(ServiceDescription description)
        {
            var type = description.ServiceType;
            var ctor = type.GetMethod("Construct", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (description.IsInstantiate && description.Instance != null)
            { 
                if (description.Instance is MonoBehaviour mono)
                {
                    var inst = GameObject.Instantiate(mono);

                    if (ctor != null)
                    {
                        ParameterInfo[] parameters = ctor.GetParameters();
                        return scope =>
                        {
                            var result = CallCtor(ctor, parameters, scope, inst);

                            var monos = inst.GetComponentsInChildren<MonoBehaviour>().ToList();
                            if (monos.Contains(inst)) monos.Remove(inst);

                            monos.ForEach(el =>
                            {
                                var type = el.GetType();
                                var ctor = type.GetMethod("Construct", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                                if (ctor != null)
                                {
                                    ParameterInfo[] parameters = ctor.GetParameters();
                                    CallCtor(ctor, parameters, scope, inst);
                                }
                            });
                            return result;
                        };
                    }

                    return scope =>
                    {
                        var monos = inst.GetComponentsInChildren<MonoBehaviour>().ToList();
                        if (monos.Contains(inst)) monos.Remove(inst);

                        monos.ForEach(el =>
                        {
                            var type = el.GetType();
                            var ctor = type.GetMethod("Construct", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                            if (ctor != null)
                            {
                                ParameterInfo[] parameters = ctor.GetParameters();
                                CallCtor(ctor, parameters, scope, el);
                            }
                        });
                        return inst;
                    };
                }
            }

            if (description.LifeTime == LifeTime.Singleton)
            {
                var go = new GameObject($"New {type.Name}");
                var instance = go.AddComponent(type);

                if (ctor != null)
                {
                    ParameterInfo[] parameters = ctor.GetParameters();

                    return scope =>
                    {
                        return CallCtor(ctor, parameters, scope, instance);
                        //var argsForCtor = new object[parameters.Length];
                        //for (int i = 0; i < argsForCtor.Length; i++)
                        //    argsForCtor[i] = CreateInstance(parameters[i].ParameterType, scope);

                        //ctor.Invoke(instance, argsForCtor);
                        //return instance;
                    };
                }
                return scope =>
                {
                    return instance;
                };
            }
            
            if (ctor != null)
            {
                ParameterInfo[] parameters = ctor.GetParameters();

                return scope =>
                {
                    var go = new GameObject($"New {type.Name}");
                    var instance = go.AddComponent(type);

                    return CallCtor(ctor, parameters, scope, instance);

                    //var argsForCtor = new object[parameters.Length];
                    //for (int i = 0; i < argsForCtor.Length; i++)
                    //    argsForCtor[i] = CreateInstance(parameters[i].ParameterType, scope);

                    //ctor.Invoke(instance, argsForCtor);
                    //return instance;
                };
            }
            return scope =>
            {
                var go = new GameObject($"New {type.Name}"); 
                var instance = go.AddComponent(type);
                return instance;
            };
        }

        public bool IsSubclassOfMonoBehaviour(Type type)
        {
            return type.IsSubclassOf(typeof(MonoBehaviour));
        }

        private object CreateInstance(Type service, IScope scope)
        {
            return buildActivators.GetOrAdd(service, service => BuildActivation(service))(scope);
        }

        private ServiceDescription FindDescriptor(Type service, Func<Dictionary<ServiceDescription, Type>, ServiceDescription> func = null)
        {
            if (func != null)
            {
                var result = func(descriptions);
                if (result != null) return result;
                result = func(instantiateDescriptions);
                if (result != null) return result;
                return null;
                //else throw new Exception("Not fit parameters");
            }

            var projectService = descriptions.FirstOrDefault(kv => kv.Key.ActionPlace == ActionPlace.Project && kv.Value == service);
            if (projectService.Key != null) return projectService.Key;

            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            var sceneService = descriptions.FirstOrDefault(kv => kv.Key.ActionPlace == ActionPlace.Scene && kv.Key.SceneId == sceneIndex && kv.Value == service);
            if (sceneService.Key != null) return sceneService.Key;

            var instantiateService = instantiateDescriptions.FirstOrDefault(kv => kv.Key.IsInstantiate == true && kv.Value == service);
            if (instantiateService.Key != null) return instantiateService.Key;

            return null;
            //throw new InvalidOperationException($"Service {service} is not registered");
        }

        private class Scope : IScope
        { 
            private Container container;
            private ConcurrentDictionary<Type, object> scopedInstance = new();
            private Stack<object> disposables = new Stack<object>();

            public Scope(Container container)
            {
                this.container = container;
            }

            public T Instantiate<T>(UnityEngine.Object origin)
            {
                var serviceType = typeof(T);
                var descriptor = container.FindDescriptor(serviceType, dictionary =>
                    { 
                        var key = dictionary.FirstOrDefault(kvp => kvp.Key.IsInstantiate == true && kvp.Value == serviceType);
                        return key.Key;
                    }    
                );
                     
                if (descriptor == null)
                {
                    descriptor = new ServiceDescription()
                    {
                        TypeDescription = TypeDescription.ByType,
                        ActionPlace = ActionPlace.Project,
                        LifeTime = LifeTime.Transient,
                        ServiceType = origin.GetType(),
                        ByType = serviceType,
                        Instance = origin,
                        IsInstantiate = true
                    };
                    container.AddDescription(descriptor);
                }
                return (T)this.Resolve(descriptor);
            }

            private object Resolve(ServiceDescription descriptor)
            {
                if (this == container.projectScope || this == container.sceneScope)
                {
                    if (descriptor.LifeTime == LifeTime.Transient) return CreateInstanceInternal(descriptor.ByType);
                    return scopedInstance.GetOrAdd(descriptor.ByType, s => container.CreateInstance(s, this));
                }
                else
                {
                    if (descriptor.IsInstantiate)
                    {
                        return container.MonoBehaviourActivation(descriptor)(this);
                    }

                    switch (descriptor.ActionPlace)
                    {
                        case ActionPlace.Project:
                            return container.projectScope.Resolve(descriptor.ByType);

                        case ActionPlace.Scene:
                            return container.sceneScope.Resolve(descriptor.ByType);

                        default:
                            throw new InvalidOperationException($"There is no service {descriptor.ByType}");
                    }
                }
            }

            public object Resolve(Type service)
            {  
                var descriptor = container.FindDescriptor(service); 
                return Resolve(descriptor);
            }

            private object CreateInstanceInternal(Type service)
            { 
                var result = container.CreateInstance(service, this); 
                if (result is IDisposable || result is IAsyncDisposable)
                    disposables.Push(result); 

                return result;
            }

            public void Dispose()
            {
                foreach (var item in disposables)
                {
                    if (item is IDisposable disposable)
                        disposable.Dispose();
                    else if (item is IAsyncDisposable async)
                        async.DisposeAsync().GetAwaiter().GetResult();   
                }
            }
            public async ValueTask DisposeAsync()
            {
                foreach (var item in disposables)
                {
                    if (item is IAsyncDisposable disposable)
                        await disposable.DisposeAsync();
                    else if (item is IDisposable async)
                        async.Dispose();
                }
            }

        }

        public void Dispose()
        {
            projectScope.Dispose();
            sceneScope.Dispose();
        }
        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            await ((IAsyncDisposable)projectScope).DisposeAsync();
            await ((IAsyncDisposable)sceneScope).DisposeAsync();
        }
    }
}
