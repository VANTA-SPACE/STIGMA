using System;
using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Manager {
    public class SoundManager2 : MonoBehaviour
    {
        public static SoundManager2 Instance => _instance;
        private static SoundManager2 _instance;
        public double offset;
        
        public EventInstance EventInstance;
        public EventDescription EventDescription;
        [NonSerialized] public int Length;
        
        public bool Playing { get; private set; }
        public bool Paused { get; private set; }

        private void Awake() {
            // RuntimeManager.LoadBank("Master");
            // RuntimeManager.WaitForAllLoads();
            if (_instance != null) {
                Destroy(gameObject);
                return;
            }

            SceneManager.sceneLoaded += (scene, mode) => {
                StopEvent();
            };
            _instance = this;
        }

        public void StopEvent(bool fadeOut = false) {
            EventInstance.stop(fadeOut ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);
            RuntimeManager.DetachInstanceFromGameObject(EventInstance);
            Playing = false;
        }

        public void Pause() {
            EventInstance.setPaused(true);
            Paused = true;
        }

        public void Unpause(bool fixSync = false) {
            EventInstance.setPaused(false);
            if (fixSync) {
                int timelinePos = (int) (PlayManager.Instance.CurrentMilisec - offset);
                EventInstance.setTimelinePosition(timelinePos);
            }
            Paused = false;
        }
    }
}