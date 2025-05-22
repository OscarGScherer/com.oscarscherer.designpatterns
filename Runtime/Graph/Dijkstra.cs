using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns
{
    namespace Graph
    {
        public class Dijkstra
        {
            /// <summary>
            /// Returns an array containing the first move you need to make in order to move from the startNode to all other nodes,
            ///  and the distances to all other nodes.
            public static (int,float)[] Pathfind<N,E>(Node<N,E> startNode, List<Node<N,E>> nodes, List<Edge<N,E>> edges)
            where N : INodeContent<N,E>
            where E : IEdgeContent<N,E>
            {
                bool[] visited = false.RepeatForArray(nodes.Count);
                (int,float)[] paths = (-1, Mathf.Infinity).RepeatForArray(nodes.Count);

                paths[startNode.index] = (startNode.index, 0f);
                visited[startNode.index] = true;

                Heap<Node<N,E>> unvisitedNodes = new Heap<Node<N,E>>(nodes.Count, (n1,n2) => paths[n1.index].Item2 < paths[n2.index].Item2);
                foreach(Edge<N,E> edge in startNode.edges)
                {
                    Node<N,E> adj = edge.Adjacent(startNode);
                    paths[adj.index] = (edge.index, paths[adj.index].Item2 + edge.length);
                    unvisitedNodes.Insert(adj);
                }

                while(unvisitedNodes.Count > 0)
                {
                    Node<N,E> curr = unvisitedNodes.Extract();
                    visited[curr.index] = true;
                    foreach(Edge<N,E> edge in curr.edges)
                    {
                        Node<N,E> adj = edge.Adjacent(curr);
                        float newDistance = paths[curr.index].Item2 + edge.length;
                        if(paths[adj.index].Item2 > newDistance)
                        {
                            paths[adj.index] = (paths[curr.index].Item1, newDistance);
                        }
                        // Adding to heap
                        if(!visited[adj.index]) unvisitedNodes.Insert(adj);
                    }
                }
                
                // Setting up paths
                return paths;
            }
        }
    }
}
