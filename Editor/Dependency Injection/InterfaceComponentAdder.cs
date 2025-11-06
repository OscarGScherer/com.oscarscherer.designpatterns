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
        private GameObject selectedGO;
        private InterfaceSearchbar interfaceSearchbar = new();


        [MenuItem("Tools/Interface Component Adder")]
        public static void ShowWindow()
        {
            GetWindow<InterfaceComponentAdder>("Add Component By Interface");
        }

        private void OnGUI()
        {
            interfaceSearchbar.Draw();

            selectedGO = Selection.activeGameObject;

            if (interfaceSearchbar.matchingTypes.Count > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Click to add to a single selected gameobject:");

                int count = 0;
                foreach (var type in interfaceSearchbar.matchingTypes)
                {
                    if (count > 10)
                    {
                        EditorGUILayout.LabelField("...");
                        break;
                    }
                    count++;
                    if (GUILayout.Button(type.FullName))
                    {
                        if (selectedGO != null) Undo.AddComponent(selectedGO, type);
                        // Debug.Log($"Added {type.Name} to {selectedGO.name}");
                    }
                }
            }
        }
    }

    public class InterfaceSearchbar
    {
        private string interfaceName = "";

        private Vector2 scroll;
        public List<Type> matchingTypes = new List<Type>();
        public Type matchedInterface;
        private List<Type> matchingInterfaces = new();
        private string matchingInterfacesNames = "";
        private string matchingInterfacesFullNames = "";

        private static List<Type> allInterfaces = new();

        static InterfaceSearchbar()
        {
            if (allInterfaces == null || allInterfaces.Count == 0) allInterfaces = GetAllInterfaces();
        }

        private bool UpdateInterfaceSearch()
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
                else matchingTypes = TypeFinder.GetAllDerivedTypes(matchedInterface).Where(t => typeof(MonoBehaviour).IsAssignableFrom(t)).ToList();
                return true;
            }
            return false;
        }

        public bool Draw()
        {
            bool matchedInterfacedChanged = false;

            EditorGUI.BeginChangeCheck();
            GUI.SetNextControlName("InterfaceaName");
            interfaceName = EditorGUILayout.TextField("Interface Name", interfaceName);
            if (EditorGUI.EndChangeCheck()) matchedInterfacedChanged = UpdateInterfaceSearch();

            Event e = Event.current;
            if (GUI.GetNameOfFocusedControl() == "InterfaceaName" && e.type == EventType.KeyDown && e.keyCode == KeyCode.Tab)
            {
                interfaceName = matchingInterfaces.Count > 0 ? matchingInterfaces[0].Name : interfaceName;
                UpdateInterfaceSearch();
            }

            GUIStyle textArea = new GUIStyle(GUI.skin.label) { wordWrap = true, alignment = TextAnchor.UpperRight, padding = new RectOffset(2, 2, 2, 2) };
            textArea.normal = new GUIStyleState() { textColor = Color.white };
            textArea.richText = true;

            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.MaxHeight(Mathf.Min(matchingInterfaces.Count * EditorGUIUtility.singleLineHeight, 100)));
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"<i>{matchingInterfacesFullNames}</i>", textArea);
            textArea.alignment = TextAnchor.UpperLeft;
            EditorGUILayout.LabelField($"<b>{matchingInterfacesNames}</b>", textArea);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();

            return matchedInterfacedChanged;
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
            if (interfaceName == "") return new List<Type>(0);
            List<Type> matches = interfaces
                .Where(i => i.Name.StartsWith(interfaceName))
                .OrderBy(i => i.Name)
                .ToList();
            return matches.GetRange(0, Mathf.Min(matches.Count, maxMatches));
        }
    }
}
