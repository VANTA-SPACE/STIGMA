using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Utils {
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue> {
        public List<SerializableKeyValuePair<TKey, TValue>> pairs = new List<SerializableKeyValuePair<TKey, TValue>>();

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() =>
            pairs.Select((pair => (KeyValuePair<TKey, TValue>) pair)).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) {
            Add(item.Key, item.Value);
        }

        public void Clear() {
            pairs.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) {
            return pairs.Any(pair => pair.Key.Equals(item.Key) && pair.Value.Equals(item.Value));
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            pairs.Select(pair => (KeyValuePair<TKey, TValue>) pair).ToList().CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) {
            var result = false;
            foreach (var pair in pairs.ToArray()) {
                if (pair.Key.Equals(item.Key) && pair.Value.Equals(item.Value)) {
                    pairs.Remove(pair);
                    result = true;
                }
            }

            return result;
        }

        public int Count => pairs.Count;
        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value) {
            if (key == null) {
                throw new ArgumentNullException(nameof(key));
            }

            if (ContainsKey(key)) {
                throw new ArgumentException($"An item with the same key has already been added. Key: {key}");
            }

            pairs.Add(new SerializableKeyValuePair<TKey, TValue>(key, value));
        }

        public bool ContainsKey(TKey key) {
            return pairs.Any(pair => pair.Key.Equals(key));
        }

        public bool Remove(TKey key) {
            var result = false;
            foreach (var pair in pairs.ToArray()) {
                if (pair.Key.Equals(key)) {
                    pairs.Remove(pair);
                    result = true;
                }
            }

            return result;
        }

        public bool TryGetValue(TKey key, out TValue value) {
            throw new NotImplementedException();
        }

        public TValue this[TKey key] {
            get {
                try {
                    return pairs.First(pair => pair.Key.Equals(key)).Value;
                } catch {
                    throw new KeyNotFoundException();
                }
            }
            set {
                Remove(key);
                Add(key, value);
            }
        }

        public ICollection<TKey> Keys => pairs.Select(pair => pair.Key).ToArray();
        public ICollection<TValue> Values => pairs.Select(pair => pair.Value).ToArray();

        public static explicit operator Dictionary<TKey, TValue>(SerializableDictionary<TKey, TValue> dictionary) {
            var result = new Dictionary<TKey, TValue>();
            foreach (var (key, value) in dictionary) {
                result[key] = value;
            }

            return result;
        }

        public static explicit operator SerializableDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary) {
            var result = new SerializableDictionary<TKey, TValue>();
            foreach (var (key, value) in dictionary) {
                result[key] = value;
            }

            return result;
        }
    }
}