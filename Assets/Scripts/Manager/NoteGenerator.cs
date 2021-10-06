using System.Collections;
using System.Collections.Generic;
using Core;
using Core.Level;
using UnityEngine;
using UnityEngine.Serialization;

namespace Manager {
    public class NoteGenerator : MonoBehaviour {
        public const double NoteGenerationBeat = -16;
        public Queue<NoteData> NoteDatas;

        public GameObject normalNotePrefab;
        public GameObject longNotePrefab;

        public void PrepareGeneratingNotes(List<NoteData> datas) {
            Debug.Log("Preparing generating notes");
            datas.Sort((data, noteData) =>
                data.StartBeat < noteData.StartBeat ? 1 : data.StartBeat < noteData.StartBeat ? -1 : 0);
            NoteDatas = new Queue<NoteData>();
            foreach (var data in datas) {
                NoteDatas.Enqueue(data);
            }

            Debug.Log("Preparing done");
        }

        public void GenerateNotes(List<NoteData> datas) {
            PrepareGeneratingNotes(datas);
            StartCoroutine(GenerateNotesCo());
        }

        public IEnumerator GenerateNotesCo() {
            while (NoteDatas.Count > 0 && PlayManager.Instance.isPlayingLevel) {
                while (NoteDatas.Count > 0 &&
                       NoteDatas.Peek().StartBeat + NoteGenerationBeat <= PlayManager.Instance.CurrentBeat) {
                    GenerateNote(NoteDatas.Dequeue());
                }
                yield return null;
            }
        }

        public void GenerateNote(NoteData data) {
            GameObject note;
            switch (data.NoteType) {
                case NoteType.Normal:
                    note = Instantiate(normalNotePrefab, transform);
                    break;
                case NoteType.Long:
                    note = Instantiate(longNotePrefab, transform);
                    break;
                default:
                    Debug.LogError($"Cannot find noteType {data.NoteType}");
                    return;
            }
            note.GetComponent<NoteBase>().Init(data);
            Debug.Log("Generated Note.");
        }
    }
}