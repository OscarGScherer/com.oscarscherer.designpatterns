#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace DesignPatterns
{
    [CustomPropertyDrawer(typeof(EnumData<>), true)]
    public class EnumDataPropertyDrawer : PropertyDrawer
    {
        private bool showData = false;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            showData = EditorGUI.Foldout(foldoutRect, showData, property.name);
            if (!showData) return;

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = indent + 1;

            SerializedProperty _metaData = property.FindPropertyRelative("_metaData");

            string enumAQN = property.FindPropertyRelative("_enumAssemblyQualifiedName").stringValue;
            Type enumType = enumAQN == null ? null : Type.GetType(enumAQN);
            string[] enumNames = Enum.GetNames(enumType);

            float currY = foldoutRect.y + EditorGUIUtility.singleLineHeight;
            for (int i = 0; i < _metaData.arraySize; i++)
            {
                float thirdWidth = position.width / 3f;
                Rect enumRect = new Rect(position.x, currY, thirdWidth, EditorGUIUtility.singleLineHeight);
                Rect labelRect = new Rect(position.x + thirdWidth, currY, thirdWidth, EditorGUIUtility.singleLineHeight);
                Rect valueRect = new Rect(position.x + 2 * thirdWidth, currY, thirdWidth, EditorGUIUtility.singleLineHeight);

                EditorGUILayout.BeginHorizontal();
                EditorGUI.LabelField(enumRect, enumNames[i]);
                EditorGUI.LabelField(labelRect, _metaData.GetArrayElementAtIndex(i).FindPropertyRelative("shortTypeName").stringValue);
                EditorGUI.LabelField(valueRect, _metaData.GetArrayElementAtIndex(i).FindPropertyRelative("stringData").stringValue);
                EditorGUILayout.EndHorizontal();

                currY += EditorGUIUtility.singleLineHeight;
            }

            EditorGUI.indentLevel = indent;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            if (showData) height *= 1 + property.FindPropertyRelative("_metaData").arraySize;
            return height;
        }
    }
}
#endif