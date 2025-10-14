#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace DesignPatterns
{
    [CustomPropertyDrawer(typeof(Implementation<>), true)]
    public class ImplementationPropertyDrawer : PropertyDrawer
    {
        private bool wrongType = false;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty componentProp = property.FindPropertyRelative("component");

            Implementation ir = property.boxedValue as Implementation;
            label.text += " [" + ir.Expects().Name + "]";

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();

            UnityEngine.Object oldRef = componentProp.objectReferenceValue;
            UnityEngine.Object newRef = EditorGUI.ObjectField(position, label, oldRef, typeof(Component), true);

            if (EditorGUI.EndChangeCheck())
            {
                Component newComponent = newRef as Component;
                if (newComponent != null && !ir.Accepts(newComponent))
                {
                    wrongType = true;
                    Debug.LogWarning("<color=\"yellow\">Component needs to be a " + ir.Expects() + "</color>");
                }
                else
                {
                    wrongType = false;
                    componentProp.objectReferenceValue = newComponent;
                }
            }

            if (wrongType)
            {
                GUIStyle style = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleRight };
                style.richText = true;
                EditorGUILayout.LabelField($"<color=\"yellow\">Component needs to implement {ir.Expects().Name}</color>", style);
            }

            EditorGUI.EndProperty();
        }
    }
}
#endif