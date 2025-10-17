using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DesignPatterns
{
    public static class TagExtensions
    {
        public static bool HasTags(this GameObject self, params string[] checkTags)
        {
            if (checkTags.Length == 0) return true;
            Tags tags = self.GetComponent<Tags>();
            if (tags == null) return false;
            return tags.HasTags(checkTags);
        }

        public static bool ParentsHaveTags(this GameObject self, params string[] checkTags)
        {
            if (checkTags.Length == 0) return true;
            List<string> checkTagsList = checkTags.ToList();
            GameObject current = self;
            while (checkTagsList.Count > 0 && current != null)
            {
                if (current.TryGetComponent(out Tags tags)) tags.SubtractOwnTagsFrom(checkTagsList);
                current = current.transform.parent?.gameObject;
            }
            return checkTagsList.Count == 0;
        }



        public static T GetComponentWithTagsInChildren<T>(this GameObject self, params string[] checkTags) where T : Component
        {
            return GetComponentWithTagsInChildren<T>(self, checkTags.ToList(), new List<string>());
        }

        public static T GetComponentWithTagsInChildren<T>(this GameObject self, List<string> remainingTags, List<string> foundTags) where T : Component
        {
            int previousFoundTagsCount = foundTags.Count;
            if (self.TryGetComponent(out Tags tags)) tags.SubtractOwnTagsFrom(remainingTags, foundTags);
            if (remainingTags.Count == 0) return self.GetComponent<T>();
            T componentFound = null;
            for (int i = 0; i < self.transform.childCount; i++)
            {
                componentFound = GetComponentWithTagsInChildren<T>(self.transform.GetChild(i).gameObject, remainingTags, foundTags);
                if (componentFound != null) break;
            }
            if (componentFound == null)
            {
                remainingTags.AddRange(foundTags.GetRange(previousFoundTagsCount, foundTags.Count - previousFoundTagsCount));
                foundTags.RemoveRange(previousFoundTagsCount, foundTags.Count - previousFoundTagsCount);
            }
            return componentFound;
        }
    }
}
