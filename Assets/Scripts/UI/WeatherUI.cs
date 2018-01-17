using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherUI : MonoBehaviour {

    public static Action<int> onPartUpdated;

    private const int PER_X = 75;

    private bool isLoad;
    private int x;
    private int part;
    private int lastPart;

    private void Start() {
        TouchController.onTouchMoved += Moved;
        WeatherManager.onWeatherLoaded += WeatherLoaded;
    }

    private void WeatherLoaded(bool isLoaded) {
        isLoad = true;
    }

    private void Moved() {
        if (!isLoad)
            return;

        x += (int)TouchController.deltaX * -1;
        part = x / 25;

        if (part != lastPart && part >= 0 && part < WeatherManager.weathers.Count) {
            if (onPartUpdated != null)
                onPartUpdated(part);

            lastPart = part;
        }
    }
}
