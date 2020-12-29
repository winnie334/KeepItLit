using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutsceneController : MonoBehaviour {
    public Image background;
    public float nextFrameTime = 5;
    public float initialDelayTime = 8;

    public List<listWrapper> paragraphs;

    public AudioClip thunder;
    public AudioClip seaShore;
    public AudioClip fire;

    private AudioSource soundEffect;
    private AudioSource backgroundMusic;

    private TextRevealer textRevealer;
    private float timer = 0;

    private float[] volumeChangeSettings = {0.3f, 1f, 4}; // [starting volume, end volume, duration to reach end volume]
    private bool shouldChangeVolume;

    private Action
        volumeChangeEndFunction; //A function that is called as soon as the volume has been completely changed

    private Action paragraphEndFunction; //A function that is called after the end of a paragraph
    private bool shouldChangeBackgroundVolume = true; //determines which audioSource volume we should change

    private int currParagraph;
    private int currFrame;


    // Start is called before the first frame update
    void Start() {
        var audioSources = GetComponents<AudioSource>();
        backgroundMusic = audioSources[0];
        backgroundMusic.volume = 0.3f;
        soundEffect = audioSources[1];
        paragraphEndFunction = () => {
            shouldChangeVolume = true;
            volumeChangeEndFunction = () => { playNextParagraph(() => { StartCoroutine(playBoatCrash()); }); };
        };
        textRevealer = GetComponent<TextRevealer>();
    }

    private void OnGUI() {
        if (Time.time < initialDelayTime) return;
        playParagraph();
    }

    private void Update() {
        timer += Time.deltaTime;
        if (shouldChangeVolume) changeVolumeGradually();
    }

    private void playParagraph() {
        if (currFrame == 0) playLineOfParagraph(paragraphs[currParagraph].list[currFrame++]);
        else {
            if (currFrame >= paragraphs[currParagraph].list.Count) {
                if (currFrame++ == paragraphs[currParagraph].list.Count) paragraphEndFunction();
                return;
            }

            if (!(timer >= nextFrameTime)) return;
            playLineOfParagraph(paragraphs[currParagraph].list[currFrame++]);
        }
    }

    private void playNextParagraph(Action newEndFunction) {
        currFrame = 0;
        paragraphEndFunction = newEndFunction;
        currParagraph++;
    }

    private void playLineOfParagraph(string line) {
        textRevealer.play(line);
        timer = 0;
    }

    // Should not be called since this happens in the Update function. Use the changeVolumeGraduallyPreparation function instead
    private void changeVolumeGradually() {
        var music = shouldChangeBackgroundVolume ? backgroundMusic : soundEffect;
        if (timer < volumeChangeSettings[2])
            music.volume = Mathf.Lerp(volumeChangeSettings[0],
                volumeChangeSettings[1], timer / volumeChangeSettings[2]);
        else {
            shouldChangeVolume = false;
            music.volume = volumeChangeSettings[1];
            volumeChangeEndFunction();
        }
    }

    // Call this when you want to change the volume gradually
    private void changeVolumeGraduallyPreparation(float[] newAudioSettings, Action newEndAudioFunction) {
        timer = 0;
        volumeChangeSettings = newAudioSettings;
        volumeChangeEndFunction = newEndAudioFunction;
        shouldChangeVolume = true;
    }

    IEnumerator playBoatCrash() {
        yield return new WaitForSeconds(1);
        textRevealer.clearText();
        soundEffect.Play();
        changeVolumeGraduallyPreparation(new[] {1f, 0.1f, 5f},
            () => { StartCoroutine(playShoreSounds()); });
    }

    IEnumerator playShoreSounds() {
        yield return new WaitForSeconds(1);
        backgroundMusic.clip = seaShore;
        backgroundMusic.Play();
        changeVolumeGraduallyPreparation(new[] {0f, 0.35f, 6f},
            () => { playNextParagraph(() => StartCoroutine(playThunder())); });
    }

    IEnumerator playThunder() {
        yield return new WaitForSeconds(4);
        textRevealer.clearText();
        shouldChangeBackgroundVolume = false;
        soundEffect.clip = thunder;
        soundEffect.Play();
        changeVolumeGraduallyPreparation(new[] {1f, 0f, 4f}, () => { StartCoroutine(playFire()); });

        yield return new WaitForSeconds(1);
        background.color = Color.white;
        yield return new WaitForSeconds(0.05f);
        background.color = Color.black;
    }

    IEnumerator playFire() {
        soundEffect.clip = fire;
        soundEffect.volume = 0.8f;
        soundEffect.Play();
        yield return new WaitForSeconds(3);
        playNextParagraph(() => { StartCoroutine(switchScene()); });
    }

    IEnumerator switchScene() {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("CharacterMovement");
    }
}

//unity hack to make list of lists visible in inspector xd
[Serializable]
public class listWrapper {
    public List<string> list;
}