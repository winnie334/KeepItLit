using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {

    public float durationGame = 20;
    public GameObject gameInterface;
    public GameObject panel;
    public Text text;
    private float timeEndGame;
    private bool gameOver = false;

    void Start() {
        timeEndGame = Time.time + durationGame * 60;
        gameInterface.SetActive(false);
    }

    private void Update() {
        if (!gameOver && Time.time > timeEndGame) {
            EndGame(false, "You run out of time");
        }
    }

    public void EndGame(bool victory, string message) {
        gameOver = true;
        var pnl = panel.GetComponent<Image>();
        if (victory) {
            text.text = "You win: " + message;
            pnl.color = Color.green;
        } else {
            text.text = "You loose: " + message;
            pnl.color = Color.red;
        }
        gameInterface.SetActive(true);
    }
}
