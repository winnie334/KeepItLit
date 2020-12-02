using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Loots {
    public GameObject resource;
    public int number;
}

public class Loot : MonoBehaviour {
    public List<Item> tools;
    public List<Loots> loots;
    public int maxHealth;
    public int stepNumber;

    private int health;
    private int step;
    private int nextStep;
    private int resourcesByStep;

    private void Start() {
        health = maxHealth;
        step = health / (stepNumber + 1);
        nextStep = health - step;

        var totalResources = 0;
        loots.ForEach(loot => { totalResources += loot.number; });

        resourcesByStep = totalResources / (stepNumber + 1);
    }

    public void Extract(Item item, int damage) {
        if (!tools.Contains(item)) {
            Debug.Log("Wrong tool");
            return;
        }

        health -= damage;
        Debug.Log(health);

        if (health <= 0) {
            foreach (var loot in loots) {
                for (var i = 0; i < loot.number; i++) {
                    var spawnPos = transform.position + (Vector3)Random.insideUnitCircle + new Vector3(0, 2, 0);
                    Instantiate(loot.resource, spawnPos, Quaternion.identity);
                }
                Destroy(gameObject);
            }
        } else {
            while (nextStep >= health) {
                nextStep -= step;
                var resourcesExtracted = 0;
                while (resourcesExtracted < resourcesByStep) {
                    var diff = resourcesByStep - resourcesExtracted;

                    var loot = loots[loots.Count - 1];
                    loots.RemoveAt(loots.Count - 1);

                    if (loot.number > diff) {
                        loot.number -= diff;
                        loots.Add(loot);
                    } else {
                        diff = loot.number;
                    }

                    for (var i = 0; i < diff; i++) {
                        var spawnPos = transform.position + (Vector3)Random.insideUnitCircle + new Vector3(0, 2, 0);
                        Instantiate(loot.resource, spawnPos, Quaternion.identity);
                    }
                    resourcesExtracted += diff;
                }
            }
        }

    }
}