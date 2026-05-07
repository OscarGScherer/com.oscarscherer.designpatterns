#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace DesignPatterns
{
    [CustomPropertyDrawer(typeof(SparseSet<>))]
    public class SparseSetEditor : UnityEditor.PropertyDrawer
    {
        public const string DATA = "data";
        public const string ID_TO_INDEX = "id_to_index";
        public const string INDEX_TO_ID = "index_to_id";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Row;
            root.style.flexGrow = 1;
            
            var dataField = new PropertyField(property.FindPropertyRelative(DATA));
            dataField.SetEnabled(false);
            dataField.style.flexGrow = 1;
            dataField.style.marginRight = 20;
            root.Add(dataField);

            var idField = new PropertyField(property.FindPropertyRelative(INDEX_TO_ID));
            idField.SetEnabled(false);
            idField.style.flexGrow = 1;
            dataField.style.marginLeft = 20;
            root.Add(idField);

            return root;
        }
    }
}
#endif