using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bobber : MonoBehaviour {

    public int minWaitTime = 1;
    public int maxWaitTime = 6;

    private bool triggerDisabled;
    private bool inSea;

    private bool shouldDrop = true;
    private bool isDropped;

    private Transform seaTransform;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        seaTransform = GameObject.Find("Sea").transform;
    }

    // Update is called once per frame
    void Update() {
        if (inSea) {
            var pos = transform.position;
            if (!isDropped) transform.position = new Vector3(pos.x, seaTransform.position.y, pos.z);
            if (shouldDrop) StartCoroutine(dropDobber());
        }
        else if (transform.position.y <= seaTransform.position.y) {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            transform.rotation = Quaternion.identity;
            inSea = true;
        }
    }

    public bool hasFish() {
        return isDropped;
    }

    IEnumerator dropDobber() {
        shouldDrop = false;
        float waitTime = Random.Range (minWaitTime, maxWaitTime);
        yield return new WaitForSeconds (waitTime);
        isDropped = true;
        rb.AddForce(new Vector3(0, -50f, 0));
        StartCoroutine(liftDobber());
    }
    
    IEnumerator liftDobber() {
        yield return new WaitForSeconds (1);
        shouldDrop = true;
        isDropped = false;
    }
    

    private void OnTriggerExit(Collider other) {
        if (triggerDisabled) return;
        GetComponent<Collider>().isTrigger = false;
        triggerDisabled = true;
    }
}
