using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour {

    public static string message;
    private static Image fadePanel;

    private static bool shouldFade;
    private static float timer;
    public float fadeDuration;

    private static string scene;

    private void Start() {
        fadePanel = GameObject.Find("Fade").GetComponent<Image>();
    }

    public static void EndGame(bool victory, string msg) {
        if (shouldFade) return;
        message = msg;
        switchScene(victory ? "Victory" : "Defeat");
    }

    public void StartGame() {
        switchScene("Intro");
    }

    public void RestartGame() {
        switchScene("CharacterMovement");
    }

    private static void switchScene(string sceneToPlay) {
        scene = sceneToPlay;
        timer = 0;
        shouldFade = true;
        fadePanel.gameObject.SetActive(true);
        Debug.Log(fadePanel);
    }

    private void Update() {
        if (!shouldFade) return;

        if (timer > fadeDuration) {
            SceneManager.LoadScene(scene);
            shouldFade = false;
        }
        else {
            timer += Time.deltaTime;
            fadePanel.color = new Color(0, 0, 0, Mathf.Lerp(0, 1, timer / fadeDuration));
        }
    }

    public void CloseGame() {
        Application.Quit();
    }



}