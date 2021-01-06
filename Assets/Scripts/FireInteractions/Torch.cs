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

    private float initialPsSize;
    private AudioSource fireSource;
    private AudioSource fireLitSfx;
    
    // Start is called before the first frame update
    void Start() {
        var audioSources = GetComponents<AudioSource>();
        fireSource = audioSources[0];
        fireLitSfx = audioSources[1];
        ps = GetComponentInChildren<ParticleSystem>();
        initialPsSize = ps.shape.radius;
        light = GetComponentInChildren<Light>();
        reset();
    }

    // Update is called once per frame
    void Update() {
        if (!isOn) return;
        var shapeModule = ps.shape;
        timeLeft -= Time.deltaTime;
        if (timeLeft / Duration <= 0.3f) { // Torch is in last 30%, start dying out
            var percentageAlive = HelperFunctions.p5Map(timeLeft / Duration, 0, 0.3f, 0, initialPsSize);
            shapeModule.radius = percentageAlive;
            light.range = percentageAlive * lightRadius;
        }

        if (timeLeft <= 0) reset();
    }

    void reset() {
        isOn = false;
        ps.Stop();
        light.enabled = false;
        fireSource.Stop();
        
        var shapeModule = ps.shape;
        shapeModule.radius = initialPsSize;
        light.range = lightRadius;
    }

    public void onFireInteraction() {
        fireSource.Play();
        fireLitSfx.Play();
        isOn = true;
        light.enabled = true;
        timeLeft = Duration;
        ps.Play();
    }
}
