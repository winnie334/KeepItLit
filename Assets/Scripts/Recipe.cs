using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Recipe : ScriptableObject {
    public String description;
    public List<Item> requiredItems; // Making this a serializable dictionary for Unity is lethal :skull:
    public GameObject resultingItem;
}
