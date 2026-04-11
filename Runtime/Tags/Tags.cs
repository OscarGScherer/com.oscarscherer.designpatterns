using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace DesignPatterns
{
    [DisallowMultipleComponent()]
    public class Tags : MonoBehaviour
    {
        private static Dictionary<string, UnityEvent<GameObject>> tagAddEvents = new Dictionary<string, UnityEvent<GameObject>>();
        private static Dictionary<string, UnityEvent<GameObject>> tagRemoveEvents = new Dictionary<string, UnityEvent<GameObject>>();

        [SerializeField] private List<string> tagAddQueue = new List<string>();
        private List<string> tagRemoveQueue = new List<string>();
        private List<string> activeTags = new List<string>();

        public List<string> GetTagsCopy() => 
            new List<string>(activeTags.Count == 0 ? tagAddQueue : activeTags);

        public void AddTag(string tag)
        {
            if (tagAddQueue.Contains(tag)) return;
            tagAddQueue.Add(tag);
            tagRemoveQueue.Remove(tag);
            if (this.didStart) AddTagsFromQueue();
        }

        public void RemoveTag(string tag)
        {
            tagAddQueue.Remove(tag);
            // If you are removing a tag, and there is no active tag, don't bother
            if (activeTags.Count == 0) return;
            tagRemoveQueue.Add(tag);
            if (this.didStart) RemoveTagsFromQueue();
        }

        void Start()
        {
            AddTagsFromQueue();
        }

        void AddTagsFromQueue()
        {
            while (tagAddQueue.Count > 0)
            {
                if (!activeTags.Contains(tagAddQueue[0]))
                {
                    activeTags.Add(tagAddQueue[0]);
                    if (tagAddEvents.TryGetValue(tag, out var evt)) 
                        evt.Invoke(gameObject);
                }
                tagAddQueue.RemoveAt(0);
            }
        }

        void RemoveTagsFromQueue()
        {
            while (tagRemoveQueue.Count > 0)
            {
                if (activeTags.Remove(tagRemoveQueue[0]))
                    if (tagRemoveEvents.TryGetValue(tag, out var evt)) 
                        evt.Invoke(gameObject);
                tagRemoveQueue.RemoveAt(0);
            }
        }

        private bool HasTag(string tag) => activeTags.Contains(tag);

        public bool HasTags(params string[] checkTags)
        {
            AddTagsFromQueue();
            for (int i = 0; i < activeTags.Count; i++)
                if (!HasTag(checkTags[i]))
                    return false;
            return true;
        }

        public void SubtractOwnTagsFrom(List<string> subractFrom, List<string> subtractedTags = null)
        {
            AddTagsFromQueue();
            for (int i = 0; i < activeTags.Count; i++)
            {
                if (subractFrom.Remove(activeTags[i]))
                    subtractedTags?.Add(activeTags[i]);
            }
        }

        public static void RegisterToTagAddEvent(string tag, UnityAction<GameObject> unityAction) => tagAddEvents.GetOrAdd(tag, new()).AddListener(unityAction);

        public static void RegisterToRemoveAddEvent(string tag, UnityAction<GameObject> unityAction) => tagRemoveEvents.GetOrAdd(tag, new()).AddListener(unityAction);
    }
}
