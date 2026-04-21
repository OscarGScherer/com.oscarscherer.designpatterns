#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DesignPatterns
{
    [CustomPropertyDrawer(typeof(EnumArray<,>), true)]
    public class EnumArrayPropertyDrawer : PropertyDrawer
    {
        private enum PlaceHolder {}

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();

            var title = new Label(property.name).WithParent(root);

            var values = property.FindPropertyRelative(EnumArray<PlaceHolder,int>.VALUES_FIELD_NAME);
            if (values == null) return root; // No values were serialized

            Type baseType = fieldInfo.FieldType.BaseType;
            while (baseType != null && (!baseType.IsGenericType || baseType.GetGenericTypeDefinition() != typeof(EnumArray<,>))) 
                baseType = baseType.BaseType;
            if (baseType == null || !baseType.IsGenericType) 
                return root;

            Type enumType = baseType.GetGenericArguments()[0];
            
            var names = Enum.GetNames(enumType);
            foreach(var name in names)
            {
                var prop = new PropertyField(values.GetArrayElementAtIndex((int)Enum.Parse(enumType, name)), name).WithParent(root);
            }

            return root;
        }
    }
}
#endif