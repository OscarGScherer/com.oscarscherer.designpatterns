using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns
{
    public class BehaviourTree : MonoBehaviour
    {
        [HideInInspector] public float treeViewerScale = 1f;
        [HideInInspector] public Node selectedNode = null;

        protected virtual Node BuildTree() => null;
        public Node root = BuildTestTree();

        private static Node BuildTestTree()
        {
            Node root = new Selector("Main");

            Node seq0 = root.Attatch(new Sequencer("Seq0"));
            Node seq1 = seq0.Attatch(new Node("Seq1"));
            seq1.Attatch(new Node("Leaf1 testing testing testing testing testing"));
            seq1.Attatch(new Node("Leaf2"));
            seq0.Attatch(new Node("Leaf3"));

            Node seq2 = root.Attatch(new Sequencer("Seq2"));
            seq2.Attatch(new Node("Leaf4"));
            seq2.Attatch(new Node("Leaf5"));

            return root;
        }

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

        public int GetNumOfLeafsRecursive(Node current = null)
        {
            if(root == null) return 0;
            if(current == null) current = root;
            if(current.children.Count == 0) return 1;
            int count = 0;
            foreach(Node child in current.children) count += GetNumOfLeafsRecursive(child);
            return count;
        }

        private void ResetNodeLastStateRecursive(Node node)
        {
            node.lastState = Node.State.NONE;
            foreach(Node child in node.children) ResetNodeLastStateRecursive(child);
        }

        protected void EvaluateTree(float deltaTime)
        {
            if(root == null) root = BuildTree();
            // ResetNodeLastStateRecursive(root);
            root.Evaluate(deltaTime);
        }

        public class Node
        {
            public State lastState = State.NONE;
            public enum State { NONE = -1, FAILURE, IN_PROGRESS, SUCCESS }

            public readonly string name;
            public List<Node> children = new List<Node>();

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
            // public Node() => this.name = "unnamed";

            public State Evaluate(float deltaTime)
            {
                lastState = EvaluateProccess(deltaTime);
                return lastState;
            }
            protected virtual State EvaluateProccess(float deltaTime) => State.SUCCESS;

            public virtual string ToDebugString() => "You can override \"ToDebugString()\" to display custom debug information about your node here!";

            public Node Attatch(Node node)
            {
                children.Add(node);
                return node;
            }
        }

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

