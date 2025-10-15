using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DesignPatterns
{
    public static class TypeFinder
    {
        private static Dictionary<Type, List<Type>> typeToDerivativesCache = new();

        public static List<Type> GetAllDerivedTypes<T>(Func<Type,bool> filter = null) => GetAllDerivedTypes(typeof(T), filter);

        public static List<Type> GetAllDerivedTypes(Type baseType, Func<Type,bool> filter = null)
        {
            filter ??= _ => true;
            if (typeToDerivativesCache.TryGetValue(baseType, out List<Type> derivedTypes)) return derivedTypes.Where(t => filter(t)).ToList();
            derivedTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly =>
                {
                    try
                    {
                        return assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException e)
                    {
                        return e.Types.Where(t => t != null);
                    }
                })
                .Where(t => t != baseType && baseType.IsAssignableFrom(t))
                .ToList();
            typeToDerivativesCache.Add(baseType, derivedTypes);
            return derivedTypes.Where(t => filter(t)).ToList();
        }

        public static Type GetTypeFromTypeFullName(string fullTypeName)
        {
            if (fullTypeName == null) return null;
            // Look through all loaded assemblies
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(fullTypeName);
                if (type != null) return type;
            }
            return null;
        }
    }
}
