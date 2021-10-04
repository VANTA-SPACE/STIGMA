using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Core.Level {
    public class LevelData: IEncodable<Dictionary<string, object>> {
        public double BPM;
        public double Offset;
        public List<NoteData> NoteDatas;
        public string EventName;
        
        public LevelData(double bpm, double offset, List<NoteData> noteDatas, string eventName) {
            BPM = bpm;
            NoteDatas = noteDatas;
            Offset = offset;
            EventName = eventName;
        }

        public LevelData(Dictionary<string, object> data) {
            Decode(data);
        }

        public Dictionary<string, object> Encode() {
            var result = new Dictionary<string, object> {
                {"BPM", BPM},
                {"Offset", Offset},
                {"NoteDatas", NoteDatas.Select(data => data.Encode() as object).ToList()},
                {"EventName", EventName}
            };
            return result;
        }

        public void Decode(Dictionary<string, object> data) {
            this.BPM = data["BPM"].As<double>();
            this.Offset = data["Offset"].As<double>();
            this.NoteDatas = ((List<object>) data["NoteDatas"])
                .Select(o => new NoteData((Dictionary<string, object>) o)).ToList();
            this.EventName = data["EventName"].As<string>();
        }
    }
}