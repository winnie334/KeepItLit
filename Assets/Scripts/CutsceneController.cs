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
    private int currParagraph;
    private int currFrame = 0;

    public AudioClip thunder;
    public AudioClip seaShore;
    public AudioClip fire;
    private AudioSource soundEffect;
    private AudioSource backgroundMusic;

    private TextRevealer textRevealer;
    private float timer = 0;
    
    private float[] audioChangeSettings = {0.3f, 1f, 4};
    private bool shouldChangeAudio;
    private Action endAudioFunction;
    private Action endFunction;
    private bool changeAudioBackground = true;


    // Start is called before the first frame update
    void Start() {
        var audioSources = GetComponents<AudioSource>();
        backgroundMusic = audioSources[0];
        backgroundMusic.volume = 0.3f;
        soundEffect = audioSources[1];
        endFunction = () => {
            shouldChangeAudio = true;
            endAudioFunction = () => { switchParagraph(() => { StartCoroutine(playBoatCrash()); }); };
        };
        textRevealer = GetComponent<TextRevealer>();
    }

    private void Update() {
        timer += Time.deltaTime;
        if (shouldChangeAudio) changeAudio();
    }

    private void switchAudioClip(AudioClip clip) {
        backgroundMusic.clip = clip;
        backgroundMusic.Play();
    }

    private void playLine(String line) {
        textRevealer.play(line);
        timer = 0;
    }

    private void playParagraph() {
        if (currFrame == 0) playLine(paragraphs[currParagraph].list[currFrame++]);
        else {
            if (currFrame >= paragraphs[currParagraph].list.Count) {
                if (currFrame++ == paragraphs[currParagraph].list.Count) endFunction();
                return;
            }

            if (!(timer >= nextFrameTime)) return;
            playLine(paragraphs[currParagraph].list[currFrame++]);
        }
    }



    private void changeAudio() {
        var music = changeAudioBackground ? backgroundMusic : soundEffect;
        if (timer < audioChangeSettings[2]) music.volume = Mathf.Lerp(audioChangeSettings[0],
            audioChangeSettings[1], timer / audioChangeSettings[2]);
        else {
            shouldChangeAudio = false;
            music.volume = audioChangeSettings[1];
            endAudioFunction();
        }
    }

    private void switchParagraph(Action newEndFunction) {
        currFrame = 0;
        endFunction = newEndFunction;
        currParagraph++;
    }

    private void OnGUI() {
        if (Time.time < initialDelayTime) return;
        playParagraph();
    }

    private void changeAudioPrep(float[] newAudioSettings, Action newEndAudioFunction) {
        timer = 0;
        audioChangeSettings = newAudioSettings;
        endAudioFunction = newEndAudioFunction;
        shouldChangeAudio = true;
    }

    IEnumerator playBoatCrash() {
        yield return new WaitForSeconds(1);
        textRevealer.clearText();
        soundEffect.Play();
        changeAudioPrep(new[] {1f, 0.1f, 5f}, 
            () => { StartCoroutine(playShoreSounds()); });
    }
    
    IEnumerator playShoreSounds() {
        yield return new WaitForSeconds(1);
        backgroundMusic.clip = seaShore;
        backgroundMusic.Play();
        changeAudioPrep(new[] {0f, 0.5f, 6f}, 
            () => { switchParagraph(() => StartCoroutine(playThunder())); });
    }

    IEnumerator playThunder() {
        yield return new WaitForSeconds(4);
        textRevealer.clearText();
        changeAudioBackground = false;
        soundEffect.clip = thunder;
        soundEffect.Play();
        changeAudioPrep(new[] {1f, 0f, 4f}, () => { StartCoroutine(playFire());});
        
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
        switchParagraph(() => { StartCoroutine(switchScene());});
    }

    IEnumerator switchScene() {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("CharacterMovement");
    }
}

//unity hack to make list of lists visible in inspector xd
[Serializable]
public class listWrapper
{
    public List<string> list;
}