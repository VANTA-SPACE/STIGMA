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
        
        public EventInstance EventInstance;
        private PARAMETER_ID _parameterID;
        public bool Playing { get; private set; }
        public bool Paused { get; private set; }

        private void Awake() {
            if (_instance != null) {
                Destroy(gameObject);
                return;
            }

            SceneManager.sceneLoaded += (scene, mode) => {
                StopEvent();
            };
            _instance = this;
        }

        public void PlayMainEvent(string eventName, double offset = 0) {
            this.offset = offset * PlayManager.Instance.currentBpm / 6000;
            StartCoroutine(_playMainEventCo(eventName));
        }

        private IEnumerator _playMainEventCo(string eventName) {
            yield return new WaitUntil(() => PlayManager.Instance.currentBeat >= offset);
            if (Playing) {
                RuntimeManager.DetachInstanceFromGameObject(EventInstance);
            }
            
            Playing = true;

            EventInstance = RuntimeManager.CreateInstance(eventName);
            EventInstance.start();

            RuntimeManager.AttachInstanceToGameObject(EventInstance, transform);

            EventInstance.getDescription(out var titleEventDescription);
            titleEventDescription.getParameterDescriptionByName("Transition", out var titleParameterDescription);
            _parameterID = titleParameterDescription.id;
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

        public void Unpause(bool fixSinc = false) {
            EventInstance.setPaused(false);
            if (fixSinc) {
                int timelinePos = (int) Math.Round((PlayManager.Instance.currentBeat - offset) * 60 /
                    PlayManager.Instance.levelData.BPM * 1000);
                EventInstance.setTimelinePosition(timelinePos);
            }

            Paused = false;
        }
    }
}