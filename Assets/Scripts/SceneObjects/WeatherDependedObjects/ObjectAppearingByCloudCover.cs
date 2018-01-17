using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAppearingByCloudCover : WeatherableObject {

    [Range(0, 1)]
    public float requiredCloudCover;

    protected override void WeatherLoaded(bool isLoad) {
        gameObject.SetActive(WeatherManager.currentWeather.cloudCover >= requiredCloudCover);
    }
}
