using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class BlackFadeOut : MonoBehaviour {

    public bool disableOnFadeDone;
    private Image img;
    private float timer;
    public float fadeOutDuration;

    // Start is called before the first frame update
    void Start() {
        img = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update() {
        if (timer > fadeOutDuration) {
            if (disableOnFadeDone) {
                gameObject.SetActive(false);
                gameObject.GetComponent<BlackFadeOut>().enabled = false;
            } else return;
        };
        timer += Time.deltaTime;
        img.color = new Color(0, 0, 0, Mathf.Lerp(1, 0, timer / fadeOutDuration));
    }
}
