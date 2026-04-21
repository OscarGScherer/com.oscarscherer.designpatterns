// using System.Collections.Generic;
// using UnityEngine;

// namespace DesignPatterns
// {
//     public class ObjectPool
//     {
//         public interface IPoolingOptions
//         {
//             public int poolStartSize {get;}
//             public int poolExpansionFactor {get;}
//             // public int poolShrinkFrameTimer {get;}
//         }

//         private static Dictionary<GameObject, ObjectPool> pools = new();

//         public static void InstantiateFromPool(GameObject prefab)
//         {
//             if(!pools.ContainsKey(prefab)) MakeNewPool(prefab);
//         }

//         private static void MakeNewPool(GameObject gameObject)
//         {
//             IPoolingOptions poolingOptions = gameObject.GetComponent<IPoolingOptions>();
//             if(poolingOptions != null)
//         }


//         private Transform objectsParent;
//         private GameObject prefab;
//         private LinkedList<GameObject> disabledObjects, enabledObjects;
//         private ObjectPool(GameObject gameObject, int startSize, int expansionFactor)
//         {
//             objectsParent = new GameObject("Pool of: " + gameObject.name).transform;
//             enabledObjects = new LinkedList<GameObject>();
//             for(int i = 0; i < startSize; i++)
//             {
//                 objects.Add(GameObject.Instantiate(gameObject, objectsParent));
//             }

//         }
//     }
// }
