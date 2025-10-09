#if UNITY_EDITOR

using System;
using System.Reflection;
using DesignPatterns;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ObservedField<>), true)]
public class ObservedFieldPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        bool isSerializeReference = property.propertyType == SerializedPropertyType.ObjectReference;

        if (isSerializeReference && property.managedReferenceValue == null)
        {
            property.managedReferenceValue = Activator.CreateInstance(fieldInfo.FieldType);
            property.serializedObject.ApplyModifiedProperties();
        }

        SerializedProperty valueProperty = property.FindPropertyRelative("_value");
        Rect valuePropertyRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        EditorGUI.BeginChangeCheck();
        EditorGUI.PropertyField(valuePropertyRect, valueProperty, label, true);

        if (EditorGUI.EndChangeCheck() && isSerializeReference)
        {
            MethodInfo onChange = typeof(ObservedField<>).GetMethod("OnChange", BindingFlags.NonPublic | BindingFlags.Instance);
            onChange.Invoke(property.managedReferenceValue, null);
        }

        if (!isSerializeReference)
        {
            Rect messageRect = new Rect(
                position.x, valuePropertyRect.yMax,
                position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.HelpBox(messageRect, $"In order for the inspector to call {property.name}'s events, it needs [SerializeReference].", MessageType.Info);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return property.propertyType == SerializedPropertyType.ObjectReference ?
            EditorGUIUtility.singleLineHeight :
            EditorGUIUtility.singleLineHeight * 2f;
    }
}

#endif