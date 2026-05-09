#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Linq;
using UnityEditor;
using System.Reflection;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace DesignPatterns
{   
    public class CustomInspector : Editor
    {
        // public override void OnInspectorGUI()
        // {
        //     ShowInterfacesAttribute(target);
        //     ButtonAttribute(target);
        //     base.OnInspectorGUI();
        // }

        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();
            InspectorElement.FillDefaultInspector(container, serializedObject, this);
            AddButtonsAttribute(container, target);
            return container;
        }

        private static void AddButtonsAttribute(VisualElement container, UnityEngine.Object target)
        {
            var targetType = target.GetType();
            var methods = targetType
                .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttribute(typeof(ButtonAttribute), true) != null);

            int buttonsAdded = 0;
            foreach (var method in methods)
            {
                var buttonAttr = method.GetCustomAttribute<ButtonAttribute>();
                string label = buttonAttr.label ?? ObjectNames.NicifyVariableName(method.Name);
                var button = new Button(() => method.Invoke(target, null));
                button.Add(new Label(label));
                if (buttonAttr.fromTop) 
                    container.Insert(buttonsAdded + buttonAttr.editorOrder, button);
                else
                    container.Insert(container.childCount - buttonAttr.editorOrder - buttonsAdded, button);
                buttonsAdded++;
            }
        }

        // private static void ShowInterfacesAttribute(UnityEngine.Object target)
        // {
        //     Type type = target.GetType();
        //     DisplayInterfacesAttribute displayInterfacesAttribute = GetAttribute<DisplayInterfacesAttribute>(type);
        //     if (displayInterfacesAttribute == null) return;

        //     GUIStyle textArea = new GUIStyle(GUI.skin.label) { wordWrap = true, alignment = TextAnchor.UpperRight, padding = new RectOffset(2, 2, 2, 2) };
        //     textArea.normal = new GUIStyleState() { textColor = Color.white };
        //     textArea.richText = true;

        //     Type[] interfaces = type.GetInterfaces();
        //     if (interfaces == null || interfaces.Count() == 0) return;
        //     string label = "";
        //     for (int i = 0; i < interfaces.Length; i++)
        //     {
        //         string color = GetAttribute<ColorAttribute>(interfaces[i])?.color ?? "white";
        //         label += $"[<color=\"{color}\">{interfaces[i].Name}</color>]";
        //     }
        //     EditorGUILayout.LabelField(label, textArea);
        // }
        
        private static T GetAttribute<T>(Type type) where T : Attribute => (T) Attribute.GetCustomAttribute(type, typeof(T));
    }

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var prop = new PropertyField(property);
            prop.SetEnabled(false);
            return prop;
        }
    }
}
#endif
