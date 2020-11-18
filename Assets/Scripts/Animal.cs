using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour {

    public int maxHealth;
    public double recoveryTime; // After being attacked, counts down until 0, after which the animal can continue on again

    private Rigidbody rb;
    private int health;
    protected NavMeshAgent agent;

    private double fallenOverTime;
    
    // Start is called before the first frame update
    void Start() {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        health = maxHealth;
    }

    // Update is called once per frame
    // Must be called from inheriting classes!
    public void Update() {
        if (fallenOverTime > 0) {
            fallenOverTime -= Time.deltaTime;
        } else if (rb.velocity.magnitude <= 0.1f) {
            rb.isKinematic = true;
            agent.enabled = true;
        }
    }

    public void takeDamage(int amount, Vector3 origin = new Vector3(), int force = 0) {
        health -= amount;
        if (health <= 0) die();

        if (origin == Vector3.zero) return;
        var dir = transform.position - origin;
        dir = dir.normalized;

        fallenOverTime = recoveryTime;
        rb.isKinematic = false;
        agent.enabled = false;
        rb.AddForce(dir * force, ForceMode.Impulse);
    }

    private void die() {
        Destroy(gameObject);
    }
}
