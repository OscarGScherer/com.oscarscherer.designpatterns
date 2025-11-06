using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DesignPatterns
{
    public static class TypeFinder
    {
        private static Dictionary<Type, List<Type>> typeToDerivativesCache = new();

        public static List<Type> GetAllDerivedTypes<T>() => GetAllDerivedTypes(typeof(T));

        public static List<Type> GetAllDerivedTypes(Type baseType)
        {
            if (typeToDerivativesCache.TryGetValue(baseType, out List<Type> derivedTypes)) return derivedTypes;
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
            return derivedTypes;
        }
    }
}
