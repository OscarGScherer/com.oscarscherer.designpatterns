using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace DesignPatterns.DependencyInjection
{
    public enum Scope { Singleton, Local }

    [CreateAssetMenu(menuName = "Project Dependency Injector")]
    public class ProjectDependencyInjector : ScriptableObject
    {
        public static readonly Func<Type, bool>[] scopeFilters =
        {
            FilterForSingletons,
            FilterForLocals
        };

        private static bool FilterForSingletons(Type type) => !type.IsAbstract && !type.IsInterface && !typeof(UnityEngine.Object).IsAssignableFrom(type);
        private static bool FilterForLocals(Type type) => !type.IsAbstract && !type.IsInterface && !typeof(UnityEngine.Object).IsAssignableFrom(type) && type.GetConstructor(Type.EmptyTypes) != null;

        private static Dictionary<Type, (Type, object)>[] bindings = Initialize<Dictionary<Type, (Type, object)>, Scope>();
        private static T[] Initialize<T, E>() where E : Enum where T : new()
        {
            int length = Enum.GetValues(typeof(E)).Length;
            T[] arr = new T[length];
            for (int i = 0; i < length; i++) arr[i] = new T();
            return arr;
        }

        public static (Type, object) GetConcrete<T>(Scope scope)
        {
            (Type, object) concrete;
            if (bindings[(int)scope].TryGetValue(typeof(T), out concrete)) return concrete;
            concrete.Item1 = TypeFinder.GetAllDerivedTypes(typeof(T)).Where(scopeFilters[(int)scope]).FirstOrDefault();
            if (concrete.Item1 == null)
            {
                Debug.LogError($"A class is requesting an implementation of {typeof(T).Name}, but no implementation exists.");
                return (null, null);
            }
            if (scope == Scope.Singleton) concrete.Item2 = Activator.CreateInstance(concrete.Item1);
            bindings[(int)scope].Add(typeof(T), concrete);
            return concrete;
        }

        public InterfaceBindings interfaceBindings;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitOnLoad()
        {
            var pdi = Resources.Load<ProjectDependencyInjector>("ProjectDependencyInjector");
            if (pdi == null) return;

            int scopes = Enum.GetValues(typeof(Scope)).Length;
            bindings = new Dictionary<Type, (Type, object)>[scopes];
            for (int i = 0; i < scopes; i++) bindings[i] = new();

            foreach(Scope scope in Enum.GetValues(typeof(Scope)))
            {
                foreach (InterfaceBinding ib in pdi.interfaceBindings[scope])
                {
                    if (ib.interfaceType == null || ib.concreteType == null) continue;
                    bindings[(int)scope].Add(ib.interfaceType, (ib.concreteType, ib.defaultValue));
                }
            }
        }
    }

    public static class DI
    {
        public static T New<T>()
        {
            Type concrete = ProjectDependencyInjector.GetConcrete<T>(Scope.Local).Item1;
            if (concrete == null) return default;
            return (T)Activator.CreateInstance(concrete);
        }
        
        public static T Singleton<T>()
        {
            return (T) ProjectDependencyInjector.GetConcrete<T>(Scope.Singleton).Item2;
        }
    }
}