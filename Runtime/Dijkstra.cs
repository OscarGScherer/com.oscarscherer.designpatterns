using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns
{
    namespace Graph
    {
        public class Dijkstra
        {
            public static int[] Pathfind<N>(N startNode, List<N> nodes) where N : Node
            {
                int[] pathOrigins = (-1).RepeatForArray(nodes.Count);
                bool[] visited = false.RepeatForArray(nodes.Count);
                float[] distances = Mathf.Infinity.RepeatForArray(nodes.Count);

                distances[startNode.index] = 0f;
                pathOrigins[startNode.index] = startNode.index;
                visited[startNode.index] = true;

                Heap<Node> unvisitedNodes = new Heap<Node>(nodes.Count, (n1,n2) => distances[n1.index] < distances[n2.index]);
                foreach(Edge edge in startNode.edges)
                {
                    Node adj = edge.Adjacent(startNode);
                    distances[adj.index] = distances[startNode.index] + edge.length;
                    pathOrigins[adj.index] = adj.index;
                    unvisitedNodes.Insert(adj);
                }

                while(unvisitedNodes.Count > 0)
                {
                    Node curr = unvisitedNodes.Extract();
                    visited[curr.index] = true;
                    foreach(Edge edge in curr.edges)
                    {
                        Node adj = edge.Adjacent(curr);
                        float newDistance = distances[curr.index] + edge.length;
                        if(distances[adj.index] > newDistance)
                        {
                            distances[adj.index] = newDistance;
                            pathOrigins[adj.index] = pathOrigins[curr.index];
                        }
                        // Adding to heap
                        if(!visited[adj.index]) unvisitedNodes.Insert(adj);
                    }
                }
                
                // Setting up paths
                return pathOrigins;
            }
        }
    }
}
