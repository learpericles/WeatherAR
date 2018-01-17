using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using LitJson;

public class WeatherManager : MonoBehaviour {
    public enum Seasons { winter, spring, summer, autumn };
    public enum WeatherStates { sunny, cloudy, mist, rainy };
    public enum Precip { none, rain, snow, sleet };
    public enum Data { Temperature, State, WindSpeed, WindDirection, Humidity, Time }
    public enum WindDirection {
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    }

    public class Weather {
        public const float MAX_WIND_SPEED = 25;

        public DateTime time;
        public int temperature = 15;
        public Precip precip = Precip.none;
        public float windDeg = 45;
        public Seasons season = Seasons.summer;
        public WeatherStates weather = WeatherStates.sunny;
        public float cloudCover = 0.16f;
        public float windSpeed;
        public float humidity;
        public float precipIntensity;
        public DateTime sunriseTime = defaultSunriseTime;
        public DateTime sunsetTime = defaultSunsetTime;

        /// <summary>
        /// Нормализованная скорость ветра относительно MAX_WIND_SPEED (25м/с)
        /// </summary>
        public float windSpeedNormalized {
            get {
                return windSpeed / MAX_WIND_SPEED;
            }
        }

        public WindDirection windDirection {
            get { return GetWindDirection(windDeg); }
        }

        public static DateTime defaultSunriseTime {
            get {
                int hourOfSunrise = 7;
                DateTime n = DateTime.Now;

                return new DateTime(n.Year, n.Month, n.Day, hourOfSunrise, 0, 0);
            }
        }

        public static DateTime defaultSunsetTime {
            get {
                int hourOfSunset = 20;
                DateTime n = DateTime.Now;
                return new DateTime(n.Year, n.Month, n.Day, hourOfSunset, 0, 0);
            }
        }

        public override string ToString() {
            string t = time + ": \n";
            t += temperature + "\n";
            t += weather + "\n";
            t += precip + "\n";
            t += cloudCover + "\n";
            t += windDeg + "\n";
            t += windSpeed + "\n";
            t += sunriseTime + "\n";
            t += sunsetTime + "\n";

            return t;
        }
    }

    public static Action<bool> onWeatherLoaded;
    public static Action onWeatherUpdated;

    public static Weather currentWeather;
    public static List<Weather> weathers = new List<Weather>();

    public static bool isCurrentWeather {
        get {
            return currentWeather == weathers[0];
        }
    }

    private void Awake() {
        LocationManager.onGotLocation += LoadWeather;
        WeatherUI.onPartUpdated += UpdateWeather;
    }

    public static void ToCurrentWeather() {
        UpdateWeather(0);
    }

    private static void UpdateWeather(int part) {
        currentWeather = weathers[part];

        if (onWeatherUpdated != null)
            onWeatherUpdated();
    }

    private void LoadWeather(bool isLoad) {
        StartCoroutine(LoadWeather());
    }

    IEnumerator LoadWeather() {
        if (Application.internetReachability == NetworkReachability.NotReachable) {

            if (onWeatherLoaded != null)
                onWeatherLoaded(false);

            yield break;
        }

        string url = "https://api.darksky.net/forecast/" +
                     Config.WEATHER_API_KEY + "/" +
                     LocationManager.latitude + "," +
                     LocationManager.longitude +
                     "?lang=ru";

        WWW www = new WWW(url);
        yield return www;

        JsonData data = JsonMapper.ToObject(www.text);
        bool isFailed = false;

        try {
            JsonData d = data["currently"];
        }
        catch {
            isFailed = true;
        }

        if (isFailed) {
            if (onWeatherLoaded != null)
                onWeatherLoaded(false);

            yield break;
        }

        FillWeatherData(data);

        if (onWeatherLoaded != null)
            onWeatherLoaded(true);
    }

    private void FillWeatherData(JsonData data) {
        CreateWeather(data["currently"]);
        AddWeather(data["hourly"]["data"]);
        AddWeather(data["daily"]["data"]);

        currentWeather = weathers[0];
    }

