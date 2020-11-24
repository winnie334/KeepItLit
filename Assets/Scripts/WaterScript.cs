using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterScript : MonoBehaviour {
    // Scroll main texture based on time

    public Vector2 scrollSpeed;
    Renderer rend;

    public float frequency = 20.0f;  // Speed of sine movement
    public float magnitude = 0.5f;   // Size of sine movement

    private float initialHeight;
    private float yOffset = 0;

    void Start() {
        initialHeight = transform.position.y;
        rend = GetComponent<Renderer>();
    }

    void Update() {
        rend.material.SetTextureOffset("_MainTex", new Vector2(Time.time * scrollSpeed.x, Time.time * scrollSpeed.y));
        yOffset = Mathf.Sin(Time.time * frequency / 100) * magnitude;
        var position = transform.position;
        position = new Vector3(position.x, initialHeight + yOffset, position.z);
        transform.position = position;
    }
}