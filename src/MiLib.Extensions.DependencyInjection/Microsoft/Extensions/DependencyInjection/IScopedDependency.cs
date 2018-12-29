using MiLib.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    [AutoBind(ServiceLifetime.Scoped)]
    public interface IScopedDependency
    {
    }
}