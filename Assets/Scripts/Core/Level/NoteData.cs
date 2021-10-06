using System.Collections.Generic;
using Serialization;
using UnityEngine;
using Utils;

namespace Core.Level {
    public struct NoteData: IEncodable<Dictionary<string, object>> {
        public NoteType NoteType;
        public double StartBeat;
        public NotePos NotePos;
        private Dictionary<string, object> additionalData;
        public object this[string key] => additionalData.ContainsKey(key) ? additionalData[key] : null;

        public NoteData(NoteType noteType, double startBeat, NotePos notePos, Dictionary<string, object> additionalData = null) {
            this.NoteType = noteType;
            this.StartBeat = startBeat;
            this.NoteType = noteType;
            this.NotePos = notePos;
            this.additionalData = additionalData ?? new Dictionary<string, object>();
        }

        public NoteData(Dictionary<string, object> data) {
            NoteType = default;
            StartBeat = default;
            NotePos = default;
            additionalData = default;
            Decode(data);
        }
        
        public Dictionary<string, object> Encode() {
            var ad = additionalData ?? new Dictionary<string, object>();
            ad["NoteType"] = NoteType;
            ad["StartBeat"] = StartBeat;
            ad["NotePos"] = NotePos;
            return ad;
        }

        public void Decode(Dictionary<string, object> data) {
            NoteType = data["NoteType"].As<NoteType>();
            StartBeat = data["StartBeat"].As<double>();
            NotePos = (NotePos) data["NotePos"].As<int>();
            additionalData = data;
            Debug.Log($"Data is {Json.Serialize(data)}");
        }
    }
}