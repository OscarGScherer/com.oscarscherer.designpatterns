using DesignPatterns;
using UnityEngine;
using UnityEngine.Events;

namespace DesignPatterns.ProjectMediators
{
    public class ProjectEventListener<T> : MonoBehaviour
    {
        public UnityEvent<T> unityEvent;
        [SerializeField][HideInInspector] private ProjectEvent<T> _projectEvent;
        public ProjectEvent<T> projectEvent
        {
            get => _projectEvent;
            set
            {
                if (_projectEvent != value)
                {
                    _projectEvent?.Unregister(this);
                    value?.Register(this);
                }
                _projectEvent = value;
            }
        }

        void OnEnable()
        {
            projectEvent?.Register(this);
        }

        void OnDisable()
        {
            projectEvent?.Unregister(this);
        }

        public void Raise(T parameter)
        {
            unityEvent?.Invoke(parameter);
        }
    }
}