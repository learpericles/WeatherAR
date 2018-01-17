using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARCameraManager : UnityARCameraManager {

    private void Awake() {
        StateManager.onStateUpdated += StateUpdated;
    }

    private void StateUpdated(StateManager.States newState) {
        if (newState == StateManager.States.SceneCreation)
            m_session.Pause();
    }
}
