using Microsoft.Extensions.DependencyInjection;
using System;

namespace MiLib.DependencyInjection
{
    public class AutoBindAttribute : Attribute
    {
        public AutoBindAttribute() : this(ServiceLifetime.Transient)
        {
        }

        public AutoBindAttribute(ServiceLifetime lifeCycle)
        {
            LifeCycle = lifeCycle;
        }

        public ServiceLifetime LifeCycle { get; private set; }
    }
}
