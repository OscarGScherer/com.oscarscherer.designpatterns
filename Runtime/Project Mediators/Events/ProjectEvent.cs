using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DesignPatterns.ProjectMediators
{
    public class ProjectEvent<T> : ScriptableObject
    {
        public ProjectVariable<T> variable;
        public T parameter;
        private List<ProjectEventListener<T>> listeners = new List<ProjectEventListener<T>>();

        public void Raise(T parameter)
        {
            foreach (ProjectEventListener<T> listener in listeners)
            {
                listener.Raise(parameter);
            }
        }
        public void Register(ProjectEventListener<T> listener)
        {
            if (listeners.Contains(listener)) return;
            listeners.Add(listener);
        }
        public void Unregister(ProjectEventListener<T> listener) => listeners.Remove(listener);
    }
}
