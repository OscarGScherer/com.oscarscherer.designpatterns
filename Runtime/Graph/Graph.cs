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

            public GraphPosition MoveUsingBestPath(GraphPosition start, GraphPosition goal, float distance)
            {
                GraphPosition graphPos = new GraphPosition(start.nodeI, start.edgeI, start.edgePos);
                //int i = 0;
                while (distance > 0 && !graphPos.SamePositionAs(goal))
                {
                    float step = 0;
                    // If the goal is on an edge and you are in the same edge as the goal, just move to the goal's edgePos
                    if (goal.IsOnEdge() && graphPos.edgeI == goal.edgeI)
                    {
                        step = graphPos.edgePos - goal.edgePos;
                    }
                    // If the goal is on an edge and you are in the same node as the goal, move to the goal's edge
                    else if (goal.IsOnEdge() && graphPos.nodeI == goal.nodeI)
                    {
                        graphPos.edgeI = goal.edgeI;
                        graphPos.edgePos = edges[graphPos.edgeI].A.index == graphPos.nodeI ? 0f : 1f;
                        continue;
                    }
                    // If you are in the edge in the correct path to the goal, move along it
                    else if (graphPos.edgeI == paths[graphPos.nodeI, goal.nodeI])
                    {
                        step = (edges[graphPos.edgeI].A.index == graphPos.nodeI ? 1f : 0f) - graphPos.edgePos;
                    }
                    // If you are in the wrong edge, move back towards your node
                    else if (graphPos.IsOnEdge())
                    {
                        step = (edges[graphPos.edgeI].A.index == graphPos.nodeI ? 0f : 1f) - graphPos.edgePos;
                    }
                    else // Else if you are in a node and not any edge, move to the best path edge
                    {
                        int targetEdge = paths[graphPos.nodeI, goal.nodeI];
                        graphPos.edgeI = targetEdge;
                        graphPos.edgePos = edges[graphPos.edgeI].A.index == graphPos.nodeI ? 0f : 1f;
                        continue;
                    }

                    // Factoring in the length of the edge
                    float stepPercent = distance / Mathf.Abs(step * edges[graphPos.edgeI].length);
                    step = Mathf.Clamp(Mathf.Abs(step), 0, stepPercent) * Mathf.Sign(step);

                    graphPos.edgePos += step;
                    graphPos.nodeI = graphPos.edgePos > 0.5f ? edges[graphPos.edgeI].B.index : edges[graphPos.edgeI].A.index;
                    distance -= Mathf.Abs(step) * edges[graphPos.edgeI].length;
                    // i++;
                    // Debug.Log($"Step percent: {stepPercent}, step: {step}, distance: {distance}");
                    // if (i > 1000) break;
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
                    for (int j = 0; j < nodes.Count; j++)
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

            public bool IsOnEdge() => edgePos > 0f && edgePos < 1f;

            public override string ToString()
            {
                return $"NodeI:{nodeI},EdgeI:{edgeI},EdgePos:{edgePos}";
            }

            public bool SamePositionAs(GraphPosition other)
            {
                return
                    nodeI == other.nodeI &&                                                         // They must be on the same node AND
                    ((edgeI == other.edgeI && edgePos == other.edgePos) ||                          // They are on the same edge and edgePos OR
                    (edgePos <= 0 || edgePos >= 1) && (other.edgePos <= 0 || other.edgePos >= 1));  // They both are on their edge's ends (0 or 1)
            }

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
