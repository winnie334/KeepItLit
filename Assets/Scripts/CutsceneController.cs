using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneController : MonoBehaviour {

    public Image background;
    public float nextFrameTime = 5;
    public float initialDelayTime = 8;
    public List<string> scriptLines;

    public AudioClip thunder;

    private TextRevealer textRevealer;
    private int currFrame = 0;
    private double timer = 0;
    private new AudioSource audio;

    
    // Start is called before the first frame update
    void Start() {
        audio = GetComponent<AudioSource>();
        audio.Play();
        textRevealer = GetComponent<TextRevealer>();
    }

    private void Update() {
        timer += Time.deltaTime;
    }

    private void switchAudioClip(AudioClip clip) {
        audio.clip = clip;
        audio.Play();
    }

    public void playThunder() {
        StartCoroutine(flashScreen());
        switchAudioClip(thunder);
    }
    
    IEnumerator flashScreen()
    {
        yield return new WaitForSeconds(1);
        background.color = Color.white;
        yield return new WaitForSeconds(0.05f);
        background.color = Color.black;
    }

    private void OnGUI() {
        if (Time.time < initialDelayTime) return;
        
        if (currFrame == 0) {
            textRevealer.play(scriptLines[currFrame++]);
            timer = 0;
        }
        else {
            if (currFrame >= scriptLines.Count) {
                if (timer >= 3 && currFrame++ == scriptLines.Count) playThunder();
                return;
            }
            if (!(timer >= nextFrameTime)) return;
            timer = 0;
            textRevealer.play(scriptLines[currFrame++]);
        }
    }
}
