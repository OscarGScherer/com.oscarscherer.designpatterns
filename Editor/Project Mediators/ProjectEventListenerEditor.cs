using UnityEditor;
using UnityEngine;

namespace DesignPatterns.ProjectMediators
{
    public class ProjectEventListenerEditor<T> : Editor
    {
        void OnEnable()
        {
            ProjectEventListener<T> pel = (ProjectEventListener<T>)target;
            pel.projectEvent?.Register(pel);
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ProjectEventListener<T> pel = (ProjectEventListener<T>)target;
            pel.projectEvent = (ProjectEvent<T>)EditorGUILayout.ObjectField("Event", pel.projectEvent, typeof(ProjectEvent<T>), false);
        }
    }
    // STRING
    [CustomEditor(typeof(ProjectEventListener<string>), true)]
    public class StringProjectEventListenerEditor : ProjectEventListenerEditor<string> { }
    // FLOAT
    [CustomEditor(typeof(ProjectEventListener<float>), true)]
    public class FloatProjectEventListenerEditor : ProjectEventListenerEditor<float> { }
    // INT
    [CustomEditor(typeof(ProjectEventListener<int>), true)]
    public class IntProjectEventListenerEditor : ProjectEventListenerEditor<int> { }
    // BOOL
    [CustomEditor(typeof(ProjectEventListener<bool>), true)]
    public class BoolProjectEventListenerEditor : ProjectEventListenerEditor<bool> { }
    // VECTOR3
    [CustomEditor(typeof(ProjectEventListener<Vector3>), true)]
    public class Vector3ProjectEventListenerEditor : ProjectEventListenerEditor<Vector3> {}
}