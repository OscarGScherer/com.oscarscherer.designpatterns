using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DesignPatterns.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DIBinderAttribute : Attribute
    {
        
    }

    /// <summary>
    /// Class to allow for static dependency injection. Using generics like so is faster than using a dictionary.
    /// </summary>
    /// <typeparam name="T">The type, usually interface, that will be associated with an implementation</typeparam>
    public class DI<T>
    {
        static DI()
        {
            var binders = TypeFinder.TypeQuery(t=>t.GetCustomAttributes(typeof(DIBinderAttribute), false).Any());
            foreach(var binder in binders)
            {
                var bindMethod = binder.GetMethod("Bind", BindingFlags.Static | BindingFlags.NonPublic);
                bindMethod?.Invoke(null, null);
            }
        }

        private static IDIInstancer<T> instancer = new DIDefaultInstancer<T>(); 
        private static T singleton;
        private static ComponentAdder unityComponentAdder;

        // Runtime mutability configuration
        private static bool allowChangingInstancer = false;
        private static bool allowChangingSingleton = false;

        // Getters
        public static T Instantiate() => instancer.Instantiate();
        public static T Singleton() => singleton;
        public static T AddComponent(GameObject gameObject) => unityComponentAdder.AddComponent(gameObject);

        // Helpers
        public static bool HasInstancer() => instancer != null && instancer.GetType() != typeof(DIDefaultInstancer<T>);

        // Setters / Registers (as in you probably only do once)
        public static void RegisterInstancer(IDIInstancer<T> newInstancer)
        {
            if (instancer is not DIDefaultInstancer<T> && !allowChangingInstancer) 
                throw new InvalidOperationException($"Instancer for {typeof(T).Name} already set to {instancer.GetType()}.");
            instancer = newInstancer ?? throw new ArgumentNullException(nameof(newInstancer));
        }
        public static void RegisterSingleton(T newSingleton)
        {
            if (!allowChangingSingleton && singleton != null) 
                throw new InvalidOperationException($"Trying to change a singleton that is set to not allow changes: {typeof(DI<T>)} | {singleton.GetType()}.");
            singleton = newSingleton;
        }
        public static void RegisterUnityComponent<TComponent>() where TComponent : Component, T => unityComponentAdder = new ComponentAdder<TComponent>();
        public static void RegisterPermissions(bool allowChangingInstancer, bool allowChangingSingleton)
        {
            DI<T>.allowChangingInstancer = allowChangingInstancer;
            DI<T>.allowChangingSingleton = allowChangingSingleton;
        }

        private abstract class ComponentAdder
        {
            public abstract T AddComponent(GameObject gameObject);
        }

        private class ComponentAdder<TComponent> : ComponentAdder
            where TComponent : Component, T
        {
            public override T AddComponent(GameObject gameObject) => gameObject.AddComponent<TComponent>();
        }
    }

    public interface IDIInstancer<T>
    {
        public abstract T Instantiate();
    }

    public class DIDefaultInstancer<T> : IDIInstancer<T>
    {
        public T Instantiate() =>
            throw new InvalidOperationException(
                $"No instancer configured for type \"{typeof(T).FullName}\". " +
                $"Call DI<{typeof(T).Name}>.SetInstancer(...) during startup.");
    }
} 