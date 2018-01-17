using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestText : MonoBehaviour {

    private Text text;

    private void Awake() {
        text = GetComponent<Text>();
        StateManager.onStateUpdated += UpdateText;
    }

    private void Start() {
        UpdateText(StateManager.currentState);
    }

    private void UpdateText(StateManager.States newState) {
        text.text = newState.ToString();
    }
}
