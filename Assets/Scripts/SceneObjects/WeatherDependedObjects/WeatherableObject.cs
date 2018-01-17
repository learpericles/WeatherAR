using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeatherableObject : MonoBehaviour {

	protected virtual void Awake() {
        WeatherManager.onWeatherLoaded += WeatherLoaded;
        gameObject.SetActive(false);
    }

    protected abstract void WeatherLoaded(bool isLoad);
}
