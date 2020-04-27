using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodHub.Core {

    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

        private static T _instance;
        public static T Instance {
            get {
                if (_instance == null) {
                    _instance = FindObjectOfType<T>();

                    if (_instance == null) {
                        Debug.LogError("ERROR: No singleton instance of " + typeof(T) + " found, there should always be one instance");

                        return null;
                    } else if (FindObjectsOfType<T>().Length > 1) {
                        Debug.LogError("ERROR: More than one singleton instance of " + typeof(T) + " found, this is not how a SINGLEton works");

                        return _instance;
                    } else {
                        return _instance;
                    }
                } else {
                    return _instance;
                }
            }
        }

        [Header("Singleton Settings")]
        [SerializeField] private bool persistent;

        private void Awake() {
            if (_instance == null) {
                _instance = Instance;

                //Debug.Log("Singleton " + typeof(T).ToString() + " Instance Set");

                if (persistent) {
                    _instance.transform.parent = null;
                    DontDestroyOnLoad(_instance.gameObject);
                }
            } else {
                //If the instance is already set then this version of the singleton should be destroyed to maintain the pattern
                Destroy(gameObject);
            }
        }
    }
}