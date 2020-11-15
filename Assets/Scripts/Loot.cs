using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour {
    public GameObject[] loot;
    private bool isQuitting;

    private void OnApplicationQuit() {
        isQuitting = true;
    }

    private void OnDestroy() {
        if (isQuitting) return;
        foreach (var item in loot)
        {
            var position = transform.position;
            Instantiate(item, new Vector3(position.x, position.y + 10, position.z),
                Quaternion.identity);
        }
    }
}