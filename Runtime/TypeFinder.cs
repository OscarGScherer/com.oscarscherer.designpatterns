using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DesignPatterns
{
    public static class TypeFinder
    {
        public static List<Type> GetAllDerivedTypes<T>() => GetAllDerivedTypes(typeof(T));

        public static List<Type> GetAllDerivedTypes(Type baseType, bool includeAbstractTypes = false)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
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
                .Where(type => baseType.IsAssignableFrom(type) && type != baseType && (!type.IsAbstract || includeAbstractTypes))
                .ToList();
        }

        public static Type GetTypeFromTypeFullName(string fullTypeName)
        {
            // Look through all loaded assemblies
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(fullTypeName);
                if (type != null) return type;
            }
            throw new ArgumentException($"Enum type '{fullTypeName}' not found in loaded assemblies.");
        }
    }
}
