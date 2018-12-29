using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MiLib.Extensions.DependencyInjection.Test
{
    public class ServiceProviderHelper
    {
        public readonly IServiceCollection ServiceCollection;
        public ServiceProviderHelper(IEnumerable<Assembly> assemblies, Action<MiLibScanOptions> optionsAction = null)
        {
            ServiceCollection = new ServiceCollection();
            ServiceCollection.ScanForDependency(assemblies, optionsAction);
            ServiceProvider = ServiceCollection.BuildServiceProvider();
        }

        public ServiceProviderHelper(Action<MiLibScanOptions> optionsAction = null)
        {
            ServiceCollection = new ServiceCollection();
            ServiceCollection.ScanForDependency<ServiceProviderHelper>(optionsAction);
            ServiceProvider = ServiceCollection.BuildServiceProvider();
        }

        public IServiceProvider ServiceProvider { get; set; }

        public T GetService<T>() => ServiceProvider.GetService<T>();
        public IEnumerable<T> GetServices<T>() => ServiceProvider.GetServices<T>();
    }

    public class ServiceProviderFixture
    {
        public ServiceProviderFixture()
        {
            ServiceProviderHelper = new ServiceProviderHelper(option => { option.DefaultConventionsBindingOnly = false; });
            ServiceCollection = ServiceProviderHelper.ServiceCollection;
        }

        public readonly IServiceCollection ServiceCollection;

        public readonly ServiceProviderHelper ServiceProviderHelper;

        public IServiceProvider ServiceProvider => ServiceProviderHelper.ServiceProvider;

        public T GetService<T>() => ServiceProviderHelper.GetService<T>();
        public IEnumerable<T> GetServices<T>() => ServiceProviderHelper.GetServices<T>();
    }
}
