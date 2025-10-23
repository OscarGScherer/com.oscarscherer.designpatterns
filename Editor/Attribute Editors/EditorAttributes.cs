#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Linq;
using UnityEditor;
using System.Reflection;

namespace DesignPatterns
{
    public class EditorAttributes : Editor
    {
        public override void OnInspectorGUI()
        {
            ShowInterfacesAttribute(target);
            ButtonAttribute(target);
            base.OnInspectorGUI();
        }

        private static void ButtonAttribute(UnityEngine.Object target)
        {
            var targetType = target.GetType();
            var methods = targetType
                .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttributes(typeof(ButtonAttribute), true).Length > 0);

            foreach (var method in methods)
            {
                var buttonAttr = method.GetCustomAttribute<ButtonAttribute>();
                string label = buttonAttr.Label ?? ObjectNames.NicifyVariableName(method.Name);
                if (GUILayout.Button(label)) method.Invoke(target, null);
            }
        }

        private static void ShowInterfacesAttribute(UnityEngine.Object target)
        {
            Type type = target.GetType();
            DisplayInterfacesAttribute displayInterfacesAttribute = GetAttribute<DisplayInterfacesAttribute>(type);
            if (displayInterfacesAttribute == null) return;

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
