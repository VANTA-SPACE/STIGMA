using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Core;
using Locale;
using UnityEngine;
using UnityEngine.Networking;

namespace Utils {
    public static class StigmaUtils {
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key,
            out TValue value) {
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

        public static Dictionary<TKey2, TValue2> As<TKey1, TValue1, TKey2, TValue2>(this object dictionary) {
            var dict = dictionary.As<Dictionary<TKey1, TValue1>>();
            if (dict == null) return null;
            var result = new Dictionary<TKey2, TValue2>();
            foreach (var (key, value) in dict) {
                if (key == null || value == null) continue;
                result[key.As<TKey2>()] = value.As<TValue2>();
            }

            return result;
        }

        public static T As<T>(this object value, T defaultValue = default) {
            if (value is null) return defaultValue;
            var type = typeof(T);
            if (type.IsSubclassOf(typeof(Nullable<>))) {
                type = type.GenericTypeArguments[0];
            }

            if (value is T result) return result;


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

            try {
                return (T) Convert.ChangeType(value, type);
            } catch {
                return defaultValue;
            }
        }

        public static bool CheckMiss(double timeOffset) {
            var frameOffset = timeOffset / Time.deltaTime;
            return frameOffset > Constants.NOTEJUDGMENT_BAD;
        }

        public static Judgment GetJudgement(double timeOffset) {
            var frameOffset = timeOffset / Time.deltaTime;
            if (frameOffset < -Constants.NOTEJUDGMENT_BAD) return Judgment.None;
            else if (frameOffset < -Constants.NOTEJUDGMENT_NORMAL) return Judgment.Bad;
            else if (frameOffset < -Constants.NOTEJUDGMENT_NORMAL + Constants.NOTEJUDGMENT_ELOFFSET)
                return Judgment.GoodEarly;
            else if (frameOffset < -Constants.NOTEJUDGMENT_PERFECT) return Judgment.Good;
            else if (frameOffset < -Constants.NOTEJUDGMENT_PERFECT + Constants.NOTEJUDGMENT_ELOFFSET)
                return Judgment.PerfectEarly;
            else if (frameOffset <= Constants.NOTEJUDGMENT_PERFECT - Constants.NOTEJUDGMENT_ELOFFSET)
                return Judgment.Perfect;
            else if (frameOffset <= Constants.NOTEJUDGMENT_PERFECT) return Judgment.PerfectLate;
            else if (frameOffset <= Constants.NOTEJUDGMENT_NORMAL - Constants.NOTEJUDGMENT_ELOFFSET)
                return Judgment.Good;
            else if (frameOffset <= Constants.NOTEJUDGMENT_NORMAL) return Judgment.GoodLate;
            else if (frameOffset <= Constants.NOTEJUDGMENT_BAD) return Judgment.Bad;
            else return Judgment.Miss;
        }

        public static (double min, double max) GetJudgmentMilisec(this Judgment judgment) {
            double frameMin;
            double frameMax;
            switch (judgment) {
                case Judgment.Perfect:
                    frameMax = Constants.NOTEJUDGMENT_PERFECT - Constants.NOTEJUDGMENT_ELOFFSET;
                    frameMin = -frameMax;
                    break;
                case Judgment.PerfectEarly:
                    frameMin = -Constants.NOTEJUDGMENT_PERFECT;
                    frameMax = -Constants.NOTEJUDGMENT_PERFECT + Constants.NOTEJUDGMENT_ELOFFSET;
                    break;
                case Judgment.PerfectLate:
                    frameMin = Constants.NOTEJUDGMENT_PERFECT - Constants.NOTEJUDGMENT_ELOFFSET;
                    frameMax = Constants.NOTEJUDGMENT_PERFECT;
                    break;
                case Judgment.Good:
                    frameMax = Constants.NOTEJUDGMENT_NORMAL - Constants.NOTEJUDGMENT_ELOFFSET;
                    frameMin = -frameMax;
                    break;
                case Judgment.GoodEarly:
                    frameMin = -Constants.NOTEJUDGMENT_NORMAL;
                    frameMax = -Constants.NOTEJUDGMENT_NORMAL + Constants.NOTEJUDGMENT_ELOFFSET;
                    break;
                case Judgment.GoodLate:
                    frameMin = Constants.NOTEJUDGMENT_NORMAL - Constants.NOTEJUDGMENT_ELOFFSET;
                    frameMax = Constants.NOTEJUDGMENT_NORMAL;
                    break;
                case Judgment.Bad:
                    frameMax = Constants.NOTEJUDGMENT_BAD;
                    frameMin = -frameMax;
                    break;
                default:
                    return (float.NaN, float.NaN);
            }

            return (Time.deltaTime * frameMin, Time.deltaTime * frameMax);
        }

        public static Vector2 Rotate(this Vector2 v, float delta) {
            if (delta == 0) return v;
            delta = Mathf.Deg2Rad * delta;
            return new Vector2(
                (float) Math.Round(v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta), 6),
                (float) Math.Round(v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta), 6)
            );
        }

        public static string SplitCapital(this string str) {
            var result = new StringBuilder();
            bool start = false;
            foreach (var chr in str) {
                if (start && char.IsUpper(chr)) result.Append(" ");
                start = true;
                result.Append(chr);
            }

            return result.ToString();
        }

        public static string FirstCapital(this string str) {
            var builder = new StringBuilder();
            var flag = true;
            foreach (var chr in str) {
                if (flag) {
                    flag = false;
                    builder.Append(chr.ToString().ToUpper());
                } else {
                    builder.Append(chr.ToString().ToLower());
                }
            }

            return builder.ToString();
        }

        public static string Encode(this string str, Encoding from, Encoding to) {
            var bytes = from.GetBytes(str);
            bytes = Encoding.Convert(from, to, bytes);
            return to.GetString(bytes);
        }

        public static string Encode(this byte[] bytes, Encoding to) {
            return to.GetString(bytes);
        }

        public static string Encode(this byte[] bytes, Encoding from, Encoding to) {
            bytes = Encoding.Convert(from, to, bytes);
            return to.GetString(bytes);
        }

        public static void ReturnValue<T>(this CoroutineResult<T> coroutineResult, T value) {
            if (coroutineResult == null) return;
            coroutineResult.Value = value;
            coroutineResult.Callback?.Invoke(value);
        }

        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key,
            TValue defaultValue = default) {
            if (dict.TryGetValue(key, out var result)) return result;
            return defaultValue;
        }

        public static IEnumerator SetDelay(this Action action, float delay) {
            yield return new WaitForSeconds(delay);
            action();
        }

        public static IEnumerator SetDelay(this Func<IEnumerable> action, float delay) {
            yield return new WaitForSeconds(delay);
            yield return action();
        }

        public static IEnumerator SetUntil(this Action action, Func<bool> delay) {
            yield return new WaitUntil(delay);
            action();
        }

        public static IEnumerator SetUntil(this Func<IEnumerable> action, Func<bool> delay) {
            yield return new WaitUntil(delay);
            yield return action();
        }

        public static Comparison<T1> Compare<T1, T2>(Func<T1, T2> toCompare, bool ascending = true)
            where T2 : IComparable<T2> {
            return (x, y) => {
                var compx = toCompare(x);
                var compy = toCompare(y);
                int result = compx.CompareTo(compy);
                if (!@ascending) result = -result;
                return result;
            };
        }

        public static void Sort<T1, T2>(this List<T1> list, Func<T1, T2> toCompare, bool ascending = true)
            where T2 : IComparable<T2> {
            var comparsion = Compare(toCompare, ascending);
            list.Sort(comparsion);
        }

        public static Color WithAlpha(this Color color, float alpha) {
            return new Color(color.r, color.g, color.b, alpha);
        }
        
        public static Color32 WithAlpha(this Color32 color, byte alpha) {
            return new Color32(color.r, color.g, color.b, alpha);
        }

        public static string GetEnumName<T>(this T @enum, Language? language = null) where T : Enum {
            if (language == null) language = Settings.CurrentLanguage;
            var type = @enum.GetType();
            Debug.Log($"Key: Enum.{type.Name}.{@enum}");
            return Translate.TryGet($"Enum.{type.Name}.{@enum}", out string result, language) ? result : @enum.ToString();
        }
    }
}