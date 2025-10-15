using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DesignPatterns
{
    public class InterfaceComponentAdder : EditorWindow
    {
        private string interfaceName = "ICustomBehaviour";
        private Vector2 scroll;
        private List<Type> matchingTypes = new List<Type>();
        private GameObject selectedGO;

        [MenuItem("Tools/Interface Component Adder")]
        public static void ShowWindow()
        {
            GetWindow<InterfaceComponentAdder>("Add Interface Component");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Interface Component Adder", EditorStyles.boldLabel);

            selectedGO = Selection.activeGameObject;
            if (selectedGO == null)
            {
                EditorGUILayout.HelpBox("Please select a GameObject in the Hierarchy.", MessageType.Warning);
                return;
            }

            interfaceName = EditorGUILayout.TextField("Interface Name", interfaceName);

            if (GUILayout.Button("Search Components"))
            {
                FindTypesImplementingInterface(interfaceName);
            }

            if (matchingTypes.Count > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Matching Components:");
                scroll = EditorGUILayout.BeginScrollView(scroll);

                foreach (var type in matchingTypes)
                {
                    if (GUILayout.Button(type.FullName))
                    {
                        Undo.AddComponent(selectedGO, type);
                        Debug.Log($"Added {type.Name} to {selectedGO.name}");
                    }
                }

                EditorGUILayout.EndScrollView();
            }
        }

        private void FindTypesImplementingInterface(string interfaceName)
        {
            matchingTypes.Clear();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var allTypes = assemblies.SelectMany(asm => asm.GetTypes());

            Type targetInterface = allTypes.FirstOrDefault(t =>
                t.IsInterface && t.Name.Equals(interfaceName, StringComparison.OrdinalIgnoreCase));

            if (targetInterface == null)
            {
                Debug.LogWarning($"Interface '{interfaceName}' not found.");
                return;
            }

            foreach (var type in allTypes)
            {
                if (type.IsClass &&
                    !type.IsAbstract &&
                    typeof(MonoBehaviour).IsAssignableFrom(type) &&
                    targetInterface.IsAssignableFrom(type))
                {
                    matchingTypes.Add(type);
                }
            }
        }
    }
}
