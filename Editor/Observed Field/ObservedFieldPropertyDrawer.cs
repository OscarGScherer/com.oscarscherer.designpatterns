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
        bool isSerializeReference = property.propertyType == SerializedPropertyType.ManagedReference;
        if (isSerializeReference && property.managedReferenceValue == null)
        {
            property.managedReferenceValue = Activator.CreateInstance(fieldInfo.FieldType);
            property.serializedObject.ApplyModifiedProperties();
        }

        var valueProp = property.FindPropertyRelative("_value");
        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.BeginChangeCheck();

        Rect propertyRect = new Rect(position.x, position.y, position.width, EditorGUI.GetPropertyHeight(valueProp, label, true));
        EditorGUI.PropertyField(propertyRect, valueProp, label, true);

        if (EditorGUI.EndChangeCheck())
        {
            property.serializedObject.ApplyModifiedProperties();
            if (isSerializeReference)
            {   
                MethodInfo OnChange = fieldInfo.FieldType.BaseType.GetMethod("OnChange", BindingFlags.NonPublic | BindingFlags.Instance);
                OnChange?.Invoke(property.managedReferenceValue, null);
            }
        }

        if (!isSerializeReference)
        {
            Rect helpBoxRect = new Rect(
                position.x, propertyRect.yMax,
                position.width, EditorGUIUtility.singleLineHeight
            );
            EditorGUI.HelpBox(helpBoxRect, $"{property.name}'s change events will only be called in the editor if {property.name} has the [SerializeReference] attribute.", MessageType.Info);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float totalHeight = EditorGUIUtility.singleLineHeight;
        if (property.propertyType != SerializedPropertyType.ManagedReference) totalHeight += EditorGUIUtility.singleLineHeight;
        return totalHeight;
    }
}

#endif