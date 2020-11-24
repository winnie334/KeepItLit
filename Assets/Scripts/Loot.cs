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
        foreach (var item in loot) {
            var spawnPos = transform.position + (Vector3) Random.insideUnitCircle + new Vector3(0, 2, 0);
            Instantiate(item, spawnPos, Quaternion.identity);
        }
    }
}