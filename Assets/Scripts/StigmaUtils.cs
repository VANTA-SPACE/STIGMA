using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class StigmaUtils {
    public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value) {
        key = pair.Key;
        value = pair.Value;
    }
    
    public static Dictionary<TKey, TValue> Copy<TKey, TValue>(this Dictionary<TKey, TValue> original) {
        var result = new Dictionary<TKey, TValue>();
        foreach (var (key, value) in original) {
            result[key] = value;
        }

        return result;
    }

    public static T As<T>(this object value) {
        if (value is T result) return result;
        
        var type = typeof(T);

        if (type == typeof(bool)) {
            return (T) (object) Convert.ToBoolean(value);
        }
        if (type == typeof(char)) {
            return (T) (object) Convert.ToChar(value);
        }
        if (type == typeof(byte)) {
            return (T) (object) Convert.ToByte(value);
        }
        if (type == typeof(sbyte)) {
            return (T) (object) Convert.ToSByte(value);
        }
        if (type == typeof(short)) {
            return (T) (object) Convert.ToInt16(value);
        }
        if (type == typeof(ushort)) {
            return (T) (object) Convert.ToUInt32(value);
        }
        if (type == typeof(int)) {
            return (T) (object) Convert.ToInt32(value);
        }
        if (type == typeof(uint)) {
            return (T) (object) Convert.ToUInt32(value);
        }
        if (type == typeof(long)) {
            return (T) (object) Convert.ToInt64(value);
        }
        if (type == typeof(ulong)) {
            return (T) (object) Convert.ToUInt64(value);
        }
        if (type == typeof(float)) {
            return (T) (object) Convert.ToSingle(value);
        }
        if (type == typeof(double)) {
            return (T) (object) Convert.ToDouble(value);
        }
        if (type == typeof(decimal)) {
            return (T) (object) Convert.ToDecimal(value);
        }

        return (T) Convert.ChangeType(value, type);
    }
}