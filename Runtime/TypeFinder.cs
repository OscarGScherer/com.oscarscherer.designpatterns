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
    }
}
