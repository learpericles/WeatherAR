using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityName : TextUI {

    protected override void Awake() {
        base.Awake();
        LocationManager.onGotCityName += SetCityName;
    }

    private void SetCityName() {
        text.text = LocationManager.cityName;
    }
}
