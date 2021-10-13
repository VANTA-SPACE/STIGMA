using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager {
    public abstract class Manager<T> : MonoBehaviour where T : Manager<T> {
        public static T Instance {
            get {
                if (instance == null) {
                    instance = FindObjectOfType<T>();
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
            Init();
        }

        public virtual void Init() { }
    }
}