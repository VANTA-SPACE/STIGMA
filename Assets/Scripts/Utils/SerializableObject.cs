/*
 using System;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine.Serialization;

namespace Utils {
    [Serializable]
    public class SerializableObject {
        //size: 1
        public bool boolData;
        public byte byteData;
        public sbyte sbyteData;

        //size: 2
        public char charData;
        public short shortData;
        public ushort ushortData;

        //size: 4
        public int intData;
        public uint uintData;
        public float floatData;

        //size: 8
        public long longData;
        public ulong ulongData;
        public double doubleData;

        public string stringData;
        public object OtherData;

        public DataType type;

        public enum DataType {
            NULL,
            BOOL,
            BYTE,
            SBYTE,
            CHAR,
            DOUBLE,
            FLOAT,
            INT,
            UINT,
            LONG,
            ULONG,
            SHORT,
            USHORT,
            STRING,
            OTHER
        }

        public SerializableObject(object data) {
            boolData = default;
            byteData = default;
            sbyteData = default;
            charData = default;
            doubleData = default;
            floatData = default;
            intData = default;
            uintData = default;
            longData = default;
            ulongData = default;
            shortData = default;
            ushortData = default;
            stringData = default;
            OtherData = default;
            type = default;
            if (data == null) {
                type = DataType.NULL;
                return;
            }

            switch (data) {
                case bool b:
                    boolData = b;
                    type = DataType.BOOL;
                    break;
                case byte b:
                    byteData = b;
                    type = DataType.BYTE;
                    break;
                case sbyte s:
                    sbyteData = s;
                    type = DataType.SBYTE;
                    break;
                case char c:
                    charData = c;
                    type = DataType.CHAR;
                    break;
                case double d:
                    doubleData = d;
                    type = DataType.DOUBLE;
                    break;
                case float f:
                    floatData = f;
                    type = DataType.FLOAT;
                    break;
                case int i:
                    intData = i;
                    type = DataType.INT;
                    break;
                case uint u:
                    uintData = u;
                    type = DataType.UINT;
                    break;
                case long l:
                    longData = l;
                    type = DataType.LONG;
                    break;
                case ulong u:
                    ulongData = u;
                    type = DataType.ULONG;
                    break;
                case short s:
                    shortData = s;
                    type = DataType.SHORT;
                    break;
                case ushort u:
                    ushortData = u;
                    type = DataType.USHORT;
                    break;

                case string s:
                    stringData = s;
                    type = DataType.STRING;
                    break;

                default:
                    OtherData = data;
                    type = DataType.OTHER;
                    break;
            }
        }

        public static object ToObject(SerializableObject o) {
            switch (o.type) {
                case DataType.NULL:
                    return null;

                case DataType.BOOL:
                    return o.boolData;

                case DataType.BYTE:
                    return o.byteData;

                case DataType.SBYTE:
                    return o.sbyteData;

                case DataType.CHAR:
                    return o.charData;
                case DataType.DOUBLE:
                    return o.doubleData;

                case DataType.FLOAT:
                    return o.floatData;

                case DataType.INT:
                    return o.intData;
                case DataType.UINT:
                    return o.uintData;

                case DataType.LONG:
                    return o.longData;

                case DataType.ULONG:
                    return o.ulongData;

                case DataType.SHORT:
                    return o.shortData;

                case DataType.USHORT:
                    return o.ushortData;

                case DataType.STRING:
                    return o.stringData;

                case DataType.OTHER:
                    return o.OtherData;
            }

            return null;
        }

        public object ToObject() => ToObject(this);
    }
}
*/