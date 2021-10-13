using System.Collections.Generic;
using Serialization;
using UnityEngine;
using Utils;

namespace Core.Level {
    public struct NoteData: IEncodable<Dictionary<string, object>> {
        public NoteType NoteType;
        public double StartMilisec;
        public double NoteLength;
        public NotePos NotePos;
        public LevelData LevelData;
        public Dictionary<string, object> AdditionalData;
        public object this[string key] => AdditionalData.ContainsKey(key) ? AdditionalData[key] : null;

        public double EndMilisec => StartMilisec + NoteLength;
        public double StartBeat => StartMilisec * MilisecToBeat;
        public double EndBeat => EndMilisec * MilisecToBeat;
        
        
        public double MilisecToBeat => LevelData.Bpm / 60 / 1000;
        public float MilisecToBeatF => (float) LevelData.Bpm / 60 / 1000;
        
        public double BeatToSecond => 60 / LevelData.Bpm;
        public float BeatToSecondF => 60 / (float) LevelData.Bpm;

        public NoteData(NoteType noteType, double startMilisec, NotePos notePos, LevelData levelData, double noteLength = 0, Dictionary<string, object> additionalData = null) {
            NoteType = noteType;
            StartMilisec = startMilisec;
            NoteType = noteType;
            NotePos = notePos;
            LevelData = levelData;
            NoteLength = noteLength;
            this.AdditionalData = additionalData ?? new Dictionary<string, object>();
        }

        public NoteData(Dictionary<string, object> data, LevelData levelData) {
            LevelData = default;
            NoteType = default;
            StartMilisec = default;
            NotePos = default;
            AdditionalData = default;
            NoteLength = default;
            LevelData = levelData;
            Decode(data);
        }
        
        public Dictionary<string, object> Encode() {
            var ad = AdditionalData ?? new Dictionary<string, object>();
            ad["NoteType"] = NoteType;
            ad["StartMilisec"] = StartMilisec;
            ad["NotePos"] = NotePos;
            return ad;
        }

        public void Decode(Dictionary<string, object> data) {
            NoteType = data["NoteType"].As<NoteType>();
            if (data.ContainsKey("StartBeat")) {
                StartMilisec = data["StartBeat"].As<double>() * BeatToSecond * 1000;
            } else {
                StartMilisec = data["StartMilisec"].As<double>();
            }
            if (data.ContainsKey("BeatLength")) {
                NoteLength = data["BeatLength"].As(0.0d) * BeatToSecond * 1000;
            } else {
                NoteLength = data.GetOrDefault("MilisecLength").As<double>();
            }
            NotePos = (NotePos) data["NotePos"].As<int>();
            AdditionalData = data;
        }
    }
}