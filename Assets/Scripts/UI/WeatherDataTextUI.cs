using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherDataTextUI : TextUI {

    public WeatherManager.Data data;

    [Tooltip("Объект, после которого проигрывается анимация появления текущего объекта")]
    public TextUI textUI;

    protected override void Awake() {
        base.Awake();
        WeatherManager.onWeatherLoaded += SetDataText;
        WeatherManager.onWeatherUpdated += SetText;
    }

    private void Start() {
        text.color = new Color(1, 1, 1, 0);
    }

    private void SetDataText(bool isLoad) {
        SetText();

        if (textUI == null) {
            Appearance();
        } else {
            textUI.onShown += Appearance;
        }
    }

    protected virtual void SetText() {
        text.text = WeatherManager.GetData(data);
    }

    private void Appearance() {
        FadeIn(() => {
            if (onShown != null)
                onShown();
        });
    }
}
