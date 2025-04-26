using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns
{
    /// <summary>
    /// Base class for behaviour trees. Inherit from it to make your own
    /// </summary>
    public abstract class BehaviourTree : MonoBehaviour
    {
        // Custom inspector variables
        [HideInInspector] public float treeViewerScale = 1f;
        [HideInInspector] public Node selectedNode = null;

        protected abstract Node BuildTree();

        public Node root;

        protected virtual void Start()
        {
            root = BuildTree();   
        }

        /// <summary>
        /// Gets the height of the tree recursively. Used only to draw the custom inspector.
        /// </summary>
        public int GetHeightRecursive(Node current = null, int acc = 1)
        {
            if(root == null) return 0;
            if(current == null) current = root;
            if(current.children.Count == 0) return acc;
            int max = acc;
            foreach(Node child in current.children)
            {
                int height = GetHeightRecursive(child, acc+1);
                if(height > max) max = height;
            }
            return max;
        }
        /// <summary>
        /// Gets the total number of leafs in the tree recusively. Used only to draw the custom inspector.
        /// </summary>
        public int GetNumOfLeafsRecursive(Node current = null)
        {
            if(root == null) return 0;
            if(current == null) current = root;
            if(current.children.Count == 0) return 1;
            int count = 0;
            foreach(Node child in current.children) count += GetNumOfLeafsRecursive(child);
            return count;
        }
        /// <summary>
        /// Resets all the lastState of all nodes in the tree. Only useful to properly draw the custom inspector.
        /// </summary>
        private void ResetNodeLastStateRecursive(Node node)
        {
            node.lastState = Node.State.NONE;
            foreach(Node child in node.children) ResetNodeLastStateRecursive(child);
        }
        /// <summary>
        /// Evaluates the tree, starting from the root, with the given delta time.
        /// </summary>
        protected void EvaluateTree(float deltaTime)
        {
            if(root == null) return;
            #if UNITY_EDITOR
                ResetNodeLastStateRecursive(root);
            #endif
            root.Evaluate(deltaTime);
        }

        /// <summary>
        /// Base class for all nodes. You should inherit from it to make your own custom nodes.
        /// </summary>
        public abstract class Node
        {
            public State lastState = State.NONE;
            public enum State { NONE = -1, FAILURE, IN_PROGRESS, SUCCESS }

            public readonly string name;
            public List<Node> children = new List<Node>();

            /// <summary>
            /// Only used for drawing the custom inspector, the priority is only used to determine
            /// which color is drawn when the lines merge (gree, red, or blue)
            /// </summary>
            public State GetHighestPriorityStateInChildren(int startIndex)
            {
                State highestPriorityState = State.NONE;
                for(int i = startIndex; i < children.Count; i++)
                {
                    if((int)children[i].lastState > (int)highestPriorityState)
                        highestPriorityState = children[i].lastState;
                }
                return highestPriorityState;
            }

            public Node(string name) => this.name = name;

            /// <summary>
            /// Evaluates this node with the given delta time.
            /// </summary>
            /// <returns> The state of this node after evaluation. </returns>
            public State Evaluate(float deltaTime)
            {
                lastState = EvaluateProccess(deltaTime);
                return lastState;
            }

            /// <summary>
            /// Implement this method to set your custom evaluation proccess.
            /// </summary>
            /// <returns> The state of this node after evaluation. </returns>
            protected abstract State EvaluateProccess(float deltaTime);

            /// <summary>
            /// Override this function to write text to be shown in the custom inspector for debuginng.
            /// </summary>
            public virtual string ToDebugString() => "You can override \"ToDebugString()\" to display custom debug information about your node here!";

            /// <summary>
            /// Attaches a child node to this node.
            /// </summary>
            /// <returns> The child node you just attatched. </returns>
            public Node Attatch(Node node)
            {
                children.Add(node);
                return node;
            }
        }

        /// <summary>
        /// Sequencer node. It evaluates all of its children in sequence,
        /// if they all succeed, then it itself succeeds, but if any of them
        /// fail or are in progress, it returns early with the same state.
        /// </summary>
        public class Sequencer : Node
        {
            public Sequencer(string name) : base(name) {}
            protected override State EvaluateProccess(float deltaTime)
            {
                foreach(Node child in children)
                {
                    switch(child.Evaluate(deltaTime))
                    {
                        case State.FAILURE: return State.FAILURE;
                        case State.IN_PROGRESS: return State.IN_PROGRESS;
                    }
                }
                return State.SUCCESS;
            }
        }

        /// <summary>
        /// Selector node. It evaluates all of its children in sequence,
        /// if any of them succeed or are in progress, then it itself returns the same state.
        /// If all children fail, it fails.
        /// </summary>
        public class Selector : Node
        {
            public Selector(string name) : base(name) {}
            protected override State EvaluateProccess(float deltaTime)
            {
                foreach(Node child in children)
                {
                    switch(child.Evaluate(deltaTime))
                    {
                        case State.SUCCESS: return State.SUCCESS;
                        case State.IN_PROGRESS: return State.IN_PROGRESS;
                    }
                }
                return State.FAILURE;
            }
        }

    }
}

