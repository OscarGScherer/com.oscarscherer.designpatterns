using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace DesignPatterns
{
    public static class UITKEditor
    {
        private static StyleSheet _editorUSS;
        public static StyleSheet editorUSS => _editorUSS ??= AssetDatabase.LoadAssetAtPath<StyleSheet>(
            "Packages/com.oscarscherer.designpatterns/Editor/Style Sheets/EditorStyles.uss");
        
        public static Button Button(string labelText, Action clickEvent) => UITKHelpers.Button(labelText, clickEvent).WithClasses("default-button");
        public static VisualElement HorizontalLine() => new VisualElement().WithClasses("horizontal-line");
        public static TextField TextField(string label, Length labelWidth)
        {
            var textField = new TextField(label);
            textField.style.flexGrow = 1;
            textField[0].style.width = labelWidth;
            textField[0].style.minWidth = 0;
            textField[1][0].style.width = 0;
            textField[1][0].style.minWidth = 0;
            return textField;
        }
    }
}