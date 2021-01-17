using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour {
    
    public Transform SunTransform;
    public Light Sun;
    public Light Moon; // Will always be present, but simply intensity changed
    
    public float maxIntensity; // Max light during day
    public float minIntensity; // Max light during night

    public float startTime;
    public float dayTime;
    public float nightTime;
    private float time;
    private Vector3 originalSunRotation;

    private void Start() {
        originalSunRotation = SunTransform.eulerAngles;
        time = startTime;
    }

    void Update() {
        time += Time.deltaTime;
        time %= dayTime + nightTime;

        float sunIntensity;
        float moonIntensity;
        float rotationX;
        if (time < dayTime) { // Intensity ranges from 0 to maxIntensity based on time distance to noon
            sunIntensity = HelperFunctions.p5Map(Math.Abs(time - dayTime / 2), 0, dayTime / 2, maxIntensity + 0.6f, 0);
            moonIntensity = 0;
            rotationX = time / dayTime * 180;
        } else {
            sunIntensity = 0;
            moonIntensity = HelperFunctions.p5Map(Math.Abs(time - dayTime - nightTime / 2), nightTime / 2, 0, 0, minIntensity + 0.7f);
            rotationX = (time - dayTime) / nightTime * 180 + 180;
        }
        
        SunTransform.rotation = Quaternion.Euler(rotationX, originalSunRotation.y, originalSunRotation.z);
        Sun.intensity = Math.Min(maxIntensity, sunIntensity);
        Moon.intensity = Math.Min(minIntensity, moonIntensity);
    }

    public bool isDay() {
        return time < dayTime;
    }

}
