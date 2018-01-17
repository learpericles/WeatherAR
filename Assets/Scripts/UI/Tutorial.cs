using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tutorial : MonoBehaviour, IPointerDownHandler {

    private const int TIME_TO_SHOW_TUTORIAL_SEC = 5;

    private bool wasShown {
        get { return PlayerPrefs.GetInt("TutorialWasShown") == 1; }
        set { PlayerPrefs.SetInt("TutorialWasShown", value ? 1 : 0); }
    }


    private void Start() {
        gameObject.SetActive(false);

        if (!wasShown)
            Invoke("Show", TIME_TO_SHOW_TUTORIAL_SEC);
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    public void OnPointerDown(PointerEventData eventData) {
        gameObject.SetActive(false);
        wasShown = true;
    }
}
