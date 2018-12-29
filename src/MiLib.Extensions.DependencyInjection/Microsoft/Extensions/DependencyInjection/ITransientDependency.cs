using MiLib.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    [AutoBind(ServiceLifetime.Transient)]
    public interface ITransientDependency
    {
    }
}
