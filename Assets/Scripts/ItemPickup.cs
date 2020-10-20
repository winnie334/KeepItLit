using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO should be interactable
public class ItemPickup : MonoBehaviour
{
    public Item item;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("spawning: " + item.title);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
