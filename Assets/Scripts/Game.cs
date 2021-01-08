using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour {

    public static string message;
    public Image img;

    private bool shouldStartGame;
    private float timer;
    public float fadeInDuration;
    
    public static void EndGame(bool victory, string msg) {
        message = msg;
        SceneManager.LoadScene(victory ? "Victory" : "Defeat");
    }

    public void StartGame() {
        shouldStartGame = true;
    }

    private void Update() {
        if (!shouldStartGame) return;

        if (timer > fadeInDuration) {
            SceneManager.LoadScene("Intro");
            shouldStartGame = false;
            timer = 0;
            return;
        }

        timer += Time.deltaTime;
        img.color = new Color(0, 0, 0, Mathf.Lerp(0, 1, timer / fadeInDuration));
    }

    public void CloseGame() {
        Application.Quit();
    }



}
