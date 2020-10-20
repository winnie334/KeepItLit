using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Recipe : ScriptableObject
{
    public List<Item> requiredItems; //TODO maybe this should be a dictionary instead DICT<Item item, Int AmountRequired>
    public Item resultingItem;
}
