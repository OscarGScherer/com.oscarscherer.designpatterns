using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DesignPatterns
{
    public static class TypeFinder
    {
        private static Type[] allTypes;

        private static Dictionary<Type, List<Type>> typeToDerivativesCache = new();

        public static List<Type> GetAllDerivedTypes<T>() => GetAllDerivedTypes(typeof(T));

        public static List<Type> GetAllDerivedTypes(Type baseType)
        {
            if (typeToDerivativesCache.TryGetValue(baseType, out List<Type> derivedTypes)) return derivedTypes;
            allTypes ??= GetAllTypes();
            derivedTypes = allTypes.Where(t => t != baseType && baseType.IsAssignableFrom(t)).ToList();
            typeToDerivativesCache.Add(baseType, derivedTypes);
            return derivedTypes;
        }

        private static Type[] GetAllTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => {
                    try {
                        return assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException e) {
                        return e.Types.Where(t => t != null);
                    }
                })
                .ToArray();
        }

        public static Type[] TypeQuery(Func<Type,bool> filter)
        {
            allTypes ??= GetAllTypes();
            return allTypes.Where(filter).ToArray();
        }
    }
}
