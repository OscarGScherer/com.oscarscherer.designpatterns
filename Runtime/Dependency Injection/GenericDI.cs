using System;

namespace DesignPatterns.DependencyInjection
{
    /// <summary>
    /// Class to allow for static dependency injection. Using generics like so is faster than using a dictionary.
    /// </summary>
    /// <typeparam name="T">The type, usually interface, that will be associated with an implementation</typeparam>
    public class DI<T>
    {
        private static IDIInstancer<T> instancer = new DIDefaultInstancer<T>(); 
        private static T singleton;

        // Runtime mutability configuration
        private static bool allowChangingInstancer = false;
        private static bool allowChangingSingleton = false;

        // Getters
        public static T Instantiate() => instancer.Instantiate();
        public static T Singleton() => singleton;

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
        public static void RegisterPermissions(bool allowChangingInstancer, bool allowChangingSingleton)
        {
            DI<T>.allowChangingInstancer = allowChangingInstancer;
            DI<T>.allowChangingSingleton = allowChangingSingleton;
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