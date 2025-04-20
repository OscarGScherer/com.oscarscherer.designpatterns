#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DesignPatterns
{
    public static class CustomEditorUtilities
    {
        public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
            r.height = thickness;
            r.y+=padding/2;
            r.x-=2;
            r.width +=6;
            EditorGUI.DrawRect(r, color);
        }

        public static bool DisplayTargetFieldsWithReflection(object target, bool show)
        {
            show = EditorGUILayout.Foldout(show, "Show fields");
            if(!show) return show;

            GUIStyle nameStyle = new GUIStyle(GUI.skin.label) { padding = new RectOffset(15,0,0,0), wordWrap = true, alignment = TextAnchor.MiddleLeft };
            GUIStyle valueStyle = new GUIStyle(GUI.skin.label) { wordWrap = true, alignment = TextAnchor.MiddleLeft };

            FieldInfo[] fields = target.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (field.IsDefined(typeof(HideInInspector), true)) continue;
                object fieldValue = field.GetValue(target);

                EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(field.Name, nameStyle);
                    EditorGUILayout.LabelField(fieldValue?.ToString() ?? "null", valueStyle);
                EditorGUILayout.EndHorizontal();
            }
            return show;
        }

        public static bool DisplayTargetPropertiesWithReflection(object target, bool show)
        {
            show = EditorGUILayout.Foldout(show, "Show properties");
            if(!show) return show;

            GUIStyle nameStyle = new GUIStyle(GUI.skin.label) { padding = new RectOffset(15,0,0,0), wordWrap = true, alignment = TextAnchor.MiddleLeft };
            GUIStyle valueStyle = new GUIStyle(GUI.skin.label) { wordWrap = true, alignment = TextAnchor.MiddleLeft };

            GUIStyle errorStyle = new GUIStyle(GUI.skin.label) 
                { padding = new RectOffset(15,0,0,0), normal = new GUIStyleState() { textColor = Color.red }};

            PropertyInfo[] properties = target.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var property in properties)
            {
                try {
                    object propertyValue = property.GetValue(target);
                    EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(property.Name, nameStyle);
                        EditorGUILayout.LabelField(propertyValue?.ToString() ?? "null", valueStyle);
                    EditorGUILayout.EndHorizontal();
                }
                catch (Exception e) {
                    EditorGUILayout.LabelField(property.Name, "Error reading property: " + e.Message, errorStyle);
                }
            }
            return show;
        } 
    }
}
#endif
