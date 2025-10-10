#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace DesignPatterns.StateMachine
{
    [CustomEditor(typeof(StateMachine), true)]
    public class StateMachineEditor : Editor
    {
        private static bool showStateChangeHistory = false, showSelectedNodeFields = false, showSelectedNodeProperties = false;
        private Vector2 stateChangeHistoryScrollPos;

        StateMachine targetSM;

        private void OnEnable()
        {
            targetSM = (StateMachine)target;
            EditorApplication.update += CheckForRepaint;
        }

        private void OnDisable()
        {
            EditorApplication.update -= CheckForRepaint;
        }

        private void CheckForRepaint()
        {
            if (targetSM == null) return;
            if (!targetSM.refreshInspector) return;
            Repaint();
            targetSM.refreshInspector = false;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            CustomEditorUtilities.DrawUILine(Color.gray, 0, 5);

            GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.fontSize = 12;
            EditorGUILayout.LabelField("State machine viewer", titleStyle);
            CustomEditorUtilities.DrawUILine(Color.gray, 2, 0);

            if (targetSM.CurrentStateObject == null)
            {
                GUILayout.Label("Current state is null");
                return;
            }

            GUIStyle boldStyle = new GUIStyle(GUI.skin.label);
            boldStyle.fontStyle = FontStyle.Bold;

            GUIStyle stateNameStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            Type stateType = targetSM.CurrentStateObject.GetType();
            EditorGUILayout.LabelField("Current state -> " + stateType.Name, stateNameStyle);

            GUIStyle textArea = new GUIStyle(GUI.skin.textArea) { wordWrap = true, alignment = TextAnchor.UpperLeft, padding = new RectOffset(10, 0, 5, 10) };
            textArea.normal = new GUIStyleState() { textColor = Color.white };
            textArea.richText = true;

            EditorGUILayout.LabelField(targetSM.CurrentStateObject.ToDebugString(), textArea);

            showStateChangeHistory = EditorGUILayout.Foldout(showStateChangeHistory, "Show history");
            if (showStateChangeHistory)
            {
                GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
                boxStyle.padding = new RectOffset(10, 10, 10, 10);
                boxStyle.margin = new RectOffset(0, 0, 10, 10);

                stateChangeHistoryScrollPos = EditorGUILayout.BeginScrollView(stateChangeHistoryScrollPos, textArea, GUILayout.Height(100));

                LinkedListNode<State> state = targetSM.stateChangeHistory.First;
                for (int i = 0; i < targetSM.stateChangeHistory.Count; i++)
                {
                    if (i == 0) EditorGUILayout.LabelField($"> {state.Value.GetType().Name} (current)");
                    else EditorGUILayout.LabelField($"> {state.Value.GetType().Name}");
                    state = state.Next;
                }
                EditorGUILayout.EndScrollView();
            }

            showSelectedNodeFields = CustomEditorUtilities.DisplayTargetFieldsWithReflection(targetSM.CurrentStateObject, showSelectedNodeFields);
            // showSelectedNodeProperties = CustomEditorUtilities.DisplayTargetPropertiesWithReflection(targetSM.CurrentStateObject, showSelectedNodeProperties);
        }
    }
}
#endif
