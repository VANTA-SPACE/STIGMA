using System;
using System.Collections.Generic;
using Utils;

namespace Core.Level {
    [Serializable]
    public struct NoteData: IEncodable<Dictionary<string, object>> {
        public NoteType NoteType;
        public double StartBeat;
        public NotePos NotePos;
        public Dictionary<string, object> AdditionalData;

        public NoteData(NoteType noteType, double startBeat, NotePos notePos, Dictionary<string, object> additionalData = null) {
            NoteType = noteType;
            StartBeat = startBeat;
            NoteType = noteType;
            NotePos = notePos;
            AdditionalData = additionalData ?? new Dictionary<string, object>();
        }

        public NoteData(Dictionary<string, object> data) {
            NoteType = default;
            StartBeat = default;
            NotePos = default;
            AdditionalData = default;
            Decode(data);
        }
        
        public Dictionary<string, object> Encode() {
            return new Dictionary<string, object> {
                {"NoteType", NoteType},
                {"StartBeat", StartBeat},
                {"NotePos", (int) NotePos},
                {"AdditionalData", AdditionalData ?? new Dictionary<string, object>()},
            };
        }

        public void Decode(Dictionary<string, object> data) {
            NoteType = data["NoteType"].As<NoteType>();
            StartBeat = data["StartBeat"].As<double>();
            NotePos = (NotePos) data["NotePos"].As<int>();
            AdditionalData = data["AdditionalData"].As<Dictionary<string, object>>() ?? new Dictionary<string, object>();
        }
    }
}