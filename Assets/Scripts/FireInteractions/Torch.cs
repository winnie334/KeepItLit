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

    public AudioClip fireActivatedSound;
    private float initialPsSize;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start() {
        audioSource = GetComponent<AudioSource>();
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
        audioSource.Stop();
        
        var shapeModule = ps.shape;
        shapeModule.radius = initialPsSize;
        light.range = lightRadius;
    }

    public void onFireInteraction() {
        if (timeLeft > Duration - 1) return; // Don't trigger this a thousand times
        audioSource.PlayOneShot(fireActivatedSound);
        isOn = true;
        light.enabled = true;
        timeLeft = Duration;
        ps.Play();
    }
}
