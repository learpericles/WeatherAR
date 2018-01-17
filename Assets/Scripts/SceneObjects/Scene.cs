using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class Scene : MonoBehaviour {

    private Transform cam;
    private bool isShow;

    private void Awake() {
        cam = GameObject.Find("Main Camera").transform;

        HitTestManager.onGotPosition += SetPosition;
    }

    private void SetPosition(Vector3 pos, Quaternion rot) {
        if (isShow)
            return;

        isShow = true;
        transform.position = pos;
        transform.rotation = rot;

        LookAtMe();
    }

    private void LookAtMe() {
        transform.LookAt(cam);
        Vector3 angles = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0, angles.y + 90, angles.z);
    }
}
