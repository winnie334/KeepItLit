using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {

    public static string message;

    public static void EndGame(bool victory, string msg) {
        message = msg;
        SceneManager.LoadScene(victory ? "Victory" : "Defeat");
    }

    public void StartGame() {
        SceneManager.LoadScene("CharacterMovement");
    }

    public void CloseGame() {
        Application.Quit();
    }



}
