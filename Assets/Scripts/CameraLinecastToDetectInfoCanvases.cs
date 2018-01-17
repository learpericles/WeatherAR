using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLinecastToDetectInfoCanvases : MonoBehaviour {

    private Camera cam;
    private ObjectSeeToCamera lastObject;

    // Use this for initialization
    void Awake() {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        Ray ray = cam.ScreenPointToRay(new Vector2(cam.pixelWidth / 2, cam.pixelHeight / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) {
            Transform objectHit = hit.transform;

            if (objectHit.GetComponent<ObjectSeeToCamera>()) {
                objectHit.GetComponent<ObjectSeeToCamera>().Show();
                lastObject = objectHit.GetComponent<ObjectSeeToCamera>();
            }
        } else if (lastObject != null) {
            lastObject.Hide();
            lastObject = null;
        }
    }
}
