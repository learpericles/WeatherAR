using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateByWind : MonoBehaviour {

    private void Awake() {
        WeatherManager.onWeatherLoaded += WeatherLoaded;
        WeatherManager.onWeatherUpdated += UpdateRotation;
    }

    private void WeatherLoaded(bool isLoad) {
        UpdateRotation();
    }

    private void UpdateRotation() {
        float windHeading = -LocationManager.compassHeading + WeatherManager.currentWeather.windDeg;
        Vector3 rot = transform.rotation.eulerAngles;

        if (gameObject.activeInHierarchy)
            StartCoroutine(SmoothRotation(new Vector3(rot.x, windHeading, rot.z)));
        else
            transform.rotation = Quaternion.Euler(new Vector3(rot.x, windHeading, rot.z));
    }

    IEnumerator SmoothRotation(Vector3 to) {
        Vector3 from = transform.rotation.eulerAngles;
        float t = 0;

        while (t <1) {
            transform.rotation = Quaternion.Euler(Vector3.Lerp(from, to, t));
            yield return new WaitForFixedUpdate();
            t += Time.deltaTime * 5;
        }
    }
}
