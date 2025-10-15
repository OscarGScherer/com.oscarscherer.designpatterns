#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Linq;
using UnityEditor;

namespace DesignPatterns
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class MonoBehaviourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Type type = target.GetType();
            Attribute displayInterfacesAttribute = Attribute.GetCustomAttribute(type, typeof(DisplayInterfacesAttribute));
            if (displayInterfacesAttribute != null)
                DisplayInterfacesOf(type);

            base.OnInspectorGUI();
        }
        
        private static void DisplayInterfacesOf(Type type)
        {
            GUIStyle textArea = new GUIStyle(GUI.skin.label) { wordWrap = true, alignment = TextAnchor.UpperRight, padding = new RectOffset(2, 2, 2, 2) };
            textArea.normal = new GUIStyleState() { textColor = Color.white };
            textArea.richText = true;

            Type[] interfaces = type.GetInterfaces();
            if (interfaces == null || interfaces.Count() == 0) return;
            string interfacesString = String.Join("</color>] - [<color=\"cyan\">", interfaces.Select(I => I.Name).ToArray());
            EditorGUILayout.LabelField($"[<color=\"cyan\">{interfacesString}</color>]", textArea);
        }
    }
}
#endif
