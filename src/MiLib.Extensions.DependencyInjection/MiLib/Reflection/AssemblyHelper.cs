using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MiLib.Reflection
{
    internal static class AssemblyHelper
    {
        public static IReadOnlyList<Type> GetAllTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types;
            }
        }
    }
}
