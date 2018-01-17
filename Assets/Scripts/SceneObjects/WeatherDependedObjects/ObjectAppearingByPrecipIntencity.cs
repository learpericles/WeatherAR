using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAppearingByPrecipIntencity : WeatherableObject {

    [Range(0, 1)]
    public float requiredPrecipIntensity;

    protected override void WeatherLoaded(bool isLoad) {
        gameObject.SetActive(WeatherManager.currentWeather.precipIntensity >= requiredPrecipIntensity);
    }
}