    private void AddWeather(JsonData d) {
        for (int i = 0; i < d.Count; i++) {
            JsonData json = d[i];
            CreateWeather(json);
        }
    }

    private void CreateWeather(JsonData json) {
        Weather w = new Weather() {
            time = GetTime(json),
            temperature = (int)FahrenheitToCelsius(GetTemperature(json)),
            weather = GetWeatherType(json),
            precip = GetWeatherPrecip(json),
            cloudCover = GetCloudCover(json),
            windDeg = GetWindDeg(json),
            windSpeed = GetWindSpeed(json),
            precipIntensity = GetPrecipIntensity(json),

            sunriseTime = GetSunriseTime(json),
            sunsetTime = GetSunsetTime(json)
        };

        weathers.Add(w);
    }

    #region JsonHandling
    bool isExist(JsonData json, string dataType) {
        try {
            JsonData d = json[dataType];
        }
        catch {
            return false;
        }

        return true;
    }

    DateTime ToDateTime(long unixTime) {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        return dateTime.AddSeconds(unixTime).ToLocalTime();
    }

    float GetWindSpeed(JsonData json) {
        float speed = GetData(json, "windSpeed");
        return MilesToKm(speed);
    }

    DateTime GetTime(JsonData json) {
        long unixTime;

        if (long.TryParse(json["time"].ToString(), out unixTime))
            return ToDateTime(unixTime);

        return DateTime.Now;
    }

    DateTime GetSunriseTime(JsonData json) {
        if (!isExist(json, "sunriseTime"))
            return Weather.defaultSunriseTime;

        long unixTime;

        if (long.TryParse(json["sunriseTime"].ToString(), out unixTime))
            return ToDateTime(unixTime);

        return Weather.defaultSunriseTime;
    }

    DateTime GetSunsetTime(JsonData json) {
        if (!isExist(json, "sunsetTime"))
            return Weather.defaultSunsetTime;

        long unixTime;

        if (long.TryParse(json["sunsetTime"].ToString(), out unixTime))
            return ToDateTime(unixTime);

        return Weather.defaultSunsetTime;
    }

    float GetTemperature(JsonData json) {
        if (isExist(json, "temperature")) {
            return GetData(json, "temperature");
        } else if (isExist(json, "temperatureMin")) {
            float min = GetData(json, "temperatureMin");
            float max = GetData(json, "temperatureMax");

            return (max + min) / 2;
        }

        throw new Exception("Данные не найдены");
    }

    float GetHumidity(JsonData json) {
        return GetData(json, "humidity");
    }

    float GetWindDeg(JsonData json) {
        return GetData(json, "windBearing");
    }

    WeatherStates GetWeatherType(JsonData json) {
        float cloudCover = GetCloudCover(json);
        float visibility = GetVisibility(json);

        if (cloudCover >= 0.8f) {
            if (visibility < 1) {
                return WeatherStates.mist;
            } else {
                return WeatherStates.cloudy;
            }
        } else {
            return WeatherStates.sunny;
        }
    }

    float GetCloudCover(JsonData json) {
        return GetData(json, "cloudCover");
    }

    float GetVisibility(JsonData json) {
        try {
            return GetData(json, "visibility");
        }
        catch {
            // TODO: определять видимость по косвенным признакам (влажность, температура)
            return 3;
        }
    }

    float GetPrecipIntensity(JsonData json) {
        return GetData(json, "precipIntensity");
    }

    Precip GetWeatherPrecip(JsonData json) {
        float precipPropability = GetData(json, "precipProbability");

        if (precipPropability < 0.8f)
            return Precip.none;

        string precip = "";

        try {
            precip = json["currently"]["precipType"].ToString();
        }
        catch {
            // TODO: precipType не всегда возвращается, определять по косвенным признакам
            return Precip.none;
        }

        switch (precip) {
            case "rain":
                return Precip.rain;
            case "snow":
                return Precip.snow;
            case "sleet":
                return Precip.sleet;
        }

        return Precip.none;
    }

