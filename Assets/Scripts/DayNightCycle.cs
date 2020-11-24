using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour {
    
    public Transform SunTransform;
    public Light Sun;

    public float maxIntensity;

    public float dayTime;
    public float nightTime;
    private float time;
    private Vector3 originalSunRotation;

    private void Start() {
        originalSunRotation = SunTransform.eulerAngles;
    }

    void Update() {
        time += Time.deltaTime;
        time %= dayTime + nightTime;

        float intensity;
        float rotationX;
        if (time < dayTime) { // Intensity ranges from 0 to maxIntensity based on time distance to noon
            intensity = HelperFunctions.p5Map(Math.Abs(time - dayTime / 2), 0, dayTime / 2, maxIntensity, 0);
            rotationX = time / dayTime * 180;
        } else {
            intensity = 0;
            rotationX = (time - dayTime) / nightTime * 180 + 180;
        }
        
        SunTransform.rotation = Quaternion.Euler(rotationX, originalSunRotation.y, originalSunRotation.z);
        Sun.intensity = intensity;
    }

}
