using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndUi : MonoBehaviour {
    public Text text;

    private void Start() {
        text.text = Game.message;
        Cursor.lockState = CursorLockMode.None;
    }
}
