#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Linq;
using UnityEditor;

namespace DesignPatterns
{
    public class MonoBehaviourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Type type = target.GetType();
            DisplayInterfacesAttribute displayInterfacesAttribute = GetAttribute<DisplayInterfacesAttribute>(type);
            if (displayInterfacesAttribute != null) DisplayInterfacesOf(type);
            base.OnInspectorGUI();
        }

        private static void DisplayInterfacesOf(Type type)
        {
            GUIStyle textArea = new GUIStyle(GUI.skin.label) { wordWrap = true, alignment = TextAnchor.UpperRight, padding = new RectOffset(2, 2, 2, 2) };
            textArea.normal = new GUIStyleState() { textColor = Color.white };
            textArea.richText = true;

            Type[] interfaces = type.GetInterfaces();
            if (interfaces == null || interfaces.Count() == 0) return;
            string label = "";
            for (int i = 0; i < interfaces.Length; i++)
            {
                string color = GetAttribute<ColorAttribute>(interfaces[i])?.color ?? "white";
                label += $"[<color=\"{color}\">{interfaces[i].Name}</color>]";
            }
            EditorGUILayout.LabelField(label, textArea);
        }
        
        private static T GetAttribute<T>(Type type) where T : Attribute => (T) Attribute.GetCustomAttribute(type, typeof(T));
    }
}
#endif
