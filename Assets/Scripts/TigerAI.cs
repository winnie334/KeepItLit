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
    
    private Transform player;
    public float attackDamage;

    public float daySpeed;
    public float nightSpeed;

    public float viewDistanceDay;
    public float viewDistanceNight;
    private float viewDistance; // distance the tiger can look around for the player
    
    private Transform target;
    private tigerState state;
    
    public Vector2 wanderTimer; // Min and max time for next wander
    private float idleTimer;
    public float boredTimer; // Time until tiger gets bored of chasing the player
    private float chaseTimer; // Time we have been chasing the player
    public float playerLostTimer; // How long the tiger has to not see the player to forget him
    private float lostTimer;
    public float cooldownThreshold; // Time the tiger ignores the player after having chased for a while
    private float cooldownTimer;
    public float attackDuration; // Time the tiger will stand still after attacking
    private float attackTimer; // If this is below the attackDuration the tiger will stand still

    private float nextWanderTimer; // The threshold we need to pass for our next wander
    private Vector3 lastPlayerPos; // The position the player was last seen from the point of view of the tiger

    private DayNightCycle dayNightCycle;

    // Use this for initialization
    void OnEnable () {
        nextWanderTimer = 0;
        cooldownTimer = cooldownThreshold;
        attackTimer = attackDuration;
        dayNightCycle = GameObject.Find("Sun").GetComponent<DayNightCycle>();
        player = GameObject.Find("Player").transform;
    }

    new void Update () {
        base.Update();
        if (!agent.enabled) return;
        
        // Not the cleanest to set this every loop, but in event-based the sun would need a list of tigers... or something
        agent.speed = dayNightCycle.isDay() ? daySpeed : nightSpeed; 
        viewDistance = dayNightCycle.isDay() ? viewDistanceDay : viewDistanceNight; 
        
        if (canSeePlayer()) {
            lastPlayerPos = player.position;
        }

        switch (state) {
            case tigerState.IDLE:
                cooldownTimer += Time.deltaTime;
                if (canSeePlayer() && cooldownTimer > cooldownThreshold) {
                    state = tigerState.CHASING;
                    chaseTimer = 0;
                    return;
                }
                idleTimer += Time.deltaTime;
                if (!(idleTimer >= nextWanderTimer)) return;
                agent.SetDestination(RandomNearAboveWater(transform.position, wanderRadius * (dayNightCycle.isDay() ? 1 : 2), -1));
                idleTimer = 0;
                nextWanderTimer = Random.Range(wanderTimer.x, wanderTimer.y);
                break;
            case tigerState.CHASING:
                if (attackTimer < attackDuration) {
                    attackTimer += Time.deltaTime;
                    return;
                }

                if (!canSeePlayer()) lostTimer += Time.deltaTime;

                // Chase until bored threshold is reached (twice as long at night)
                chaseTimer += Time.deltaTime;
                if (chaseTimer > (dayNightCycle.isDay() ? boredTimer : boredTimer * 2) || lostTimer > playerLostTimer) {
                    state = tigerState.IDLE;
                    idleTimer = nextWanderTimer; // Immediately repath to idle location
                    cooldownTimer = 0;
                    lostTimer = 0;
                    return;
                }

                agent.SetDestination(lastPlayerPos);
                break;
        }
    }

    private bool canSeePlayer() {
        RaycastHit hit;
        // Multiply by 2 since limiting rayCast distance in the cast itself doesn't seem to work properly
        var rayDirection = (player.position - transform.position) * 2;
        if (Physics.Raycast(transform.position, rayDirection, out hit)) {
            var hitPos = hit.transform.position;
            return Vector3.Distance(hitPos, player.position) < 0.3f && 
                   Vector3.Distance(transform.position, hitPos) < viewDistance; // We check here if it was actually in range
        }

        return false;
    }

    #if UNITY_EDITOR
    public void OnDrawGizmosSelected() {
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, transform.up, wanderRadius);
        Handles.color = Color.blue;
        Handles.DrawWireDisc(transform.position, transform.up, viewDistance);
        if (Application.isPlaying) Gizmos.DrawLine(transform.position, agent.destination);
    }
    #endif

    public void OnTriggerEnter(Collider other) {
        if (chaseTimer < attackDuration) return; // This is still the same attack
        if (other.gameObject.CompareTag("Player")) {
            var playerScript = other.gameObject.GetComponent<PlayerMovement>();
            playerScript.TakeDamage(attackDamage);
            chaseTimer = 0;
            // Todo play some attack animation and sound
        }
    }
}
