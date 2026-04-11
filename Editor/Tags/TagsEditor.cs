#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DesignPatterns
{
    [CustomEditor(typeof(Tags))]
    public class TagsEditor : Editor
    {
        string tag = "";
        
        // public override void OnInspectorGUI()
        // {
        //     Tags tags = (Tags)target;

        //     GUIStyle title = new GUIStyle(EditorStyles.label);
        //     title.fontSize = 12;
        //     title.richText = true;
        //     GUILayout.Label("<b>Tags</b>", title);
        //     CustomEditorUtilities.DrawUILine(Color.gray, 1, 5);
        //     GUILayout.Space(5);

        //     GUILayout.BeginHorizontal();

        //     tag = EditorGUILayout.TextArea(tag, GUILayout.MinHeight(10), GUILayout.ExpandWidth(true));

        //     if (GUILayout.Button("Add", GUILayout.MaxWidth(60)))
        //     {
        //         tags.AddTag(tag);
        //     }
        //     if (GUILayout.Button("Remove", GUILayout.MaxWidth(60)))
        //     {
        //         tags.RemoveTag(tag);
        //     }

        //     GUILayout.EndHorizontal();

        //     List<string> tagList = tags.GetTagsCopy();
        //     string tagString = string.Join("] [", tagList.Select(tag => $"<color=\"cyan\"><b><u>{tag}</u></b></color>"));
        //     tagString = tagString.Length > 0 ? "[" + tagString + "]" : "<i>This object has no tags...</i>";
        //     GUIStyle tagsStyle = new GUIStyle(EditorStyles.label);
        //     //tagsStyle.fontSize = 14;
        //     tagsStyle.richText = true;
        //     tagsStyle.wordWrap = true;

        //     GUILayout.Space(10);
        //     GUILayout.Label(tagString, tagsStyle);
        //     GUILayout.Space(10);

        //     if (GUILayout.Button("Remove All", GUILayout.MaxWidth(100)))
        //     {
        //         tagList.ForEach(tag => tags.RemoveTag(tag));
        //     }
        // }

        public override VisualElement CreateInspectorGUI()
        {
            Tags tags = (Tags)target;
            var root = new VisualElement();

            root.styleSheets.Add(UITKEditor.editorUSS);

            var title = new Label("Tags").WithParent(root);

            root.Add(UITKEditor.HorizontalLine());

            var inputLine = UITKHelpers.FlexContainer(FlexDirection.Row).WithParent(root);

            var tagField = UITKEditor.TextField("Tag", 40).WithFlex(5).WithParent(inputLine);

            var tagListText = new Label();
            var addButton = UITKEditor.Button("Add", AddTag).WithParent(inputLine);
            var removeButton = UITKEditor.Button("Remove", RemoveTag).WithParent(inputLine);
            root.Add(tagListText);
            UpdateTagList();

            void UpdateTagList()
            {
                List<string> tagList = tags.GetTagsCopy();
                string tagString = string.Join("] [", tagList.Select(tag => $"<color=\"cyan\"><b><u>{tag}</u></b></color>"));
                tagString = tagString.Length > 0 ? "[" + tagString + "]" : "<i>This object has no tags...</i>";
                tagListText.text = tagString;
            }
            void AddTag() { tags.AddTag(tagField.text); UpdateTagList(); }
            void RemoveTag() { tags.RemoveTag(tagField.text); UpdateTagList(); }

            return root;
        }
    }
}
#endif