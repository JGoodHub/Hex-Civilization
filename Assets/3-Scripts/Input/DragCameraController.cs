using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragCameraController : MonoBehaviour {

    public float timeToDrag;
    public float distanceToDrag;

    public float dragSpeed;

    private Vector2 pointerDownPosition;

    void Start() {

    }

    void Update() {
        if (Input.touchCount == 1) {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase) {
                case TouchPhase.Began:
                    Debug.Log("Touch Began");
                    break;
                case TouchPhase.Moved:
                    Debug.Log("Touch Moved");
                    break;
                case TouchPhase.Ended:
                    Debug.Log("Touch Ended");
                    break;
                default:
                    break;
            }


        }
    }

}
