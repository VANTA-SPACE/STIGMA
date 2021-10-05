using System;
using System.Collections.Generic;
using UnityEngine;

namespace Serialization {
    public class Csv {
        private List<List<string>> data;
        public int rowCount => columnCount == 0 ? 0 : data[0].Count;
        public int columnCount => data.Count;
        public List<List<string>> Data => data;

        public Csv(string csvFile) {
            var rows = csvFile.Split(new []{"\n"}, StringSplitOptions.RemoveEmptyEntries);
            var cols = new List<List<string>>();

            foreach (var row in rows) {
                var escape = row.Replace("\\", "\\\\");
                escape = escape.Replace("\"\"", "u\\22");
                var split = escape.Split(new[] {",\",", ",\"", "\",", "\""}, StringSplitOptions.RemoveEmptyEntries);
                var finSplit = new List<string>();
                var doEscape = false;
                foreach (var s2 in split) {
                    if (doEscape) {
                        finSplit.Add(s2.Replace("u\\22", "\"").Replace("\\\\", "\\"));
                    } else {
                        var split2 = s2.Split(',');
                        foreach (var s3 in split2) {
                            finSplit.Add(s3.Replace("u\\22", "\"\"").Replace("\\\\", "\\"));
                        }
                    }

                    doEscape = !doEscape;
                }
                cols.Add(finSplit);
            }

            data = cols;
        }

        public string this[int row, int col] => data[row][col];
    }
}