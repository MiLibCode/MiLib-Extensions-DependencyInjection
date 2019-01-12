using MiLib.DependencyInjection;
using MiLib.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        private static MiLibScanOptions _options;

        /// <summary>
        /// Scan calling assembly and bind all classes in that assembly automatically to service collection based on convention and <see cref="ServiceLifetime"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="optionsAction"></param>
        public static IServiceCollection ScanForDependency<T>(this IServiceCollection services, Action<MiLibScanOptions> optionsAction = null)
        {
            return ScanForDependency(services, new[] { typeof(T).GetTypeInfo().Assembly }, optionsAction);
        }

        /// <summary>
        /// Scan supplied assemblies and bind them automatically to service collection based on convention and <see cref="ServiceLifetime"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        /// <param name="optionsAction"></param>
        public static IServiceCollection ScanForDependency(this IServiceCollection services, Assembly assembly, Action<MiLibScanOptions> optionsAction = null)
        {
            return ScanForDependency(services, new[] { assembly }, optionsAction);
        }

        /// <summary>
        /// Scan supplied assemblies and bind them automatically to service collection based on convention and <see cref="ServiceLifetime"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <param name="optionsAction"></param>
        public static IServiceCollection ScanForDependency(this IServiceCollection services, IEnumerable<Assembly> assemblies, Action<MiLibScanOptions> optionsAction = null)
        {
            _options = new MiLibScanOptions();
            optionsAction?.Invoke(_options);
            return ScanAndRegisterAssemblies(services, assemblies);
        }

        /// <summary>
        /// Add Type to service collection based on default convention and <see cref="ServiceLifetime"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="type"></param>
        /// <param name="optionsAction"></param>
        public static IServiceCollection AddTypeOf(this IServiceCollection services, Type type, Action<MiLibScanOptions> optionsAction = null)
        {
            _options = new MiLibScanOptions();
            optionsAction?.Invoke(_options);
            AddType(services, type);
            return services;
        }

        /// <summary>
        /// Add all classes in <see cref="T"/> assembly automatically to service collection based on default convention and <see cref="ServiceLifetime"/>
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddAssemblyOf<T>(this IServiceCollection services)
        {
            return services.AddAssembly(typeof(T).GetTypeInfo().Assembly);
        }

        /// <summary>
        /// Add all classes in assembly automatically to service collection based on default convention and <see cref="ServiceLifetime"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        public static IServiceCollection AddAssembly(this IServiceCollection services, Assembly assembly)
        {
            _options = new MiLibScanOptions();
            return ScanAndRegisterAssemblies(services, new[] { assembly });
        }

        private static IServiceCollection ScanAndRegisterAssemblies(IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                var types = AssemblyHelper.GetAllTypes(assembly)
                    .Where(type => type != null && type.IsClass && !type.IsAbstract && !type.IsGenericType).ToArray();

                AddTypes(services, types);
            }

            return services;
        }

        private static void AddTypes(IServiceCollection services, params Type[] types)
        {
            foreach (var type in types)
            {
                AddType(services, type);
            }
        }

        private static void AddType(IServiceCollection services, Type type)
        {
            var autoBindAttribute = GetAutoBindAttributeOrNull(type);
            var lifeTime = GetLifeTimeOrNull(type, autoBindAttribute);
            if (lifeTime == null)
            {
                return;
            }

            foreach (var serviceType in GetDefaultExposedServices(services, type))
            {
                var serviceDescriptor = ServiceDescriptor.Describe(serviceType, type, lifeTime.Value);
                services.Add(serviceDescriptor);
            }

        }

        private static AutoBindAttribute GetAutoBindAttributeOrNull(Type type)
        {
            //return type.GetCustomAttribute<AutoBindAttribute>(true);
            return type.GetCustomAttributesIncludingBaseInterfaces<AutoBindAttribute>().FirstOrDefault();
        }

        private static ServiceLifetime? GetLifeTimeOrNull(Type type, AutoBindAttribute autoBindAttribute)
        {
            return autoBindAttribute?.LifeCycle;
        }

        private static IEnumerable<Type> GetDefaultExposedServices(IServiceCollection services, Type type)
        {
            var serviceTypes = new List<Type>();

            //Self Register Type 
            serviceTypes.Add(type);

            foreach (var interfaceType in type.GetTypeInfo().GetInterfaces())
            {
                if (_options.DefaultConventionsBindingOnly)
                {
                    var interfaceName = interfaceType.Name;

                    if (interfaceName.StartsWith("I"))
                    {
                        interfaceName = interfaceName.Right(interfaceName.Length - 1);
                    }

                    if (type.Name.EndsWith(interfaceName))
                    {
                        serviceTypes.Add(interfaceType);
                    }
                }
                else
                    serviceTypes.Add(interfaceType);
            }

            return serviceTypes;
        }

    }
}
