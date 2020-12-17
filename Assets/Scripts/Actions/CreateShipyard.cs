using System;
using System.Collections;
using System.Collections.Generic;
using Actions;
using UnityEngine;

public class CreateShipyard : MonoBehaviour, IAction, IOnEquip {
    public GameObject shipyard;
    public GameObject goodShipyard;
    public GameObject badShipyard;
    public float shipyardSpawnDistance = 5;
    public float maxShipyardSpawnHeight = 2;

    private GameObject player;
    private GameObject projectedShipyard;
    private bool shouldProject;

    public void execute(PlayerMovement playerMov) {
        var targetPos =  determineShipyardLocation();
        if (targetPos.y > maxShipyardSpawnHeight) return;
        
        Instantiate(shipyard, targetPos, Quaternion.identity);
        playerMov.removeObject(gameObject);
        Destroy(gameObject);
    }

    private void Start() {
        player = GameObject.Find("Player");
    }

    private void Update() {
        if (!shouldProject) return;
        if (projectedShipyard) Destroy(projectedShipyard);
        var targetPos = determineShipyardLocation();
        
        projectedShipyard = targetPos.y < maxShipyardSpawnHeight
            ? Instantiate(goodShipyard, targetPos, Quaternion.identity)
            : Instantiate(badShipyard, targetPos, Quaternion.identity);
    }

    public Vector3 determineShipyardLocation() {
        var t = player.transform;
        var pos = t.position;
        return new Vector3(pos.x, pos.y - 1f,
            pos.z) + shipyardSpawnDistance * t.forward;
    }

    public void onEquip() {
        shouldProject = true;
    }

    public void onUnEquip() {
        if (projectedShipyard) Destroy(projectedShipyard);
        shouldProject = false;
    }

    private void OnDestroy() {
        if (projectedShipyard) Destroy(projectedShipyard);
    }
}