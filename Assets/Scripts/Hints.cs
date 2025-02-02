﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Hints : MonoBehaviour {

    public GameObject hintCard;
    public AudioClip hintSound;
    public float timeToShow; // How long each hint should be shown on the screen
    private float timeShown;
    
    private static List<String> hintsDisplayed = new List<string>();
    private GameObject currentCard;
    private Queue<String> messagesToDisplay = new Queue<string>();
    
    void Start() {
        
    }
    
    void Update() {
        if (!currentCard && messagesToDisplay.Count > 0) createCard(messagesToDisplay.Dequeue());
        if (!currentCard) return; // There is nothing being currently shown
        if (timeShown < timeToShow) timeShown += Time.deltaTime;
        else Destroy(currentCard);
    }

    // Adds a hint to the queue
    private void addHint(String message) {
        messagesToDisplay.Enqueue(message);
    }

    // Creates and displays a card with a given message to the user
    private void createCard(String message) { // TODO add hint sound?
        currentCard = Instantiate(hintCard, hintCard.transform.position, Quaternion.identity);
        currentCard.transform.SetParent(transform, false);
        currentCard.GetComponentInChildren<Text>().text = message;
        currentCard.GetComponent<AudioSource>().PlayOneShot(hintSound);
        timeShown = 0;
    }

    // A public static hint function so it can be called from any script, without needing each script to keep a
    // reference to this object
    public static void displayHint(String message) {
        if (hintsDisplayed.Any(message.Contains)) return; // We already displayed this hint
        hintsDisplayed.Add(message);
        // Now we know it is a new hint, we do the slightly performance heavy operation of finding the hint script
        GameObject.Find("Hints").GetComponent<Hints>().addHint(message);
    }

    // Put all hints related to picking up an item for the first time here
    public static void displayHintOnGrab(String item) {
        string hint = null;
        if (item == "Shipyard Construction Kit") hint = "I can place this in the sea";
        if (item == "RawChicken" || item == "Fish") hint = "This looks edible... but I should cook it first";
        if (item == "Ore") hint = "This rock has some ore, maybe I can smelt it to metal";
        if (item == "Torch") hint = "This will let me see at night, but it's still dangerous";
        if (hint != null) displayHint(hint);
    }
}
