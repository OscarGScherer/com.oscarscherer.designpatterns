#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace DesignPatterns
{
    [CustomEditor(typeof(BehaviourTree), true)]
    public class BehaviourTreeEditor : Editor
    {
        private static bool showSelectedNodeFields = false, showSelectedNodeProperties = false;
        private static List<(Rect,BehaviourTree.Node)> treeViewerNodes = new List<(Rect,BehaviourTree.Node)>();

        // State outcome color scheme
        private static Color inProgressColor = new Color(0.4f,0.4f,0.7f);
        private static Color successColor = new Color(0.2f,0.5f,0.2f);
        private static Color failureColor = new Color(0.5f,0.2f,0.2f);
        private static Color unusedColor = new Color(0.5f,0.5f,0.5f);

        static GUIStyle successNode;
        static GUIStyle failureNode;
        static GUIStyle inProgressNode;
        static GUIStyle unusedNode;

        private static GUIStyle NodeStateToStyle(BehaviourTree.Node.State state)
        {
            switch(state)
            {
                case BehaviourTree.Node.State.SUCCESS: return successNode;
                case BehaviourTree.Node.State.FAILURE: return failureNode;
                case BehaviourTree.Node.State.IN_PROGRESS: return inProgressNode;
                default: return unusedNode;
            }
        }

        private static Color NodeStateToColor(BehaviourTree.Node.State state)
        {
            switch(state)
            {
                case BehaviourTree.Node.State.SUCCESS: return successColor;
                case BehaviourTree.Node.State.FAILURE: return failureColor;
                case BehaviourTree.Node.State.IN_PROGRESS: return inProgressColor;
                default: return unusedColor;
            }
        }

        private static GUIStyle MakeNodeStyle(Color color, float scale)
        {
            GUIStyleState styleState = new GUIStyleState();
            styleState.textColor = Color.white;
            styleState.background = new Texture2D(1,1);
            styleState.background.SetPixel(0,0,color);
            styleState.background.Apply();

            GUIStyle nodeStyle = new GUIStyle();
            nodeStyle.wordWrap = scale > 1f;
            nodeStyle.alignment = TextAnchor.MiddleCenter;
            nodeStyle.clipping = TextClipping.Ellipsis;
            nodeStyle.fontStyle = FontStyle.Normal;
            nodeStyle.normal = styleState;
            nodeStyle.fontSize = Mathf.Clamp(Mathf.FloorToInt(30 * scale), 0, 20);

            return nodeStyle;
        }

        public override void OnInspectorGUI() 
        {
            DrawDefaultInspector();

            CustomEditorUtilities.DrawUILine(Color.gray, 0, 10);
            GUIStyle boldStyle = new GUIStyle(GUI.skin.label);
            boldStyle.fontStyle = FontStyle.Bold;
            boldStyle.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.LabelField("Graph viewer", boldStyle);
            CustomEditorUtilities.DrawUILine(Color.gray, 1, 0);

            BehaviourTree bt = (BehaviourTree) target;
            if(bt.root == null)
            {
                GUILayout.Label("The tree's root node is currently null");
                return;
            }

            int treeHeight = bt.GetHeightRecursive();
            int treeNumLeafs = bt.GetNumOfLeafsRecursive();

            // Selected node info display
            if(bt.selectedNode != null)
            {
                GUIStyle nodeNameStyle = new GUIStyle(){ fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
                nodeNameStyle.normal = new GUIStyleState(){ textColor = Color.white};

                GUIStyle nodeStateStyle = new GUIStyle(){ fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleRight };
                nodeStateStyle.normal = new GUIStyleState(){ textColor = NodeStateToColor(bt.selectedNode.lastState) };

                GUIStyle textArea = new GUIStyle(){ wordWrap = true, alignment = TextAnchor.UpperLeft, padding = new RectOffset(20,0,5,10) };
                textArea.normal = new GUIStyleState(){ textColor = Color.white };

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Selected: " + bt.selectedNode.name, nodeNameStyle);
                EditorGUILayout.LabelField(bt.selectedNode.lastState.ToString(), nodeStateStyle);
                EditorGUILayout.EndHorizontal();

                showSelectedNodeFields = CustomEditorUtilities.DisplayTargetFieldsWithReflection(bt.selectedNode, showSelectedNodeFields);
                showSelectedNodeProperties = CustomEditorUtilities.DisplayTargetPropertiesWithReflection(bt.selectedNode, showSelectedNodeProperties);

                EditorGUILayout.TextArea(bt.selectedNode.ToDebugString(), textArea);
            }

            bt.treeViewerScale = EditorGUILayout.Slider("Tree height scale:", bt.treeViewerScale, 0f, 2f);

            successNode = MakeNodeStyle(successColor, bt.treeViewerScale);
            failureNode = MakeNodeStyle(failureColor, bt.treeViewerScale);
            inProgressNode = MakeNodeStyle(inProgressColor, bt.treeViewerScale);
            unusedNode = MakeNodeStyle(unusedColor, bt.treeViewerScale);

            float nodeHeight = 30f * bt.treeViewerScale;
            float nodeVSpacing = 5f * bt.treeViewerScale;

            EditorGUILayout.BeginScrollView(Vector2.zero, GUILayout.Height(treeNumLeafs * (nodeHeight + nodeVSpacing) - nodeVSpacing), GUILayout.ExpandWidth(true));

            Handles.BeginGUI();
            float nodeWidth = EditorGUIUtility.currentViewWidth / treeHeight;
            nodeWidth *= 0.75f;
            float hSpacing = nodeWidth / 3f;

            treeViewerNodes.Clear();
            DrawTreeRecursive(bt.root, nodeWidth, nodeHeight, hSpacing, nodeVSpacing, Mathf.Clamp(8f * bt.treeViewerScale, 0f, 6f));

            // Handling ui clicks
            Event currentEvent = Event.current;
            if (currentEvent.type == EventType.MouseDown)
            {
                foreach((Rect,BehaviourTree.Node) treeNode in treeViewerNodes)
                {
                    Rect clickArea = treeNode.Item1;
                    if (clickArea.Contains(currentEvent.mousePosition))
                    {
                        currentEvent.Use();
                        bt.selectedNode = treeNode.Item2;

                        int state = (int)bt.selectedNode.lastState + 1;
                        state = state > 2 ? -1 : state;
                        bt.selectedNode.lastState = (BehaviourTree.Node.State) state;
                        break;
                    }
                }
            }
            Handles.EndGUI();
            EditorGUILayout.EndScrollView();
        }

        private void DrawHorizontalLine(Color color, Vector2 from, float lineThickness, float length)
        {
            Handles.DrawSolidRectangleWithOutline(
                new Rect(from.x, from.y - lineThickness/2f, length, lineThickness), 
                color, Color.clear);
        }

        private void DrawVerticalLine(Color color, Vector2 from, float lineThickness, float length)
        {
            Handles.DrawSolidRectangleWithOutline(
                    new Rect(from.x, from.y - lineThickness/2f, lineThickness, length), 
                    color, Color.clear);
        }

        private int DrawTreeRecursive(BehaviourTree.Node node, 
            float nodeXSize = 50f, float nodeYSize = 30f, float hSpace = 20f, float vSpace = 20f, float lineThickness = 2f,
            int depth = 0, int height = 0
        )
        {   
            Vector2 position = new Vector2(depth * (nodeXSize + hSpace), height * (nodeYSize + vSpace));
            Rect nodeRect = new Rect(position.x, position.y, nodeXSize, nodeYSize);
            treeViewerNodes.Add((nodeRect,node));
            GUI.Box(nodeRect, node.name, NodeStateToStyle(node.lastState));

            if(node.children.Count == 0) return height + 1;
            Vector2 linePosition = position + new Vector2(nodeXSize,nodeYSize/2f);

            DrawHorizontalLine(NodeStateToColor(node.GetHighestPriorityStateInChildren(0)), linePosition, lineThickness, hSpace/2f + lineThickness);
            linePosition += Vector2.right * hSpace / 2f;
            
            int prevHeigh = height;
            for(int i = 0; i < node.children.Count; i++)
            {
                BehaviourTree.Node child = node.children[i];
                height = DrawTreeRecursive(child, nodeXSize, nodeYSize, hSpace, vSpace, lineThickness, depth + 1, height);
                DrawHorizontalLine(NodeStateToColor(node.children[i].lastState), linePosition + new Vector2(lineThickness,0), lineThickness, hSpace/2f - lineThickness);

                if(i == node.children.Count - 1) break;

                Color verticalLineColor = NodeStateToColor(node.GetHighestPriorityStateInChildren(i+1));
                DrawVerticalLine(verticalLineColor, linePosition + new Vector2(0,lineThickness), lineThickness, (nodeYSize + vSpace) * (height - prevHeigh));
                linePosition += Vector2.up * (nodeYSize + vSpace) * (height - prevHeigh);

                prevHeigh = height;
            }

            return height;
        }
    }
}
#endif
