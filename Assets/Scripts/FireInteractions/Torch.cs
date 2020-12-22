using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour, IFireInteraction {

    public int Duration;
    public float lightRadius;

    private float timeLeft;
    private ParticleSystem ps;
    private new Light light;
    private bool isOn;
    
    // Start is called before the first frame update
    void Start() {
        ps = GetComponentInChildren<ParticleSystem>();
        light = GetComponentInChildren<Light>();
        ps.Stop();
        light.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isOn) return;
        var shapeModule = ps.shape;
        timeLeft -= Time.deltaTime;
        if (shapeModule.radius > 0.5f) {
            shapeModule.radius = timeLeft / Duration;
            light.range = timeLeft / Duration * lightRadius;
        }

        if (timeLeft <= 0) reset();
        }

    void reset() {
        isOn = false;
        ps.Stop();
        light.enabled = false;
        
        var shapeModule = ps.shape;
        shapeModule.radius = 1;
        light.range = lightRadius;
    }

    public void onFireInteraction() {
        isOn = true;
        light.enabled = true;
        timeLeft = Duration;
        ps.Play();
    }
}
