#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace DesignPatterns.StateMachine
{
    [CustomEditor(typeof(StateMachine), true)]
    public class StateMachineEditor : Editor
    {
        private static bool showSelectedNodeFields = false, showSelectedNodeProperties = false;

        public override void OnInspectorGUI() 
        {
            DrawDefaultInspector();

            CustomEditorUtilities.DrawUILine(Color.gray, 0, 10);
            GUIStyle boldStyle = new GUIStyle(GUI.skin.label);
            boldStyle.fontStyle = FontStyle.Bold;
            boldStyle.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.LabelField("State machine viewer", boldStyle);
            CustomEditorUtilities.DrawUILine(Color.gray, 1, 0);

            StateMachine sm = (StateMachine) target;

            if(sm.currentStateObject == null)
            {
                GUILayout.Label("Current state is null");
                return;
            }
            
            GUIStyle stateNameStyle = new GUIStyle(GUI.skin.label){ fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };

            GUIStyle textArea = new GUIStyle(GUI.skin.textArea){ wordWrap = true, alignment = TextAnchor.UpperLeft, padding = new RectOffset(20,0,5,10) };
            textArea.normal = new GUIStyleState(){ textColor = Color.white };

            Type stateType = sm.currentStateObject.GetType();
            EditorGUILayout.LabelField("Current state: " + stateType.Name, stateNameStyle);

            showSelectedNodeFields = CustomEditorUtilities.DisplayTargetFieldsWithReflection(sm.currentStateObject, showSelectedNodeFields);
            // showSelectedNodeProperties = CustomEditorUtilities.DisplayTargetPropertiesWithReflection(sm.currentStateObject, showSelectedNodeProperties);

            EditorGUILayout.LabelField(sm.currentStateObject.ToDebugString(), textArea);
        }
    }
}
#endif
