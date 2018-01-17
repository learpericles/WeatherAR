using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class StateManager: MonoBehaviour {

    public enum States { PlaneDetection, PlaneFound, SceneCreation }

    public static Action<States> onStateUpdated;
    public static States currentState {
        get {
            return _currentState;
        }

        set {
            if (_currentState != value) {
                _currentState = value;

                if (onStateUpdated != null)
                    onStateUpdated(value);
            }
        }
    }
    private static States _currentState;

    private void Awake() {
        HitTestManager.onGotPosition += delegate {
            currentState = States.SceneCreation;
        };

        UnityARSessionNativeInterface.ARAnchorAddedEvent += ARAnchorAddedEvent;
    }

    private void ARAnchorAddedEvent(ARPlaneAnchor anchorData) {
        currentState = States.PlaneFound;
    }
}
