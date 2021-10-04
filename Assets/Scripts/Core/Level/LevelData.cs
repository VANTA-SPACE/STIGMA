using System.Collections.Generic;
using System.Linq;

namespace Core.Level {
    public class LevelData: IEncodable<Dictionary<string, object>> {
        public double BPM;
        public List<NoteData> NoteDatas;
        
        public LevelData(double bpm, List<NoteData> noteDatas) {
            BPM = bpm;
            NoteDatas = noteDatas;
        }

        public LevelData(Dictionary<string, object> data) => Decode(data);
        
        public Dictionary<string, object> Encode() {
            var result = new Dictionary<string, object> {
                {"BPM", BPM},
                {"NoteDatas", NoteDatas.Select(data => data.Encode() as object).ToList()}
            };
            return result;
        }

        public void Decode(Dictionary<string, object> data) {
            this.BPM = (double) data["BPM"];
            this.NoteDatas = ((List<object>) data["NoteDatas"])
                .Select(o => new NoteData((Dictionary<string, object>) o)).ToList();
        }
    }
}