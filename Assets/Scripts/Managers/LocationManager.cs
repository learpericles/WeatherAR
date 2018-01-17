using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using LitJson;

public class LocationManager : MonoBehaviour {

    //Moscow geolocation
    private const float STANDART_LATITUDE = 55.7496f;
    private const float STANDART_LONGITUDE = 37.6237f;
    private const int WAIT_TIME_SEC = 5;

    public static Action<bool> onGotLocation;
    public static Action onGotCityName;

    public static string cityName = "Москва, Россия";
    public static float latitude;
    public static float longitude;
    public static float compassHeading;

    private void Awake() {
        latitude = STANDART_LATITUDE;
        longitude = STANDART_LONGITUDE;
    }

    private void Start() {
        StartCoroutine(GetLocation(() => {
            StartCoroutine(GetCityName(latitude, longitude));
        }));

    }

    IEnumerator GetLocation(Action callback = null) {
        if (!Input.location.isEnabledByUser) {
            if (onGotLocation != null)
                onGotLocation(false);

            yield break;
        }

        Input.location.Start();
        Input.compass.enabled = true;
        compassHeading = Input.compass.trueHeading;

        int waitTime = 10;

        while (Input.location.status == LocationServiceStatus.Initializing && waitTime > 0) {
            yield return new WaitForSeconds(1f);
            compassHeading = Input.compass.trueHeading;
            waitTime--;
        }

        if (waitTime < 1) {
            if (onGotLocation != null)
                onGotLocation(false);
        } else if (Input.location.status == LocationServiceStatus.Failed) {
            if (onGotLocation != null)
                onGotLocation(false);
        } else {
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;

            if (onGotLocation != null)
                onGotLocation(true);
        }

        Input.location.Stop();
        Input.compass.enabled = false;

        if (callback != null)
            callback();
    }

    IEnumerator GetCityName(float lat, float lng) {
        if (Application.internetReachability == NetworkReachability.NotReachable)
            yield break;

        string url = "http://maps.googleapis.com/maps/api/geocode/json?latlng={0},{1}&sensor=true";

        WWW www = new WWW(string.Format(url, lat, lng));
        yield return www;

        JsonData data = JsonMapper.ToObject(www.text);
        JsonData results = data["results"];

        foreach (JsonData result in results) {
            foreach (JsonData type in result["types"]) {
                if (type.ToString() == "locality") {
                    cityName = result["formatted_address"].ToString();

                    if (onGotCityName != null)
                        onGotCityName();

                    yield break;
                }
            }
        }

        if (onGotCityName != null)
            onGotCityName();
    }
}