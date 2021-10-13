using UnityEngine;

namespace Utils {
    public class NamedComponent : MonoBehaviour {
        public SerializableDictionary<string, MonoBehaviour> components = new SerializableDictionary<string, MonoBehaviour>();
    }
}