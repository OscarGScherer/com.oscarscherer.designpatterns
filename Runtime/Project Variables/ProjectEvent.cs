using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DesignPatterns
{
    [CreateAssetMenu(menuName = "Project Event")]
    public class ProjectEvent : ScriptableObject
    {
        private List<ProjectEventListener> listeners = new List<ProjectEventListener>();

        public void Raise()
        {
            foreach (ProjectEventListener listener in listeners) listener.Raise();
        }
        public void Register(ProjectEventListener listener) => listeners.Add(listener);
        public void Unregister(ProjectEventListener listener) => listeners.Remove(listener);
    }
}
