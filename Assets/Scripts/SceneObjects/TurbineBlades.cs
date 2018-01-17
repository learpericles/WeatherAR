using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurbineBlades : MonoBehaviour {

    private const int MAX_SPEED = 300;
    private bool isLoad;
    private float rotateSpeed;

    private void Awake() {
        WeatherManager.onWeatherLoaded += WeatherLoaded;
        WeatherManager.onWeatherUpdated += SetSpeed;
    }

    private void WeatherLoaded(bool isLoad) {
        this.isLoad = true;
        SetSpeed();
    }

    private void SetSpeed() {
        rotateSpeed = WeatherManager.currentWeather.windSpeedNormalized;
    }

    private void FixedUpdate() {
        if (!isLoad) {
            return;
        }

        transform.Rotate(Vector3.forward * Time.deltaTime * MAX_SPEED * rotateSpeed);
    }
}
