using System;
using UnityEngine;

namespace Manager {
    public abstract class DontDestroyManager<T> : MonoBehaviour where T : DontDestroyManager<T> {
        public static T Instance {
            get {
                if (instance == null) {
                    instance = FindObjectOfType<T>();
                    DontDestroyOnLoad(instance.gameObject);
                }

                return instance;
            }

            private set => instance = value;
        }

        private static T instance;

        protected void Awake() {
            if (instance != null && instance != this) {
                Destroy(gameObject);
                return;
            }

            instance = (T) this;
            DontDestroyOnLoad(gameObject);
            Init();
        }

        public virtual void Init() { }
    }
}