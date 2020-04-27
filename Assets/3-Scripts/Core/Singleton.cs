using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MellowMadness.Core {

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
    }
}