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
        public class Graph<N, E>
        where N : Node
        where E : Edge
        {
            public List<N> nodes;
            public List<E> edges;
            private int[,] paths;
            private float[,] distances;

            public Graph(int numNodes)
            {
                nodes = new List<N>(numNodes);
                edges = new List<E>(numNodes * 2);
            }

            public void AddNode(N node)
            {
                nodes.Add(node);
            }
            public void AddEdge(E edge)
            {
                if (!nodes.Contains(edge.A) || !nodes.Contains(edge.B)) return;
                edges.Add(edge);
                edge.A.edges.Add(edge);
                edge.B.edges.Add(edge);
            }

            private void UpdatePaths()
            {
                paths = new int[nodes.Count, nodes.Count];
                distances = new float[nodes.Count, nodes.Count];
                for (int i = 0; i < nodes.Count; i++)
                {
                    (int, float)[] pathInfo = Dijkstra.Pathfind(nodes[i], nodes);
                    for (int j = 0; j < nodes.Count; i++)
                    {
                        paths[i, j] = pathInfo[j].Item1;
                        distances[i, j] = pathInfo[j].Item2;
                    }
                }
            }

            /// <summary>
            /// Generates a random graph with the given number of nodes.
            /// Unused.
            /// </summary>
            // public static Graph<N, E> GenerateRandom(int numNodes)
            // {
            //     numNodes = numNodes < 0 ? 0 : numNodes;

            //     Graph<N, E> graph = new Graph<N, E>(numNodes);
            //     // Adding nodes
            //     for (int n = 0; n < numNodes; n++)
            //     {
            //         graph.nodes.Add(new Node<N>(n, null, default));
            //     }

            //     // If there are 1 or 0 nodes, there aren't any edges, return
            //     if (numNodes <= 1) return graph;

            //     // Setting edges
            //     NodeGroups<Node<N>> nodeGroups = new NodeGroups<Node<N>>(graph.nodes);
            //     while (nodeGroups.numGroups > 1)
            //     {
            //         int a = Random.Range(0, graph.nodes.Count);
            //         int b = Random.Range(0, graph.nodes.Count);
            //         if (a == b)
            //         {
            //             b += Random.Range(0f, 1f) > 0.5f ? 1 : -1;
            //             b = b > graph.nodes.Count - 1 ? 0 : b < 0 ? graph.nodes.Count - 1 : b;
            //         }
            //         Edge<E> edge = new Edge<E>(graph.nodes[a], graph.nodes[b], default);
            //         nodeGroups.AddEdge(edge);
            //     }

            //     // Setup Paths
            //     graph.UpdatePaths();

            //     return graph;
            // }
        }

        public class NodeGroups<N> where N : Node
        {
            private List<List<Node>> nodeGroups = new List<List<Node>>();
            public int numGroups => nodeGroups.Count;

            public NodeGroups(List<N> nodes)
            {
                nodes.ForEach((n) => nodeGroups.Add(new List<Node>() { n }));
            }

            /// <summary> Merge's the edge's a node group with the edge's b node group </summary>
            public void AddEdge(Edge edge)
            {
                List<Node> groupWithA = nodeGroups.FirstOrDefault((g) => g.Contains(edge.A)) ?? new List<Node>() { edge.A };
                nodeGroups.Remove(groupWithA);
                List<Node> groupWithB = nodeGroups.FirstOrDefault((g) => g.Contains(edge.B));
                if (groupWithB == null)
                {
                    groupWithB = new List<Node>() { edge.B };
                    nodeGroups.Add(groupWithB);
                }
                groupWithB.AddRange(groupWithA);
            }
        }

        public abstract class Edge
        {
            public Node A { private set; get; }
            public Node B { private set; get; }

            public float length;

            /// <summary> Just a shorthand. It assumes n is A or B. </summary>
            public Node Adjacent(Node n) => n == A ? B : A;
            /// <summary> Creates an edge between a and b, with the given length. </summary>
            public Edge(Node a, Node b, float? length = null)
            {
                A = a;
                B = b;
                this.length = length == null ? Vector3.Distance(A.position,B.position) : length.Value;
            }
        }

        public abstract class Node
        {
            public int index { private set; get; }
            public List<Edge> edges { private set; get; }

            public Vector3 position;

            public Node(int index, Vector3? position)
            {
                this.index = index;
                this.position = position ?? Vector3.zero;
                edges = new List<Edge>();
            }

            public int GetNumAdjacentNodes() => edges.Count;
        }
    }
}
