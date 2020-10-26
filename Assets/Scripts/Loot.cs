using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public GameObject[] loot;
    private bool isQuitting;

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private void OnDestroy()
    {
        if (isQuitting) return;
        foreach (var item in loot)
        {
            Instantiate(item, new Vector3(transform.position.x, transform.position.y + 10, transform.position.z),
                Quaternion.identity);
        }
    }
}