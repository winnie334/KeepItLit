using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Actions;
using UnityEngine;

public class CreateShipyard : MonoBehaviour, IAction, IOnEquip {
    public GameObject shipyard;
    public Material goodLocationMaterial;
    public Material badLocationMaterial;
    public float shipyardSpawnDistance = 5;
    public float maxShipyardSpawnHeight = 2;
    public AudioClip buildSound;

    private GameObject player;
    private GameObject projectedShipyard;
    private List<MeshRenderer> renderers = new List<MeshRenderer>();
    private bool shouldProject;

    public void execute(PlayerMovement playerMov) {
        var targetPos =  determineShipyardLocation();
        if (targetPos.y > maxShipyardSpawnHeight) return;
        
        Instantiate(shipyard, targetPos, Quaternion.identity);
        playerMov.removeObject(gameObject);
        playerMov.playSound(buildSound);
        Hints.displayHint("Awesome, here I can craft my boat");
        Destroy(gameObject);
    }

    private void Start() {
        player = GameObject.Find("Player");
        projectedShipyard = Instantiate(shipyard, determineShipyardLocation(), Quaternion.identity);
        projectedShipyard.GetComponent<Collider>().isTrigger = true;
        projectedShipyard.SetActive(false);
        
        renderers.Add(projectedShipyard.GetComponent<MeshRenderer>());
        foreach (Transform child in projectedShipyard.transform) {
            renderers.Add(child.GetComponent<MeshRenderer>());
            var script = child.GetComponent<Shipyard>();
            if (script) Destroy(script);
        }

    }

    private void Update() {
        if (!shouldProject) projectedShipyard.SetActive(false);
        var targetPos = determineShipyardLocation();
        colorProjectedShipyard(targetPos.y < maxShipyardSpawnHeight ? goodLocationMaterial : badLocationMaterial);
        projectedShipyard.transform.position = targetPos;
    }

    private void colorProjectedShipyard(Material color) {
        if (renderers[0].material == color) return; //already in the right color
        renderers.ForEach(r => r.material = color);
    }

    private Vector3 determineShipyardLocation() {
        var t = player.transform;
        var pos = t.position;
        return new Vector3(pos.x, pos.y - 1f,
            pos.z) + shipyardSpawnDistance * t.forward;
    }

    public void onEquip() {
        shouldProject = true;
        projectedShipyard.SetActive(true);
    }

    public void onUnEquip() {
        projectedShipyard.SetActive(false);
        shouldProject = false;
    }

    private void OnDestroy() {
        if (projectedShipyard) Destroy(projectedShipyard);
    }
}