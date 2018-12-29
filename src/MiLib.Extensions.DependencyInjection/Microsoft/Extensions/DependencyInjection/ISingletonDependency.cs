using MiLib.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    [AutoBind(ServiceLifetime.Singleton)]
    public interface ISingletonDependency
    {
    }
}