    float GetData(JsonData json, string dataType) {
        float data = 0;
        if (float.TryParse(json[dataType].ToString(), out data)) {
            return data;
        }

        return data;
    }
    #endregion

    #region DataTransforming
    public static WindDirection GetWindDirection(float deg) {
        if (deg >= 337.5f || deg <= 22.5f)
            return WindDirection.North;

        if (deg >= 22.5f && deg <= 67.5f)
            return WindDirection.NorthEast;

        if (deg >= 67.5f && deg <= 112.5f)
            return WindDirection.East;

        if (deg >= 112.5f && deg <= 157.5f)
            return WindDirection.SouthEast;

        if (deg >= 157.5f && deg <= 202.5f)
            return WindDirection.South;

        if (deg >= 202.5f && deg <= 247.5f)
            return WindDirection.SouthWest;

        if (deg >= 247.5f && deg <= 292.5f)
            return WindDirection.West;

        if (deg >= 292.5f && deg <= 337.5f)
            return WindDirection.NorthWest;

        return WindDirection.South;
    }

    float FahrenheitToCelsius(float fahrenheit) {
        return (fahrenheit - 32) / 1.8f;
    }

    static float MilesToKm(float mile) {
        float speed = mile * 1.60934f;
        return speed = (float)Math.Round(speed, 1);
    }
    #endregion

    public static string GetData(Data data) {
        switch (data) {
            case Data.Temperature:
                return TemperatureText(currentWeather);
            case Data.State:
                return StateText(currentWeather);
            case Data.WindSpeed:
                return WindSpeedText(currentWeather);
            case Data.WindDirection:
                return WindDirectionText(currentWeather);
            case Data.Humidity:
                return HumidityText(currentWeather);
            case Data.Time:
                return TimeText(currentWeather);
            default:
                return "Error";
        }
    }

    #region Texts
    public static string TimeText(Weather weather) {
        bool isToday = weather.time.DayOfYear == DateTime.Now.DayOfYear;
        bool isThisWeek = weather.time.DayOfWeek < DayOfWeek.Saturday;
        bool isNext48Hours = (weather.time - DateTime.Now).TotalHours < 48;
        string text = "";

        if (isToday) {
            text = weather.time.ToShortTimeString();
        } else if (isNext48Hours) {
            text = weather.time.DayOfWeek + " " + weather.time.ToShortTimeString();
        } else if (isThisWeek) {
            text = weather.time.DayOfWeek.ToString();
        } else {
            text = weather.time.ToShortDateString();
        }

        return text;
    }

    public static string HumidityText(Weather weather) {
        string text = "Влажность: {0}%";
        int humidity = (int)(weather.humidity * 100);

        return string.Format(text, humidity);
    }

    public static string TemperatureText(Weather weather) {
        int t = weather.temperature;
        string str = "";

        str += t < 0 ? "-" : "";
        str += t > 0 ? "+" : "";
        str += t.ToString() + "°";

        return str;
    }

    public static string StateText(Weather weather) {
        return weather.weather.ToString().ToUpper();
    }

    public static string WindSpeedText(Weather weather) {
        string format = "{0} км/ч";
        return String.Format(format, weather.windSpeed);
    }

    public static string WindDirectionText(Weather weather) {
        switch (weather.windDirection) {
            case WindDirection.North:
                return "Северный";
            case WindDirection.NorthEast:
                return "Северо-восточный";
            case WindDirection.East:
                return "Западный";
            case WindDirection.SouthEast:
                return "Юго-восточный";
            case WindDirection.South:
                return "Южный";
            case WindDirection.SouthWest:
                return "Юго-западный";
            case WindDirection.West:
                return "Западный";
            case WindDirection.NorthWest:
                return "Северо-западный";
            default:
                return "Штиль";
        }
    }
    #endregion
}