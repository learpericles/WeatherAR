using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchController : MonoBehaviour {

    public static System.Action onTouchBegan;
    public static System.Action onTouchMoved;
    public static System.Action onTouchStopped;

    public static float normalizedPosX {
        get { return lastPosX / Screen.width; }
    }

    public static float normalizedPosY {
        get { return lastPosY / Screen.width; }
    }

    public static float deltaX { get; private set; }
    public static float deltaY { get; private set; }

    private static float startX;
    private static float startY;

    private static float lastPosX;
    private static float lastPosY;

    private static bool isDrag;

    void LateUpdate() {
#if UNITY_EDITOR
        MouseMovement();
#else
        TouchMovement();
#endif
    }

    private void TouchMovement() {
        if (Input.touchCount != 1)
            return;

        Touch touch = Input.GetTouch(0);

        switch (touch.phase) {
            case TouchPhase.Began:
                lastPosX = touch.position.x;
                lastPosY = touch.position.y;

                startX = lastPosX;
                startY = lastPosY;

                if (onTouchBegan != null)
                    onTouchBegan();
                break;

            case TouchPhase.Moved:
                lastPosX = touch.position.x;
                lastPosY = touch.position.y;

                deltaX = lastPosX - startX;
                deltaY = lastPosY - startY;

                startX = lastPosX;
                startY = lastPosY;

                if (onTouchMoved != null)
                    onTouchMoved();
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                deltaX = 0;
                deltaY = 0;

                if (onTouchStopped != null)
                    onTouchStopped();
                break;
        }
    }

    private void MouseMovement() {
        if (Input.GetMouseButtonDown(0) && !isDrag) {
            lastPosX = Input.mousePosition.x;
            lastPosY = Input.mousePosition.y;

            startX = lastPosX;
            startY = lastPosY;
            isDrag = true;

            if (onTouchBegan != null)
                onTouchBegan();
        }

        if (Input.GetMouseButton(0) && isDrag) {
            lastPosX = Input.mousePosition.x;
            lastPosY = Input.mousePosition.y;

            deltaX = lastPosX - startX;
            deltaY = lastPosY - startY;

            startX = lastPosX;
            startY = lastPosY;

            if (onTouchMoved != null)
                onTouchMoved();
        }

        if (Input.GetMouseButtonUp(0)) {
            deltaX = 0;
            deltaY = 0;
            isDrag = false;

            if (onTouchStopped != null)
                onTouchStopped();
        }
    }
}
