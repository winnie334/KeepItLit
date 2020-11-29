﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

// Class responsible for detecting which items are available to craft, filtering possible recipes,
// and doing the actual crafting of the new item
public class Crafter : MonoBehaviour {
    public List<Recipe> knownRecipes;
    private float craftRadius = 6;
    private Dictionary<Item, List<GameObject>> availableItems;
    private CraftUI craftUI;

    void Start() {
        craftUI = GameObject.Find("Canvas").GetComponent<CraftUI>();
    }

    // Returns all available items nearby for crafting, along with their corresponding GameObjects 
    // (So we can remove the objects from the world if needed)
    Dictionary<Item, List<GameObject>> getAvailableItems() {
        Collider[] nearbyItems = Physics.OverlapSphere(transform.position, craftRadius) // Look for objects in a radius
            .Where(hit => hit.GetComponent<ItemAssociation>() != null)
            .ToArray(); // If it doesn't have item script, it isn't an item

        // We categorize all nearby objects by item. Basically a lookup table for each item to the GameObjects
        // e.g. 'wood' -> [GameObject3, GameObject4], 'stone' -> [GameObject2], etc
        var availableItems = new Dictionary<Item, List<GameObject>>();
        foreach (var obj in nearbyItems) {
            var item = obj.GetComponent<ItemAssociation>().item;
            if (!availableItems.ContainsKey(item)) availableItems.Add(item, new List<GameObject> {obj.gameObject});
            else availableItems[item].Add(obj.gameObject);
        }

        return availableItems;
    }

    // Gives a list of recipes the player has the necessary materials for
    public List<Recipe> getPossibleRecipes() {
        if (availableItems is null) availableItems = getAvailableItems();
        return knownRecipes.Where(recipe => canMakeRecipe(recipe, availableItems)).ToList();
    }

    void destroyRequiredMaterials(List<Item> requiredMaterials) {
        var availableItems = getAvailableItems();
        // Assert.IsTrue(canMakeRecipe(recipe, availableItems)); //TODO put back on
        foreach (var requiredMaterial in requiredMaterials) {
            var bestObject = availableItems[requiredMaterial][0]; // If you want to do smart picking (closest first, direct raycast, ...) do it here
            availableItems[requiredMaterial].Remove(bestObject);
            Destroy(bestObject);
        }
    }

    // Executes a recipe by removing the ingredients from the world and spawning the outcome of the recipe
    public void craftPlayerRecipe(Recipe recipe) {
        // maybe save available items? Has some design consequences, todo discuss
        destroyRequiredMaterials(recipe.requiredItems);
        Instantiate(recipe.resultingItem, transform.position + transform.rotation * Vector3.forward,
            Quaternion.identity);
    }

    public void craftShipyardRecipe(Recipe recipe, GameObject shipyard) {
        destroyRequiredMaterials(recipe.requiredItems);
        shipyard.GetComponent<Shipyard>().handleCreateRecipe(recipe);
    }

    // Returns true if the given recipe can be made with a given list of items
    bool canMakeRecipe(Recipe recipe, Dictionary<Item, List<GameObject>> availableItems) {
        // We create a new copy (prevent pass-by-reference errors!!) and change the list of objects to a simple count
        var availableCount = availableItems.ToDictionary(entry => entry.Key, entry => entry.Value.Count);
        foreach (var requiredItem in recipe.requiredItems) {
            if (!availableCount.ContainsKey(requiredItem) || availableCount[requiredItem] <= 0) return false;
            availableCount[requiredItem]--;
        }

        return true;
    }

    // Returns the amount of necessary items for a given recipe and how many of those items are available
    public Dictionary<Item, Tuple<int, int>> getNeededItems(Recipe recipe) {
        var availableCount = availableItems.ToDictionary(entry => entry.Key, entry => entry.Value.Count);
        var requiredCount = recipe.requiredItems.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
        return requiredCount.Keys.ToDictionary(item => item, item =>
            new Tuple<int, int>(availableCount.GetOrDef(item), requiredCount[item]));
    }

    public void resetAvailableItems() {
        availableItems = new Dictionary<Item, List<GameObject>>();
    }

    private void OnTriggerEnter(Collider hit) {
        if (availableItems is null) return;
        var itemAssociation = hit.gameObject.GetComponent<ItemAssociation>();
        if (itemAssociation is null) return;
        if (!availableItems.ContainsKey(itemAssociation.item))
            availableItems.Add(itemAssociation.item, new List<GameObject> {hit.gameObject});
        else availableItems[itemAssociation.item].Add(hit.gameObject);
        craftUI.refreshUI();
    }

    private void OnTriggerExit(Collider hit) {
        if (availableItems is null) return;
        var itemAssociation = hit.gameObject.GetComponent<ItemAssociation>();
        if (itemAssociation is null) return;
        if (availableItems.ContainsKey(itemAssociation.item))
            availableItems[itemAssociation.item].Remove(hit.gameObject);
        craftUI.refreshUI();
    }
}