using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject {
    public string title;
    public Sprite icon;
    public float fuelSize;
    public bool isTool;
}