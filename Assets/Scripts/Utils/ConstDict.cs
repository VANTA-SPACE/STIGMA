using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Utils {
    public class ConstDict<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> {
        public ConstDict(params (TKey key, TValue value)[] pairs) {
            foreach (var (key, value) in pairs) {
                values[key] = value;
            }
        }

        private readonly Dictionary<TKey, TValue> values = new Dictionary<TKey, TValue>();
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => values.Count;
        public bool ContainsKey(TKey key) => values.ContainsKey(key);

        public bool TryGetValue(TKey key, out TValue value) => values.TryGetValue(key, out value);

        public TValue this[TKey key] => values[key];

        public ICollection<TKey> Keys => values.Keys;
        public ICollection<TValue> Values => values.Values;
    }
}