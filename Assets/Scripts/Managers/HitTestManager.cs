using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class HitTestManager : MonoBehaviour {

    public static Action<Vector3, Quaternion> onGotPosition;

    bool HitTestWithResultType(ARPoint point, ARHitTestResultType resultTypes) {
        List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(point, resultTypes);
        if (hitResults.Count > 0) {
            foreach (var hitResult in hitResults) {
                Vector3 pos = UnityARMatrixOps.GetPosition(hitResult.worldTransform);
                Quaternion rot = UnityARMatrixOps.GetRotation(hitResult.worldTransform);

                if (onGotPosition != null)
                    onGotPosition(pos, rot);

                return true;
            }
        }

        return false;
    }

    private void Update() {
        if (Input.touchCount > 0) {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began) {
                var screenPosition = Camera.main.ScreenToViewportPoint(touch.position);
                ARPoint point = new ARPoint {
                    x = screenPosition.x,
                    y = screenPosition.y
                };

                // prioritize reults types
                ARHitTestResultType[] resultTypes = {
                        ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent, 
                        // if you want to use infinite planes use this:
                        //ARHitTestResultType.ARHitTestResultTypeExistingPlane,
                        ARHitTestResultType.ARHitTestResultTypeHorizontalPlane,
                        ARHitTestResultType.ARHitTestResultTypeFeaturePoint
                    };

                foreach (ARHitTestResultType resultType in resultTypes) {
                    if (HitTestWithResultType(point, resultType))
                        break;
                }
            }
        }
    }
}
