using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextUI : MonoBehaviour {

    public Action onShown;
    protected Text text;
    private float alpha;

    protected virtual void Awake() {
        text = GetComponent<Text>();
    }

    protected void FadeIn(Action callback = null) {
        CrossFadeAlpha(1, callback, 0.5f);
    }

    protected void CrossFadeAlpha(float alpha, Action callback = null, float time = 1) {
        this.alpha = alpha;
        if (gameObject.activeSelf && gameObject.activeInHierarchy)
            StartCoroutine(SmoothAlpha(alpha, callback));
    }

    IEnumerator SmoothAlpha(float newAlpha, Action callback = null, float time = 1) {
        float t = 0;
        Color oldColor = text.color;
        Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, newAlpha);

        while (t < 1) {
            text.color = Color.Lerp(oldColor, newColor, t);
            yield return new WaitForFixedUpdate();
            t += Time.deltaTime / time;
        }

        text.color = newColor;

        if (callback != null)
            callback();
    }
}
