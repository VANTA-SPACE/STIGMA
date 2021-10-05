using System;
using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Manager {
    public class SoundManager : MonoBehaviour {
        public static SoundManager Instance => _instance;
        private static SoundManager _instance;
        public double offset;
        
        private EventInstance _eventInstance;
        private PARAMETER_ID _parameterID;
        public bool Playing { get; private set; }
        public bool Paused { get; private set; }

        private void Awake() {
            if (_instance != null) {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        public void PlayMainEvent(string eventName, double offset = 0) {
            this.offset = offset * PlayManager.Instance.currentBpm / 6000;
            StartCoroutine(_playMainEventCo(eventName));
        }

        private IEnumerator _playMainEventCo(string eventName) {
            yield return new WaitUntil(() => PlayManager.Instance.currentBeat >= offset);
            Debug.LogError("Play Event");
            if (Playing) {
                RuntimeManager.DetachInstanceFromGameObject(_eventInstance);
            }
            
            Playing = true;

            _eventInstance = RuntimeManager.CreateInstance(eventName);
            _eventInstance.start();

            RuntimeManager.AttachInstanceToGameObject(_eventInstance, transform);

            _eventInstance.getDescription(out var titleEventDescription);
            titleEventDescription.getParameterDescriptionByName("Transition", out var titleParameterDescription);
            _parameterID = titleParameterDescription.id;
        }

        public void StopEvent(bool fadeOut = false) {
            _eventInstance.stop(fadeOut ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);
            RuntimeManager.DetachInstanceFromGameObject(_eventInstance);
            Playing = false;
        }

        public void Pause() {
            _eventInstance.setPaused(true);
            Paused = true;
        }

        public void Unpause(bool fixSinc = false) {
            _eventInstance.setPaused(false);
            if (fixSinc) {
                int timelinePos = (int) Math.Round((PlayManager.Instance.currentBeat - offset) * 60 /
                    PlayManager.Instance.levelData.BPM * 1000);
                _eventInstance.setTimelinePosition(timelinePos);
            }

            Paused = false;
        }
    }
}