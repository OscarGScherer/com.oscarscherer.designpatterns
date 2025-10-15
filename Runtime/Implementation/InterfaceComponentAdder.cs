using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DesignPatterns
{
    public class InterfaceComponentAdder : EditorWindow
    {
        private string interfaceName = "";

        private Vector2 scroll;
        private List<Type> matchingTypes = new List<Type>();
        private Type matchedInterface;
        private List<Type> matchingInterfaces = new();
        private string matchingInterfacesNames = "";
        private string matchingInterfacesFullNames = "";
        private GameObject selectedGO;

        private static List<Type> allInterfaces = new();

        static InterfaceComponentAdder()
        {
            if (allInterfaces == null || allInterfaces.Count == 0) allInterfaces = GetAllInterfaces();
        }

        [MenuItem("Tools/Interface Component Adder")]
        public static void ShowWindow()
        {
            GetWindow<InterfaceComponentAdder>("Add Component By Interface");
        }

        private void UpdateInterfaceSearch()
        {
            matchingInterfaces = GetMatchingInterfaces(interfaceName, allInterfaces, 20);

            matchingInterfacesNames = String.Join("\n", matchingInterfaces.Select(i =>
                "<color=\"cyan\">" + i.Name.Insert(interfaceName.Length, "</color>")
            ));
            matchingInterfacesFullNames = String.Join("\n", matchingInterfaces.Select(i => i.FullName));

            Type topMatch = matchingInterfaces.Count > 0 ? matchingInterfaces[0] : null;
            if (topMatch != matchedInterface)
            {
                matchedInterface = topMatch;
                if (matchedInterface == null) matchingTypes = new List<Type>();
                else matchingTypes = TypeFinder.GetAllDerivedTypes(matchedInterface, t => typeof(MonoBehaviour).IsAssignableFrom(t));
            }
        }

        private void OnGUI()
        {
            selectedGO = Selection.activeGameObject;
            EditorGUI.BeginChangeCheck();
            interfaceName = EditorGUILayout.TextField("Interface Name", interfaceName);
            if (EditorGUI.EndChangeCheck()) UpdateInterfaceSearch();
            
            Event e = Event.current;
            if(hasFocus && e.type == EventType.KeyDown && e.keyCode == KeyCode.Tab)
            {
                interfaceName = matchingInterfaces.Count > 0 ? matchingInterfaces[0].Name : interfaceName;
                UpdateInterfaceSearch();
            }

            GUIStyle textArea = new GUIStyle(GUI.skin.label) { wordWrap = true, alignment = TextAnchor.UpperRight, padding = new RectOffset(2, 2, 2, 2) };
            textArea.normal = new GUIStyleState() { textColor = Color.white };
            textArea.richText = true;

            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.MaxHeight(Mathf.Min(matchingInterfaces.Count*EditorGUIUtility.singleLineHeight, 100)));
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"<i>{matchingInterfacesFullNames}</i>", textArea);
            textArea.alignment = TextAnchor.UpperLeft;
            EditorGUILayout.LabelField($"<b>{matchingInterfacesNames}</b>", textArea);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();

            if (matchingTypes.Count > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Click to add to a single selected gameobject:");
                scroll = EditorGUILayout.BeginScrollView(scroll);

                int count = 0;
                foreach (var type in matchingTypes)
                {
                    if (count > 10)
                    {
                        EditorGUILayout.LabelField("...");
                        break;
                    }
                    count++;
                    if (GUILayout.Button(type.FullName))
                    {
                        if(selectedGO != null) Undo.AddComponent(selectedGO, type);
                        // Debug.Log($"Added {type.Name} to {selectedGO.name}");
                    }
                }

                EditorGUILayout.EndScrollView();
            }
        }

        private static List<Type> GetAllInterfaces()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly =>
                {
                    try
                    {
                        return assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException e)
                    {
                        return e.Types.Where(t => t != null);
                    }
                })
                .Where(t => t.IsInterface)
                .ToList();
        }

        private static List<Type> GetMatchingInterfaces(string interfaceName, List<Type> interfaces, int maxMatches = 30)
        {
            List<Type> matches = interfaces
                .Where(i=>i.Name.StartsWith(interfaceName))
                .OrderBy(i=>i.Name)
                .ToList();
            return matches.GetRange(0, Mathf.Min(matches.Count, maxMatches));
        }
    }
}
