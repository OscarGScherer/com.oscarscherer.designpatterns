using System;
using UnityEngine;

namespace DesignPatterns
{
    [Serializable]
    public class FindTags<T> where T : Component
    {
        private readonly string[] tags;
        [HideInInspector] [SerializeField] private MonoBehaviour findFrom;
        [SerializeField] private T _value;

        public T Find(MonoBehaviour findFrom)
        {
            if (findFrom == null) return default;
            if (_value == null || this.findFrom != findFrom)
            {
                this.findFrom = findFrom;
                _value = findFrom.gameObject.GetComponentWithTagsInChildren<T>(tags);
                if (_value == null)
                    Debug.LogWarning($"TagFind: Gameobject {findFrom.name} requested a {typeof(T)} with tags {String.Join(", ", tags)} but did not find it.");
            }
            return _value;
        }

        public static implicit operator T(FindTags<T> tagFind) => tagFind._value;

        public FindTags(params string[] tags)
        {
            this.tags = tags;
        }
    }
}
