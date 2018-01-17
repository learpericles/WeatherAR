using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Precips : MonoBehaviour {

    private GameObject rain;
    private GameObject snow;
    private GameObject hail;

    private void Awake() {
        rain = transform.Find("Rain").gameObject;
        snow = transform.Find("Snow").gameObject;
        hail = transform.Find("Hail").gameObject;

        WeatherManager.onWeatherLoaded += WeatherLoaded;
        WeatherManager.onWeatherUpdated += SetPrecip;
    }

    private void Start() {
        rain.SetActive(false);
        snow.SetActive(false);
        hail.SetActive(false);
    }

    private void WeatherLoaded(bool isLoaded) {
        SetPrecip();
    }

    private void SetPrecip() {
        rain.SetActive(false);
        snow.SetActive(false);
        hail.SetActive(false);

        WeatherManager.Precip precip = WeatherManager.currentWeather.precip;

        switch (precip) {
            case WeatherManager.Precip.rain:
                rain.SetActive(true);
                break;
            case WeatherManager.Precip.snow:
                snow.SetActive(true);
                break;
            case WeatherManager.Precip.sleet:
                hail.SetActive(true);
                break;
        }
    }
}
