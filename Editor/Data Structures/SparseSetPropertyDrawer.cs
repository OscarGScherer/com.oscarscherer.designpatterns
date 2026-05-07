#if UNITY_EDITOR
using Unity.Properties;
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
        public const string COUNT = "_count";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();

            root.Add(new Label(property.displayName));

            var countField = new PropertyField(property.FindPropertyRelative(COUNT)).WithParent(root);
            countField.SetEnabled(false);

            var horizontal = UITKHelpers.FlexContainer(FlexDirection.Row).WithParent(root);
            
            var dataField = new PropertyField(property.FindPropertyRelative(DATA));
            dataField.SetEnabled(false);
            dataField.style.flexGrow = 1;
            dataField.style.marginRight = 20;
            horizontal.Add(dataField);

            var idField = new PropertyField(property.FindPropertyRelative(INDEX_TO_ID));
            idField.SetEnabled(false);
            idField.style.flexGrow = 1;
            dataField.style.marginLeft = 20;
            horizontal.Add(idField);

            return root;
        }
    }
}
#endif