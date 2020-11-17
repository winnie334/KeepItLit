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
    private GameObject prevBoat;

    // Start is called before the first frame update
    void Start()
    {
        crafter = GetComponent<Crafter>();
        setCraftingRecipe();
        GameObject.Find("Canvas").GetComponent<CraftUI>().setShipyard(gameObject);
    }

    bool isBoatFinished()
    {
        return currRecipeIndex == boatComponents.Count;
    }

    void setCraftingRecipe()
    {
        crafter.knownRecipes = new List<Recipe>() {boatComponents[currRecipeIndex++]};
    }

    //TODO use an array with all the recipes and a index, as soon as we reach the last state the game should be over
    public void handleCreateRecipe(Recipe recipe)
    {
        if (prevBoat) Destroy(prevBoat.gameObject);
        prevBoat = Instantiate(recipe.resultingItem, transform);
        if (isBoatFinished()) Debug.Log("game should end");
        else setCraftingRecipe();
    }
}