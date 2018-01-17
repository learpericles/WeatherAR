using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mist : MonoBehaviour {

    private void Awake() {
        WeatherManager.onWeatherLoaded += SetMistActivity;
        WeatherManager.onWeatherUpdated += SetMistActivity;
    }

    void Start() {
        gameObject.SetActive(false);
    }

    private void SetMistActivity(bool isLoaded) {
        SetMistActivity();
    }

    private void SetMistActivity() {
        bool isMist = WeatherManager.currentWeather.weather == WeatherManager.WeatherStates.mist;
        gameObject.SetActive(isMist);
    }
}
