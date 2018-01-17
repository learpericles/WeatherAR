using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectActivityByState : MonoBehaviour {

    public StateManager.States state;
    public bool isDisplayedByDefault;

    private void Awake() {
        StateManager.onStateUpdated += StateUpdated;
    }

    private void Start() {
        StateUpdated(StateManager.currentState);
    }

    private void StateUpdated(StateManager.States newState) {
        gameObject.SetActive(isDisplayedByDefault == ((int)state > (int)newState));
    }
}
