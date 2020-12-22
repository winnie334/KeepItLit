using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shipyard : MonoBehaviour
{
    public List<Recipe> boatComponents;
    private Crafter crafter;
    private int currRecipeIndex = 0;

    private CraftUI ui;
    private bool shouldSkip = true;

    // Start is called before the first frame update
    void Start() {
        crafter = GetComponent<Crafter>();
        crafter.knownRecipes = boatComponents;
        ui = GameObject.Find("Canvas").GetComponent<CraftUI>();
        ui.setCrafter(crafter);
    }

    bool isBoatFinished() {
        return currRecipeIndex == boatComponents.Count;
    }

    void removeRecipe() {
        boatComponents.RemoveAt(0);
        crafter.knownRecipes = boatComponents;
    }

    public void handleBoatPartCreated() {
        if (isBoatFinished()) Game.EndGame(true, "");
        else removeRecipe();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            ui.setCrafter(crafter);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            ui.setCrafter(null);
        }
    }
}