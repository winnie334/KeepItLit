using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {

    static public string message;

    static public void EndGame(bool victory, string msg) {
        message = msg;
        if (victory) {
            SceneManager.LoadScene("Victory");
        } else {
            SceneManager.LoadScene("Defeat");
        }
    }



}
