using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardCameraController : MonoBehaviour {
    public float panSpeed;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

        if (Input.GetKey(KeyCode.LeftArrow)) {
            transform.parent.position -= transform.parent.right * panSpeed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.RightArrow)) {
            transform.parent.position += transform.parent.right * panSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.UpArrow)) {
            transform.parent.position += transform.parent.forward * panSpeed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.DownArrow)) {
            transform.parent.position -= transform.parent.forward * panSpeed * Time.deltaTime;
        }

    }

}
