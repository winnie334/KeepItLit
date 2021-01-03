using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundtrackManager : MonoBehaviour {

    public List<AudioClip> soundtrack;
    private AudioSource source;
    private int currSongIndex;
    // Start is called before the first frame update
    void Start() {
        source = GetComponent<AudioSource>();
        source.clip = soundtrack[0];
        source.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!source.isPlaying) changeSong();
    }

    void changeSong() {
        currSongIndex = currSongIndex == soundtrack.Count - 1 ? 0 : currSongIndex+1;
        source.clip = soundtrack[currSongIndex];
        source.Play();
    }
}
