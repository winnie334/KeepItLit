﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Animal : MonoBehaviour {

    public int maxHealth;
    public double recoveryTime; // After being attacked, counts down until 0, after which the animal can continue on again
    public List<GameObject> drops;

    private Rigidbody rb;
    private float distToGround;
    private int health;
    protected NavMeshAgent agent;

    private double fallenOverTime;
    protected AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start() {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        distToGround = GetComponent<Collider>().bounds.extents.y;
        health = maxHealth;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    // Must be called from inheriting classes!
    public void Update() {
        if (fallenOverTime > 0) {
            fallenOverTime -= Time.deltaTime;
        } else if (!rb.isKinematic && isGrounded()) {
            rb.isKinematic = true;
            agent.enabled = true;
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    public void takeDamage(int amount, Vector3 origin = new Vector3(), int force = 0) {
        health -= amount;
        audioSource.PlayOneShot(audioSource.clip);
        if (health <= 0) die();

        if (origin == Vector3.zero) return;
        var dir = Vector3.Normalize(transform.position - origin);
        dir = new Vector3(dir.x, 1.3f, dir.z);

        fallenOverTime = recoveryTime;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.isKinematic = false;
        // transform.position += dir;
        agent.enabled = false;
        rb.AddForce(dir * force, ForceMode.Impulse);
    }

    private void die() {
        Destroy(gameObject);
        foreach (var loot in drops) {
            var spawnPos = transform.position + (Vector3)Random.insideUnitCircle + new Vector3(0, 2, 0);
            Instantiate(loot, spawnPos, Quaternion.identity);
        }
    }

    private bool isGrounded() {
        return Physics.Raycast(transform.position, Vector3.down, distToGround + 0.4f) && Math.Abs(rb.velocity.y) <= 0.1f;
    }
    
    // Gets a random point in space on the navmesh within a certain radius of the given origin
    protected static Vector3 RandomNearPosition(Vector3 origin, float dist, int layermask) {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition (randDirection, out navHit, dist, layermask);
        return navHit.position;
    }

    // Calls RandomNearPosition multiple times until it receives a position which is above the water layer
    protected static Vector3 RandomNearAboveWater(Vector3 origin, float dist, int layermask) {
        var pos = RandomNearPosition(origin, dist, layermask);
        var attempts = 0;
        while (pos.y < 0.3 && attempts < 10) {
            attempts++;
            pos = RandomNearPosition(origin, dist, layermask);
        }

        return pos;
    }
}
