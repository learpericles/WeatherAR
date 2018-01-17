using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToCurrentWeatherButton : MonoBehaviour {

    private Button button;

    private void Awake() {
        button = GetComponent<Button>();
        WeatherManager.onWeatherUpdated += SetActive;
    }

    private void Start() {
        button.onClick.AddListener(Click);
        gameObject.SetActive(false);
    }

    private void SetActive() {
        gameObject.SetActive(!WeatherManager.isCurrentWeather);
    }

    private void Click() {
        WeatherManager.ToCurrentWeather();
    }
}
