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
        where N : INodeContent<N,E>
        where E : IEdgeContent<N,E>
        {
            public List<Node<N,E>> nodes;
            public List<Edge<N,E>> edges;
            public int[,] paths;
            public float[,] distances;

            public Graph(int numNodes)
            {
                nodes = new List<Node<N,E>>(numNodes);
                edges = new List<Edge<N,E>>(numNodes * 2);
            }

            public GraphPosition MoveUsingBestPath(GraphPosition graphPos, int nodeGoal, float distance)
            {
                while (distance > 0 && graphPos.nodeI != nodeGoal)
                {
                    if (graphPos.edgeI != -1) // If the position is on an edge, move
                    {
                        // Update node pos in case the node position is wrong
                        graphPos.nodeI = graphPos.edgePos > 0.5f ? edges[graphPos.edgeI].B.index : edges[graphPos.edgeI].A.index;
                        int targetEdge = paths[graphPos.nodeI, nodeGoal];
                        int direction = -1, targetPos = 0;
                        if ((graphPos.edgeI == targetEdge && edges[targetEdge].A.index == graphPos.nodeI) ||
                            (graphPos.edgeI != targetEdge && edges[targetEdge].B.index == graphPos.nodeI))
                        {
                            direction = 1;
                            targetPos = 1;
                        }
                        float step = Mathf.Clamp(Mathf.Abs(targetPos - graphPos.edgePos), 0f, distance) * direction;
                        distance -= Mathf.Abs(step);
                        graphPos.edgePos += step;
                        // Update node position based on edgePos
                        graphPos.nodeI = graphPos.edgePos > 0.5f ? edges[graphPos.edgeI].B.index : edges[graphPos.edgeI].A.index;
                    }
                    else // Otherwise, pick an edge to move
                    {
                        graphPos.edgeI = paths[graphPos.nodeI, nodeGoal];
                        graphPos.edgePos = edges[graphPos.edgeI].A.index == graphPos.nodeI ? 0f : 1f;
                    }
                }
                return graphPos;
            }

            public Node<N, E> AddNode(N nodeContent)
            {
                nodes.Add(new Node<N, E>(nodes.Count, nodeContent));
                return nodes[nodes.Count - 1];
            }
            public void AddEdge(N aContent, N bContent, E edgeContent)
            {
                Node<N,E> A = nodes.FirstOrDefault((n) => n.content.Equals(aContent));
                if (A == null) return;
                Node<N,E> B = nodes.FirstOrDefault((n) => n.content.Equals(bContent));
                if (B == null) return;
                AddEdge(A, B, edgeContent);
            }
            public void AddEdge(int aInd, int bInd, E edgeContent)
            {
                if (!nodes.ValidIndex(aInd) || !nodes.ValidIndex(bInd)) return;
                AddEdge(nodes[aInd], nodes[bInd], edgeContent);
            }
            public void AddEdge(Node<N,E> A, Node<N,E> B, E edgeContent)
            {
                if (!nodes.Contains(A) || !nodes.Contains(B)) return;
                edges.Add(new Edge<N,E>(edges.Count, A, B, edgeContent));
                Edge<N,E> edge = edges[edges.Count - 1];
                edge.A.edges.Add(edge);
                edge.B.edges.Add(edge);
            }

            public void UpdatePathsAndDistances()
            {
                paths = new int[nodes.Count, nodes.Count];
                distances = new float[nodes.Count, nodes.Count];
                for (int i = 0; i < nodes.Count; i++)
                {
                    (int, float)[] pathInfo = Dijkstra.Pathfind(nodes[i], nodes, edges);
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

        public class GraphPosition
        {
            public int nodeI, edgeI;
            public float edgePos;
            public GraphPosition(int nodeI, int edgeI, float edgePos)
            {
                this.nodeI = nodeI;
                this.edgeI = edgeI;
                this.edgePos = edgePos;
            }
            public GraphPosition(int nodeI)
            {
                this.nodeI = nodeI;
                edgeI = -1;
                edgePos = -1;
            }
            public GraphPosition(int edgeI, float edgePos)
            {
                nodeI = -1;
                this.edgeI = edgeI;
                this.edgePos = edgePos;
            }
        }

        public class NodeGroups<N, E>
        where N : INodeContent<N, E>
        where E : IEdgeContent<N, E>
        {
            private List<List<Node<N, E>>> nodeGroups = new List<List<Node<N, E>>>();
            public int numGroups => nodeGroups.Count;

            public NodeGroups(List<Node<N, E>> nodes)
            {
                nodes.ForEach((n) => nodeGroups.Add(new List<Node<N, E>>() { n }));
            }

            /// <summary> Merge's the edge's a node group with the edge's b node group </summary>
            public void AddEdge(Edge<N, E> edge)
            {
                List<Node<N, E>> groupWithA = nodeGroups.FirstOrDefault((g) => g.Contains(edge.A)) ?? new List<Node<N, E>>() { edge.A };
                nodeGroups.Remove(groupWithA);
                List<Node<N, E>> groupWithB = nodeGroups.FirstOrDefault((g) => g.Contains(edge.B));
                if (groupWithB == null)
                {
                    groupWithB = new List<Node<N, E>>() { edge.B };
                    nodeGroups.Add(groupWithB);
                }
                groupWithB.AddRange(groupWithA);
            }
        }

        public interface IEdgeContent<N,E>
        where E : IEdgeContent<N,E>
        where N : INodeContent<N, E>
        {
            public abstract void SetEdge(Edge<N, E> edge);
            public float length { get; }
        }

        public class Edge<N, E>
        where E : IEdgeContent<N, E>
        where N : INodeContent<N, E>
        {
            public int index;
            public Node<N, E> A, B;
            public E content;
            public float length => content.length;

            public Node<N, E> Adjacent(Node<N, E> n) => n == A ? B : A;

            public Edge(int index, Node<N, E> A, Node<N, E> B, E content)
            {
                this.index = index;
                this.A = A;
                this.B = B;
                this.content = content;
                content.SetEdge(this);
            }
        }

        public interface INodeContent<N, E>
        where E : IEdgeContent<N,E>
        where N : INodeContent<N, E>
        {
            public abstract void SetNode(Node<N, E> node);
            public Vector3 position { get; }
        }

        public class Node<N, E>
        where N : INodeContent<N, E>
        where E : IEdgeContent<N, E>
        {
            public int index;
            public List<Edge<N, E>> edges = new List<Edge<N, E>>();
            public Vector3 position => content.position;
            public N content;

            public Node(int index, N content)
            {
                this.index = index;
                this.content = content;
                content.SetNode(this);
            }

            public int GetNumAdjacentNodes() => edges.Count;

            public List<N> GetAdjacentContent()
            {
                List<N> adjacent = new List<N>(edges.Count);
                foreach (Edge<N, E> edge in edges) adjacent.Add(edge.Adjacent(this).content);
                return adjacent;
            }
        }
    }
}
