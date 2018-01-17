using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ObjectSeeToCamera : MonoBehaviour {

    private Transform cam;
    private Animator anim;
    private bool isShow;

    private void Awake() {
        cam = GameObject.Find("Main Camera").transform;
        anim = transform.Find("CanvasParent").GetComponent<Animator>();
    }

    private void FixedUpdate() {
        transform.LookAt(cam);
        Vector3 angles = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0, angles.y + 90, angles.z);
    }

    public void Show() {
        if (isShow)
            return;

        print(gameObject.name + " show");
        anim.SetTrigger(Animator.StringToHash("Show"));
        isShow = true;
    }

    public void Hide() {
        if (!isShow)
            return;

        print(gameObject.name + " hide");
        anim.SetTrigger(Animator.StringToHash("Hide"));
        isShow = false;
    }
}
