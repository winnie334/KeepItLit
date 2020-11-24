using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum tigerState {
    IDLE,
    CHASING,
} 

public class TigerAI : Animal {
    
    public float wanderRadius;

    public float viewDistance; // distance the tiger can look around for the player
    public Transform player;

    private Transform target;
    private tigerState state;
    
    public Vector2 wanderTimer; // Min and max time for next wander
    private float idleTimer;
    public float boredTimer; // Time until tiger gets bored of chasing the player
    private float attackingTimer; // Time we have been chasing the player
    public float cooldownThreshold; // Time the tiger ignores the player after having chased for a while
    private float cooldownTimer;

    private float nextWanderTimer; // The threshold we need to pass for our next wander
    private Vector3 lastPlayerPos; // The position the player was last seen from the point of view of the tiger

    // Use this for initialization
    void OnEnable () {
        nextWanderTimer = 0;
        cooldownTimer = cooldownThreshold;
    }

    new void Update () {
        base.Update();
        if (!agent.enabled) return;
        if (canSeePlayer()) {
            lastPlayerPos = player.position;

                // } else { // Get closest point on the circle of fire distance to the player
            //     lastPlayerPos = fire.transform.position + fireDistance *
            //         (player.position - fire.transform.position) /
            //         Vector3.Distance(player.position, fire.transform.position);
            // }
        }
            

        switch (state) {
            case tigerState.IDLE:
                cooldownTimer += Time.deltaTime;
                if (canSeePlayer() && cooldownTimer > cooldownThreshold) {
                    state = tigerState.CHASING;
                    attackingTimer = 0;
                    return;
                }
                idleTimer += Time.deltaTime;
                if (!(idleTimer >= nextWanderTimer)) return;
                agent.SetDestination(RandomNearAboveWater(transform.position, wanderRadius, -1));
                idleTimer = 0;
                nextWanderTimer = Random.Range(wanderTimer.x, wanderTimer.y);
                break;
            case tigerState.CHASING:
                attackingTimer += Time.deltaTime;
                if (attackingTimer > boredTimer) {
                    state = tigerState.IDLE;
                    idleTimer = nextWanderTimer; // Immediately repath to idle location
                    cooldownTimer = 0;
                    return;
                }

                agent.SetDestination(lastPlayerPos);
                break;
        }
    }

    private bool canSeePlayer() {
        RaycastHit hit;
        var rayDirection = player.position - transform.position;
        if (Physics.Raycast(transform.position, rayDirection, out hit, viewDistance)) {
            return hit.transform == player;
        }

        return false;
    }

    public void OnDrawGizmosSelected() {
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, transform.up, wanderRadius);
        Handles.color = Color.blue;
        Handles.DrawWireDisc(transform.position, transform.up, viewDistance);
        if (Application.isPlaying) Gizmos.DrawLine(transform.position, agent.destination);
    }

    public void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Player")) {
            var player = other.gameObject.GetComponent<PlayerMovement>();
            // Todo give player damage (and knockback?)
        }
    }
}
