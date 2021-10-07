using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Core.Level {
    public class LevelData: IEncodable<Dictionary<string, object>> {
        public double Bpm;
        public double Offset;
        public List<NoteData> NoteDatas;
        public string EventName;
        public double EndMilisec;

        public LevelData(double bpm, double offset, List<NoteData> noteDatas, string eventName) {
            Bpm = bpm;
            NoteDatas = noteDatas;
            Offset = offset;
            EventName = eventName;
            
            NoteDatas.Sort(n => n.StartBeat);
        }

        public LevelData(Dictionary<string, object> data) {
            Decode(data);
        }

        public Dictionary<string, object> Encode() {
            var result = new Dictionary<string, object> {
                {"Bpm", Bpm},
                {"Offset", Offset},
                {"NoteDatas", NoteDatas.Select(data => data.Encode() as object).ToList()},
                {"EventName", EventName}
            };
            return result;
        }

        public void Decode(Dictionary<string, object> data) {
            Bpm = data["Bpm"].As<double>();
            Offset = data["Offset"].As<double>();
            NoteDatas = ((List<object>) data["NoteDatas"])
                .Select(o => new NoteData((Dictionary<string, object>) o, this)).ToList();
            EventName = data["EventName"].As<string>();
            
            NoteDatas.Sort(n => n.StartBeat);
            EndMilisec = NoteDatas.Last().EndMilisec;
        }
    }
}