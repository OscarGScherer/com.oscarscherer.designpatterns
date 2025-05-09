using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DesignPatterns
{
    namespace Graph
    {
        public class Graph
        {
            private List<Node> nodes;
            private List<Edge> edges;
            private int[,] paths;

            public Graph(int numNodes)
            {
                nodes = new List<Node>(numNodes);
                edges = new List<Edge>(numNodes*2);
            }

            public void AddNode(Node node)
            {
                nodes.Add(node);
            }

            private void UpdatePaths()
            {
                paths = new int[nodes.Count, nodes.Count];
                for(int i = 0; i < nodes.Count; i++)
                {
                    int[] pathsFromI = Dijkstra.Pathfind(nodes[i], nodes);
                    for(int j = 0; j < nodes.Count; i++)
                    {
                        paths[i,j] = pathsFromI[j];
                    }
                }
            }

            /// <summary>
            /// Generates a random graph with the given number of nodes.
            /// </summary>
            public static Graph GenerateRandom(int numNodes)
            {
                numNodes = numNodes < 0 ? 0 : numNodes;

                Graph graph = new Graph(numNodes);
                // Adding nodes
                for(int n = 0; n < numNodes; n++)
                {
                    graph.nodes.Add(new Node(n));
                }

                // If there are 1 or 0 nodes, there aren't any edges, return
                if(numNodes <= 1) return graph;

                // Setting edges
                NodeGroups nodeGroups = new NodeGroups(graph.nodes);
                while(nodeGroups.numGroups > 1)
                {
                    int a = Random.Range(0,graph.nodes.Count);
                    int b = Random.Range(0,graph.nodes.Count);
                    if(a == b)
                    {
                        b += Random.Range(0f,1f) > 0.5f ? 1 : -1;
                        b = b > graph.nodes.Count - 1 ? 0 : b < 0 ? graph.nodes.Count - 1 : b;
                    }
                    Edge edge = new Edge(graph.nodes[a], graph.nodes[b]);
                    nodeGroups.AddEdge(edge);
                }

                // Setup Paths
                graph.UpdatePaths();

                return graph;
            }

            private class NodeGroups
            {
                private List<List<Node>> nodeGroups = new List<List<Node>>();
                public int numGroups => nodeGroups.Count;

                public NodeGroups(List<Node> nodes)
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
        }

        public class Edge
        {
            public Node A,B;
            public float length;
            public float distance;
            public Edge(Node a, Node b)
            {
                A = a;
                B = b;
                a.edges.Add(this);
                b.edges.Add(this);
            }
            /// <summary> Just a shorthand. It assumes n is A or B. </summary>
            public Node Adjacent(Node n) => n == A ? B : A; 
        }

        public class Node
        {
            public Vector3 position;
            public int index;
            public List<Edge> edges = new List<Edge>();

            public Node(int index)
            {
                this.index = index;
            }

            public int GetNumAdjacentNodes() => edges.Count;

            /// <returns> All nodes ajacent to this one. </returns>
            public List<Node> GetAdjacent()
            {
                List<Node> adjacentNodes = new List<Node>(edges.Count);
                foreach(Edge edge in edges) adjacentNodes.Add(edge.A == this ? edge.B : edge.A);
                return adjacentNodes;
            }
        }
    }
}
