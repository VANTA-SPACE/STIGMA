using System;
using System.Collections.Generic;

namespace Utils {
    [Serializable]
    public struct SerializableKeyValuePair<TKey, TValue> {
        public SerializableKeyValuePair(TKey key, TValue value) {
            this.Key = key;
            this.Value = value;
        }

        public TKey Key;
        public TValue Value;

        public override string ToString() {
            return ((KeyValuePair<TKey, TValue>) this).ToString();
        }

        public void Deconstruct(out TKey key, out TValue value) {
            key = this.Key;
            value = this.Value;
        }

        public static implicit operator KeyValuePair<TKey, TValue>(SerializableKeyValuePair<TKey, TValue> pair) {
            return new KeyValuePair<TKey, TValue>(pair.Key, pair.Value);
        }
        
        public static implicit operator SerializableKeyValuePair<TKey, TValue>(KeyValuePair<TKey, TValue> pair) {
            return new SerializableKeyValuePair<TKey, TValue>(pair.Key, pair.Value);
        }
    }
}