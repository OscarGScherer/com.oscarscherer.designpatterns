#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DesignPatterns.DependencyInjection
{
    [CustomEditor(typeof(InterfaceBindings))]
    public class InterfaceBindingsEditor : Editor
    {
        private InterfaceSearchbar interfaceSearchbar = new();

        public override void OnInspectorGUI()
        {
            InterfaceBindings ibs = (InterfaceBindings)target;

            EditorGUILayout.BeginVertical(GUI.skin.box);
            // Searching for interfaces
            interfaceSearchbar.Draw();

            bool wasEdited = false;

            SerializedProperty bindingsSP = serializedObject.FindProperty("_bindings");
            while (bindingsSP.arraySize < Enum.GetValues(typeof(Scope)).Length)
            {
                bindingsSP.InsertArrayElementAtIndex(bindingsSP.arraySize);
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Add as:", GUILayout.MaxWidth(50));
            (bool clicked, Scope scope) = DrawEnumButtons<Scope>();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            CustomEditorUtilities.DrawUILine(Color.gray);

            // Adding new interfaces
            if (clicked)
            {
                SerializedProperty selectedBindsSP = GetBindingList(bindingsSP, scope);

                Type iType = interfaceSearchbar.matchedInterface;
                if (iType != null && !BindingsContainsInterface(selectedBindsSP, iType))
                {
                    selectedBindsSP.InsertArrayElementAtIndex(selectedBindsSP.arraySize);
                    SerializedProperty newBinding = selectedBindsSP.GetArrayElementAtIndex(selectedBindsSP.arraySize - 1);
                    newBinding.FindPropertyRelative("interfaceAQN").stringValue = iType.AssemblyQualifiedName;
                    wasEdited = true;
                }
            }

            // Drawing all interfaces
            EditorGUILayout.LabelField("Singletons", EditorStyles.whiteBoldLabel);
            wasEdited |= DisplayBindings(GetBindingList(bindingsSP, Scope.Singleton), true, ProjectDependencyInjector.scopeFilters[(int)Scope.Singleton]);

            EditorGUILayout.LabelField("Locals", EditorStyles.whiteBoldLabel);
            wasEdited |= DisplayBindings(GetBindingList(bindingsSP, Scope.Local), false, ProjectDependencyInjector.scopeFilters[(int)Scope.Local]);

            if (wasEdited) serializedObject.ApplyModifiedProperties();
        }

        static SerializedProperty GetBindingList(SerializedProperty bindingsSP, Scope scope)
        {
            return bindingsSP.GetArrayElementAtIndex((int)scope).FindPropertyRelative("list");
        }

        static (bool, E) DrawEnumButtons<E>() where E : Enum
        {
            bool clicked = false;
            E value = default;
            foreach (E curr in Enum.GetValues(typeof(E)))
            {
                if (GUILayout.Button(curr.ToString()))
                {
                    value = curr;
                    clicked = true;
                }
            }
            return (clicked, value);
        }

        static bool BindingsContainsInterface(SerializedProperty bindingsSP, Type interfaceType)
        {
            string interfaceAQN = interfaceType.AssemblyQualifiedName;
            for (int i = 0; i < bindingsSP.arraySize; i++)
            {
                string bindingAQN = bindingsSP.GetArrayElementAtIndex(i).FindPropertyRelative("interfaceAQN").stringValue;
                if (bindingAQN == interfaceAQN) return true;
            }
            return false;
        }

        static bool DisplayBindings(SerializedProperty bindingsSP, bool displayDefault, Func<Type, bool> filter)
        {
            EditorGUILayout.BeginVertical(GUI.skin.window);

            bool wasEdited = false;
            for (int i = 0; i < bindingsSP.arraySize; i++)
            {
                if (i > 0) CustomEditorUtilities.DrawUILine(new Color().Hex("#505050ff"));

                SerializedProperty bind = bindingsSP.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                if (GUILayout.Button("-", GUILayout.MaxWidth(25)))
                {
                    bindingsSP.DeleteArrayElementAtIndex(i);
                    i--;
                    wasEdited = true;
                    EditorGUILayout.EndHorizontal();
                    continue;
                }

                string interfaceAQN = bind.FindPropertyRelative("interfaceAQN").stringValue;

                Type interfaceType = InterfaceBinding.AQNToType(interfaceAQN);

                if (interfaceType == null)
                {
                    EditorGUILayout.LabelField($"Missing interface: {interfaceAQN}");
                    EditorGUILayout.EndHorizontal();
                    continue;
                }

                EditorGUILayout.LabelField(interfaceType.Name, EditorStyles.boldLabel);

                Type[] concreteTypes = TypeFinder.GetAllDerivedTypes(interfaceType)
                    .Where(t => filter(t))
                    .ToArray();

                string[] concreteNames = concreteTypes.Select(t => t.Name).ToArray();
                string[] concreteAQNs = concreteTypes.Select(t => t.AssemblyQualifiedName).ToArray();
                wasEdited |= CustomEditorUtilities.DropDown("", bind.FindPropertyRelative("concreteAQN"), concreteNames, concreteAQNs, out int selected);

                EditorGUILayout.EndHorizontal();

                if (!displayDefault) continue;

                SerializedProperty defaultValueSP = bind.FindPropertyRelative("defaultValue");
                if (wasEdited)
                {
                    object newInstance = Activator.CreateInstance(concreteTypes[selected]);
                    defaultValueSP.managedReferenceValue = newInstance;
                }

                EditorGUI.BeginChangeCheck();
                int indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(defaultValueSP, true);
                EditorGUI.indentLevel = indent;
                wasEdited |= EditorGUI.EndChangeCheck();
            }

            EditorGUILayout.EndVertical();

            return wasEdited;
        }
    }
}
#endif