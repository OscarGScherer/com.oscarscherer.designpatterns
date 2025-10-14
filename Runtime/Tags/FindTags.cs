using UnityEngine;

namespace DesignPatterns
{
    public class FindTags<T> where T : Component
    {
        private readonly string[] tags;
        private MonoBehaviour findFrom;

        private T _value;

        public T this[MonoBehaviour findFrom]
        {
            get
            {
                if (findFrom == null) return default;
                if (this.findFrom == null) this.findFrom = findFrom;
                if (_value == null) _value = findFrom.gameObject.GetComponentWithTagsInChildren<T>(true, tags);
                return _value;
            }
        }

        public static implicit operator T(FindTags<T> tagFind) => tagFind._value;

        public FindTags(params string[] tags)
        {
            this.tags = tags;
        }
    }
}
