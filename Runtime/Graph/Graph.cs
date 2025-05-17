using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DesignPatterns
{
    namespace Graph
    {
        /// <summary>
        /// Base class for graphs, featuring baked in pathfinding.
        /// </summary>
        /// <typeparam name="N">The type of data that is stored in your nodes</typeparam>
        /// <typeparam name="E">The type of data that is stored in your edges</typeparam>
        public class Graph<N,E>
        {
            public List<Node<N>> nodes;
            public List<Edge<E>> edges;
            private int[,] paths;
            private float[,] distances;

            public Graph(int numNodes)
            {
                nodes = new List<Node<N>>(numNodes);
                edges = new List<Edge<E>>(numNodes*2);
            }

            /// <summary> Adds a node to the graph, with the given content and position. </summary>
            public void AddNode(N nodeItem, Vector3? position = null)
            {
                nodes.Add(new Node<N>(nodes.Count, position, nodeItem));
            }
            /// <summary> Adds edge between two existing nodes in the graph, finds the nodes by their index. </summary>
            public bool AddEdge(int aInd, int bInd, E e)
            {
                if(!nodes.ValidIndex(aInd) || !nodes.ValidIndex(bInd)) return false;
                return AddEdge(nodes[aInd], nodes[bInd], e);
            }
            /// <summary> Adds edge between two existing nodes in the graph, finds the nodes by their content. </summary>
            public bool AddEdge(N a, N b, E e)
            {
                Node<N> nodeA = nodes.FirstOrDefault((n)=>n.nodeItem.Equals(a));
                Node<N> nodeB = nodes.FirstOrDefault((n)=>n.nodeItem.Equals(b));
                return AddEdge(nodeA, nodeB, e);
            }
            /// <summary> Helper function to add edges to the graph. </summary>
            private bool AddEdge(Node<N> nodeA, Node<N> nodeB, E e, float? length = null)
            {
                if(nodeA == null || nodeB == null || nodeA == nodeB) return false;
                edges.Add(length == null ? new Edge<E>(nodeA, nodeB, e) : new Edge<E>(nodeA, nodeB, length.Value, e));
                return true;
            }

            private void UpdatePaths()
            {
                paths = new int[nodes.Count, nodes.Count];
                distances = new float[nodes.Count, nodes.Count];
                for(int i = 0; i < nodes.Count; i++)
                {
                    (int,float)[] pathInfo = Dijkstra.Pathfind(nodes[i], nodes);
                    for(int j = 0; j < nodes.Count; i++)
                    {
                        paths[i,j] = pathInfo[j].Item1;
                        distances[i,j] = pathInfo[j].Item2;
                    }
                }
            }

            /// <summary>
            /// Generates a random graph with the given number of nodes.
            /// Unused.
            /// </summary>
            public static Graph<N,E> GenerateRandom(int numNodes)
            {
                numNodes = numNodes < 0 ? 0 : numNodes;

                Graph<N,E> graph = new Graph<N,E>(numNodes);
                // Adding nodes
                for(int n = 0; n < numNodes; n++)
                {
                    graph.nodes.Add(new Node<N>(n, null, default));
                }

                // If there are 1 or 0 nodes, there aren't any edges, return
                if(numNodes <= 1) return graph;

                // Setting edges
                NodeGroups<Node<N>> nodeGroups = new NodeGroups<Node<N>>(graph.nodes);
                while(nodeGroups.numGroups > 1)
                {
                    int a = Random.Range(0,graph.nodes.Count);
                    int b = Random.Range(0,graph.nodes.Count);
                    if(a == b)
                    {
                        b += Random.Range(0f,1f) > 0.5f ? 1 : -1;
                        b = b > graph.nodes.Count - 1 ? 0 : b < 0 ? graph.nodes.Count - 1 : b;
                    }
                    Edge<E> edge = new Edge<E>(graph.nodes[a], graph.nodes[b], default);
                    nodeGroups.AddEdge(edge);
                }

                // Setup Paths
                graph.UpdatePaths();

                return graph;
            }
        }

        public class NodeGroups<N> where N : Node
        {
            private List<List<Node>> nodeGroups = new List<List<Node>>();
            public int numGroups => nodeGroups.Count;

            public NodeGroups(List<N> nodes)
            {
                nodes.ForEach((n)=>nodeGroups.Add(new List<Node>(){n}));
            }

            /// <summary> Merge's the edge's a node group with the edge's b node group </summary>
            public void AddEdge(Edge edge)
            {
                List<Node> groupWithA = nodeGroups.FirstOrDefault((g)=>g.Contains(edge.A)) ?? new List<Node>(){edge.A};
                nodeGroups.Remove(groupWithA);
                List<Node> groupWithB = nodeGroups.FirstOrDefault((g)=>g.Contains(edge.B));
                if(groupWithB == null)
                {
                    groupWithB = new List<Node>(){edge.B};
                    nodeGroups.Add(groupWithB);
                }
                groupWithB.AddRange(groupWithA);
            }
        }

        public abstract class Edge
        {
            public Node A, B;
            public float length;
            /// <summary> Just a shorthand. It assumes n is A or B. </summary>
            public Node Adjacent(Node n) => n == A ? B : A;
            /// <summary> Creates an edge between a and b, using the position of the nodes to infer the edge length. </summary>
            public Edge(Node a, Node b)
            {
                A = a;
                B = b;
                length = Vector3.Distance(A.position, B.position);
                a.edges.Add(this);
                b.edges.Add(this);
            }
            /// <summary> Creates an edge between a and b, with the given length. </summary>
            public Edge(Node a, Node b, float length)
            {
                A = a;
                B = b;
                this.length = length;
                a.edges.Add(this);
                b.edges.Add(this);
            }
        }

        public class Edge<E> : Edge
        {
            public E edgeItem;
            public Edge(Node a, Node b, E edgeItem) : base(a, b)
            {
                this.edgeItem = edgeItem;
            }
            public Edge(Node a, Node b, float length, E edgeItem) : base(a,b,length)
            {
                this.edgeItem = edgeItem;
            }
        }

        public abstract class Node
        {
            public Vector3 position;
            public int index;
            public List<Edge> edges = new List<Edge>();
            public int GetNumAdjacentNodes() => edges.Count;

            public Node(int index, Vector3? position)
            {
                this.index = index;
                this.position = position ?? Vector3.zero;
            }

            // /// <returns> All nodes ajacent to this one. </returns>
            // public List<Node<N,E>> GetAdjacent()
            // {
            //     List<Node<N,E>> adjacentNodes = new List<Node<N,E>>(edges.Count);
            //     foreach(Edge<N,E> edge in edges) adjacentNodes.Add(edge.A == this ? edge.B : edge.A);
            //     return adjacentNodes;
            // }

        }

        public class Node<N> : Node
        {
            public N nodeItem;
            public Node(int index, Vector3? position, N nodeItem) : base(index, position)
            {
                this.nodeItem = nodeItem;
            }
        }
    }
}
