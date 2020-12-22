using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour {

    public GameObject settingsUI;
    public Grid island;
    private bool paused;

    // Start is called before the first frame update
    void Start() {
        QualitySettings.SetQualityLevel(0, true);
        island.setGrassQuality(0);
        changeResolution(0);
        AudioListener.volume = 0.5f;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) togglePause();
    }

    // Options are 1280x720, 1920x1080, 2560x1440
    public void changeResolution(int option) {
        Screen.SetResolution(new[] {1280, 1920, 2560}[option], new[] {720, 1080, 1440}[option], true);
    }

    public void setQuality(int option) {
        QualitySettings.SetQualityLevel(option, true);
        island.setGrassQuality(option);
    }

    public void togglePause() {
        paused = !paused;
        Time.timeScale = paused ? 0 : 1;
        settingsUI.SetActive(paused);
    }
    
    public void changeMusicVolume(float vol) {
        // TODO once we have music
    }

    public void changeSFXVolume(float vol) {
        AudioListener.volume = vol;
    }
}
