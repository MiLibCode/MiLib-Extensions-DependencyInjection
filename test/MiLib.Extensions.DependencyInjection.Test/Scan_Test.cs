using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace MiLib.Extensions.DependencyInjection.Test
{
    public class Scan_Test : IClassFixture<ServiceProviderFixture>
    {
        private readonly ServiceProviderFixture _fixture;

        public Scan_Test(ServiceProviderFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void Scan_Should_Ignore_AbstractClass()
        {
            //Act
            var abstractHelloWorld = _fixture.GetService<AbstractHelloWorld>();

            //Assert
            abstractHelloWorld.ShouldBeNull();
        }

        [Fact]
        public void Should_Register_Transient()
        {
            //Assert
            _fixture.ServiceCollection.ShouldContainTransient(typeof(MyTransientClass));
        }

        [Fact]
        public void Should_Register_Singleton()
        {
            //Assert
            _fixture.ServiceCollection.ShouldContainSingleton(typeof(MySingletonClass));
        }

        [Fact]
        public void Should_Register_Scoped()
        {
            //Assert
            _fixture.ServiceCollection.ShouldContainScoped(typeof(MyScopedClass));
        }

        [Fact]
        public void Should_Not_Be_Register()
        {
            //Assert
            _fixture.ServiceCollection.ShouldNotContainService(typeof(NoDependencyClass));
        }

        [Fact]
        public void Should_Register_And_GetServices()
        {
            //Act
            var helloWorldScoped = _fixture.GetService<IHelloWorldScopedInterface>();
            var helloWorldSingleton = _fixture.GetService<IHelloWorldSingletonInterface>();
            var helloWorldTransient = _fixture.GetService<IHelloWorldTransientInterface>();

            //Assert
            helloWorldScoped.ShouldBeOfType<MyScopedClass>();
            helloWorldSingleton.ShouldBeOfType<MySingletonClass>();
            helloWorldTransient.ShouldBeOfType<MyTransientClass>();
        }

        [Fact]
        public void Should_Register_Using_Default_Convention_Only()
        {
            //Act
            var services = new ServiceCollection();

            services.ScanForDependency<ServiceProviderHelper>(options => options.DefaultConventionsBindingOnly = true);

            var serviceProvider = services.BuildServiceProvider();

            //Assert
            serviceProvider.GetService<ITaxCalculator>().ShouldBeOfType(typeof(TaxCalculator));
            serviceProvider.GetService<ICalculator>().ShouldBeOfType(typeof(TaxCalculator));
            serviceProvider.GetService<ICanCalculate>().ShouldBeNull();
            serviceProvider.GetService(typeof(TaxCalculator)).ShouldBeOfType<TaxCalculator>();
        }

        [Fact]
        public void Should_Register_Without_Using_Default_Convention()
        {
            //Assert
            _fixture.GetService<ITaxCalculator>().ShouldBeOfType(typeof(TaxCalculator));
            _fixture.GetService<ICalculator>().ShouldBeOfType(typeof(TaxCalculator));
            _fixture.GetService<ICanCalculate>().ShouldBeOfType<TaxCalculator>();
            _fixture.ServiceProvider.GetService(typeof(TaxCalculator)).ShouldBeOfType<TaxCalculator>();
        }

        [Fact]
        public void Should_Add_Assembly()
        {
            //Act
            var services = new ServiceCollection();
            services.AddAssemblyOf<ServiceProviderHelper>();

            var serviceProvider = services.BuildServiceProvider();

            //Assert
            serviceProvider.GetService<ITaxCalculator>().ShouldBeOfType(typeof(TaxCalculator));
            serviceProvider.GetService<ICalculator>().ShouldBeOfType(typeof(TaxCalculator));
            serviceProvider.GetService<ICanCalculate>().ShouldBeNull();
            serviceProvider.GetService(typeof(TaxCalculator)).ShouldBeOfType<TaxCalculator>();
        }

        [Fact]
        public void Should_Add_TypeOf()
        {
            //Act
            var services = new ServiceCollection();
            services.AddTypeOf(typeof(TaxCalculator));

            var serviceProvider = services.BuildServiceProvider();

            //Assert
            serviceProvider.GetService<ITaxCalculator>().ShouldBeOfType(typeof(TaxCalculator));
            serviceProvider.GetService<ICalculator>().ShouldBeOfType(typeof(TaxCalculator));
            serviceProvider.GetService<ICanCalculate>().ShouldBeNull();
            serviceProvider.GetService(typeof(TaxCalculator)).ShouldBeOfType<TaxCalculator>();
        }

        [Fact]
        public void Should_Add_TypeOf_Without_Default_Convention()
        {
            //Act
            var services = new ServiceCollection();
            services.AddTypeOf(typeof(TaxCalculator), opt => opt.DefaultConventionsBindingOnly = false);

            var serviceProvider = services.BuildServiceProvider();

            //Assert
            serviceProvider.GetService<ITaxCalculator>().ShouldBeOfType(typeof(TaxCalculator));
            serviceProvider.GetService<ICalculator>().ShouldBeOfType(typeof(TaxCalculator));
            serviceProvider.GetService<ICanCalculate>().ShouldNotBeNull();
            serviceProvider.GetService<ICanCalculate>().ShouldBeOfType(typeof(TaxCalculator));
            serviceProvider.GetService(typeof(TaxCalculator)).ShouldBeOfType<TaxCalculator>();
        }
    }


    public interface IHelloWorldForAbstract : ISingletonDependency
    {
    }

    public abstract class AbstractHelloWorld : IHelloWorldForAbstract
    {

    }

    public class NoDependencyClass
    {

    }

    public class MyTransientClass : IHelloWorldTransientInterface
    {

    }

    public class MySingletonClass : IHelloWorldSingletonInterface
    {

    }

    public class MyScopedClass : IHelloWorldScopedInterface
    {

    }

    public interface IHelloWorldScopedInterface : IScopedDependency
    {
    }

    public interface IHelloWorldTransientInterface : ITransientDependency
    {
    }

    public interface IHelloWorldSingletonInterface : ISingletonDependency
    {
    }

    public interface ITaxCalculator : ISingletonDependency { }
    public interface ICalculator : ISingletonDependency { }
    public interface ICanCalculate : ISingletonDependency { }

    public class TaxCalculator : ITaxCalculator, ICalculator, ICanCalculate
    {

    }
}
