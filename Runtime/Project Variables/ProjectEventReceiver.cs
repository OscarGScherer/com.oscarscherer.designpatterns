using DesignPatterns;
using UnityEngine;
using UnityEngine.Events;

public class ProjectEventListener : MonoBehaviour
{
    public UnityEvent unityEvent;
    public ProjectEvent projectEvent;

    void OnEnable()
    {
        projectEvent?.Register(this);
    }

    void OnDisable()
    {
        projectEvent?.Unregister(this);
    }

    public void Raise() => unityEvent?.Invoke();
}