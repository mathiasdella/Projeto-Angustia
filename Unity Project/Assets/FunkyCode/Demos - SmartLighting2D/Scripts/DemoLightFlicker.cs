using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoLightFlicker : MonoBehaviour {
    public float flickersPerSecond = 10f;

    LightingSource2D lightSource;
    float lightAlpha;
    TimerHelper timer;

    void Start() {
        lightSource = GetComponent<LightingSource2D>();
        lightAlpha = lightSource.lightAlpha;
        timer = TimerHelper.Create();
    }

    void Update() {
        if (timer.GetMillisecs() > 1000f / flickersPerSecond) {
            float tempAlpha = lightAlpha;
            tempAlpha = tempAlpha + Random.Range(-0.2f, 0f);
            lightSource.lightAlpha = tempAlpha;
            timer.Reset();
        }
    }
}
