#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DesignPatterns
{
    [CustomEditor(typeof(Tags))]
    public class TagsEditor : Editor
    {
        string tag = "";

        public override void OnInspectorGUI()
        {
            Tags tags = (Tags)target;

            GUIStyle title = new GUIStyle(EditorStyles.label);
            title.fontSize = 12;
            title.richText = true;
            GUILayout.Label("<b>Tags</b>", title);
            CustomEditorUtilities.DrawUILine(Color.gray, 1, 5);
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();

            tag = EditorGUILayout.TextArea(tag, GUILayout.MinHeight(10), GUILayout.ExpandWidth(true));

            if (GUILayout.Button("Add", GUILayout.MaxWidth(60)))
            {
                tags.AddTag(tag);
            }
            if (GUILayout.Button("Remove", GUILayout.MaxWidth(60)))
            {
                tags.RemoveTag(tag);
            }

            GUILayout.EndHorizontal();

            List<string> tagList = tags.GetTagsCopy();
            string tagString = string.Join("] [", tagList.Select(tag => $"<color=\"cyan\"><b><u>{tag}</u></b></color>"));
            tagString = tagString.Length > 0 ? "[" + tagString + "]" : "<i>This object has no tags...</i>";
            GUIStyle tagsStyle = new GUIStyle(EditorStyles.label);
            //tagsStyle.fontSize = 14;
            tagsStyle.richText = true;
            tagsStyle.wordWrap = true;

            GUILayout.Space(10);
            GUILayout.Label(tagString, tagsStyle);
            GUILayout.Space(10);

            if (GUILayout.Button("Remove All", GUILayout.MaxWidth(100)))
            {
                tagList.ForEach(tag => tags.RemoveTag(tag));
            }
        }
    }
}
#endif