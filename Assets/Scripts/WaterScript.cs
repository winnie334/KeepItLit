using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterScript : MonoBehaviour {
    // Scroll main texture based on time

    public Vector2 scrollSpeed;
    Renderer rend;

    void Start() {
        rend = GetComponent<Renderer> ();
    }

    void Update() {
        rend.material.SetTextureOffset("_MainTex", new Vector2(Time.time * scrollSpeed.x, Time.time * scrollSpeed.y));
    }
}