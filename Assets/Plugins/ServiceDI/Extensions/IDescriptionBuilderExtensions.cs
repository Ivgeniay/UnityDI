using System;

namespace MainCore.Maintenance.ServiceDI.RegistrationDependencies.Descriptions
{
    public static class IDescriptionBuilderExtensions
    {
        private static IDescriptionBuilder Type(this IDescriptionBuilder builder, Type type)
        {
            builder.Description.TypeDescription = TypeDescription.ByType;
            builder.Description.ServiceType = type;
            return builder;
        }

        private static IDescriptionBuilder ByType(this IDescriptionBuilder builder, Type type)
        {
            builder.Description.TypeDescription = TypeDescription.ByType;
            builder.Description.ByType = type;
            return builder;
        }

        public static IDescriptionBuilder TypeByType(this IDescriptionBuilder builder, Type byType, Type registerType)
        {
            builder.Type(registerType);
            builder.ByType(byType);
            return builder;
        }

        private static IDescriptionBuilder Type<T>(this IDescriptionBuilder builder)
        {
            builder.Type(typeof(T));
            return builder;
        }

        private static IDescriptionBuilder ByType<T>(this IDescriptionBuilder builder)
        {
            builder.ByType(typeof(T));
            return builder;
        }

        public static IDescriptionBuilder TypeByType<T1, T2>(this IDescriptionBuilder builder) where T2 : T1
        {
            builder.Type(typeof(T2));
            builder.ByType(typeof(T1));
            return builder;
        }

        public static IDescriptionBuilder ByInstance(this IDescriptionBuilder builder, object instance)
        {
            builder.Description.TypeDescription = TypeDescription.ByInstance;
            builder.Description.ServiceType = instance.GetType();
            builder.Description.ByType = instance.GetType();
            builder.Description.Instance = instance;
            return builder;
        }

        public static IDescriptionBuilder ByInstance<T1>(this IDescriptionBuilder builder, object instance)
        {
            builder.Description.TypeDescription = TypeDescription.ByInstance;
            builder.Description.ServiceType = instance.GetType();
            builder.Description.ByType = typeof(T1);
            builder.Description.Instance = instance;
            return builder;
        }

        public static IDescriptionBuilder ByFabric<T>(this IDescriptionBuilder builder, Func<IScope, T> func)
        {
            builder.Type(typeof(T));
            builder.Description.Fabric = el => func(el);
            builder.Description.ServiceType = typeof(T);
            builder.Description.ByType = typeof(T);
            builder.Description.TypeDescription = TypeDescription.ByLambda;
            //builder.Description.LifeTime = LifeTime.Transient;
            return builder;
        }



        public static IDescriptionBuilder AsSingleton(this IDescriptionBuilder builder)
        {
            builder.Description.LifeTime = LifeTime.Singleton;
            return builder;
        } 

        public static IDescriptionBuilder AsTransient(this IDescriptionBuilder builder)
        {
            builder.Description.LifeTime = LifeTime.Transient;
            return builder;
        }

        public static IDescriptionBuilder AsProject(this IDescriptionBuilder builder)
        {
            builder.Description.ActionPlace = ActionPlace.Project;
            return builder;
        }

        public static IDescriptionBuilder AsScene(this IDescriptionBuilder builder, int sceneId)
        {
            builder.Description.ActionPlace = ActionPlace.Scene;
            builder.Description.SceneId = sceneId;
            return builder;
        }
         
    }
}
