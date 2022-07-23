
using System.Collections.Generic;

namespace W
{
    public static class DictionaryExtension
    {
        public static V GetOrCreate<K, V>(this Dictionary<K, V> dict, K key) where V : new() {
            if (!dict.TryGetValue(key, out V v)) {
                v = new V();
                dict.Add(key, v);
            }
            return v;
        }
        public static V GetOrNull<K, V>(this Dictionary<K, V> dict, K key) where V : class {
            if (!dict.TryGetValue(key, out V v)) {
                v = null;
            }
            return v;
        }
    }
}
