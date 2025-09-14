using UnityEditor;
using UnityEngine;

namespace DesignPatterns.ProjectMediators
{
    public class ProjectEventEditor<T> : Editor
    {
        public override void OnInspectorGUI()
        {
            // DrawDefaultInspector();
            ProjectEvent<T> pe = (ProjectEvent<T>)target;

            SerializedProperty prop = serializedObject.FindProperty("parameter");
            if (prop != null)
            {
                if (prop.type == "string")
                {
                    EditorGUILayout.LabelField("Parameter");
                    prop.stringValue = EditorGUILayout.TextArea(prop.stringValue, GUILayout.ExpandHeight(true), GUILayout.MinHeight(50));
                }
                else EditorGUILayout.PropertyField(prop);
            }
            else EditorGUILayout.LabelField("Unsuported parameter type");

            if (GUILayout.Button("Raise"))
            {
                pe.Raise(pe.parameter);
            }
        }
    }
    // STRING
    [CustomEditor(typeof(ProjectEvent<string>), true)]
    public class StringProjectEventEditor : ProjectEventEditor<string> { }
    // FLOAT
    [CustomEditor(typeof(ProjectEvent<float>), true)]
    public class FloatProjectEventEditor : ProjectEventEditor<float> { }
    // Int
    [CustomEditor(typeof(ProjectEvent<int>), true)]
    public class IntProjectEventEditor : ProjectEventEditor<int> { }
    // BOOL
    [CustomEditor(typeof(ProjectEvent<bool>), true)]
    public class BoolProjectEventEditor : ProjectEventEditor<bool> { }
    // VECTOR3
    [CustomEditor(typeof(ProjectEvent<Vector3>), true)]
    public class Vector3ProjectEventEditor : ProjectEventEditor<Vector3> {}
}