using System.Collections.Generic;

namespace DesignPatterns
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrAdd<TKey,TValue>(this Dictionary<TKey,TValue> self, TKey key, TValue addValue)
        {
            TValue value;
            if(!self.TryGetValue(key, out value))
            {
                value = addValue;
                self.Add(key, value);
            }
            return value;
        }
    }
}