using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeUIText : WeatherDataTextUI {

    protected override void SetText() {
        if (WeatherManager.isCurrentWeather)
            text.text = "";
        else
            base.SetText();
    }
}
