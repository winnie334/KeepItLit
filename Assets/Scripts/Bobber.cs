using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bobber : MonoBehaviour {

    public int minWaitTime = 6;
    public int maxWaitTime = 12;
    public AudioClip bobberSubmergeSound;

    private bool triggerDisabled;
    private bool inSea;

    private bool shouldDrop = true;
    private bool isDropped;
    private bool shouldDropFish;

    private Transform seaTransform;
    private Rigidbody rb;

    private AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        seaTransform = GameObject.Find("Sea").transform;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        if (inSea) {
            var t = transform;
            var pos = t.position;
            t.rotation = Quaternion.identity;
            if (!isDropped || t.position.y >= seaTransform.position.y) transform.position = new Vector3(pos.x, seaTransform.position.y, pos.z);
            if (shouldDrop) StartCoroutine(dropDobber());
        }
        else if (transform.position.y <= seaTransform.position.y) {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            inSea = true;
        }
    }

    public bool hasFish() {
        return shouldDropFish;
    }

    IEnumerator dropDobber() {
        shouldDrop = false;
        float waitTime = Random.Range (minWaitTime, maxWaitTime);
        yield return new WaitForSeconds (waitTime);
        isDropped = true;
        rb.velocity = Vector3.zero;
        rb.AddForce(new Vector3(0, -100f, 0));
        shouldDropFish = true;
        audioSource.PlayOneShot(bobberSubmergeSound); // TODO is this correct place? I don't understand this code
        StartCoroutine(liftDobber());
    }
    
    IEnumerator liftDobber() {
        yield return new WaitForSeconds (1);
        rb.velocity = Vector3.zero;
        rb.AddForce(new Vector3(0, 50f, 0));
        shouldDropFish = false;
        yield return new WaitForSeconds (2);
        shouldDrop = true;
        isDropped = false;
    }
    

    private void OnTriggerExit(Collider other) {
        if (triggerDisabled) return;
        GetComponent<Collider>().isTrigger = false;
        triggerDisabled = true;
    }
}
