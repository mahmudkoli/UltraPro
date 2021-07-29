using AutoMapper;
using UltraPro.Common.Model;
using System;
using System.Linq;
using System.Reflection;

namespace UltraPro.Common.Mappings
{
    public static class CommonMappingProfile
    {
        public static void ApplyMappingsFromAssembly(Assembly assembly, object obj)
        {
            var types = assembly.GetExportedTypes()
                        .Where(t => t.GetInterfaces()
                            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
                        .ToList();

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);

                var methodInfo = type.GetMethod("Mapping") ?? type.GetInterface("IMapFrom`1").GetMethod("Mapping");
                
                methodInfo?.Invoke(instance, new object[] { obj });
            }
        }
    }
}