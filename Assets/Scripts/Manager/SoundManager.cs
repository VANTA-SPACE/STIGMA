using System;
using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Manager {
    public class SoundManager : MonoBehaviour {
        public static SoundManager Instance => _instance;
        private static SoundManager _instance;
        public double offset;

        public string currentEventName;
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

            SceneManager.sceneLoaded += (scene, mode) => { StopEvent(); };
            _instance = this;
        }

        public void PlayLevelEvent(string eventName, double msOffset = 0) {
            Debug.Log($"Play {eventName}");
            offset = msOffset;
            currentEventName = eventName;
            StartCoroutine(_playMainEventCo(eventName));
        }

        private IEnumerator _playMainEventCo(string eventName) {
            if (offset == 0) yield return new WaitUntil(() => true);
            else yield return new WaitUntil(() => PlayManager.Instance.CurrentMilisec >= offset);
            if (Playing) {
                RuntimeManager.DetachInstanceFromGameObject(EventInstance);
            }

            Playing = true;

            // event:/Scene_Intro

            EventInstance = RuntimeManager.CreateInstance("event:/" + eventName);
            EventInstance.getDescription(out EventDescription);
            EventDescription.getLength(out Length);
            SetVolume(Settings.MasterVolume / 100f);
            EventInstance.start();

            RuntimeManager.AttachInstanceToGameObject(EventInstance, transform);
        }


        public void PlayEvent(string eventName) {
            PlayLevelEvent(eventName, 0);
        }

        public void EditParameter(String parameterName, float value) {
            EventDescription.getParameterDescriptionByName(parameterName, out var titleParameterDescription);
            EventInstance.setParameterByID(titleParameterDescription.id, value);
        }

        public void StopEvent(bool fadeOut = false) {
            Debug.Log("Stop event");
            EventInstance.stop(fadeOut ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);
            RuntimeManager.DetachInstanceFromGameObject(EventInstance);
            Playing = false;
        }

        public void SetVolume(float volume) {
            EventInstance.setVolume(volume);
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