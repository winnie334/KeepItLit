using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public enum chickenState {
    IDLE,
    FLEEING,
} 

public class ChickenAI : Animal {
    
    public float wanderRadius;
    public Vector2 wanderTimer; // Min and max time for next wander
 
    private Transform target;

    private float timer;
    private float nextWanderTimer; // The threshold we need to pass for our next wander
 
    // Use this for initialization
    void OnEnable () {
        nextWanderTimer = 0;
    }

    new void Update () {
        base.Update();
        timer += Time.deltaTime;
 
        if (timer >= nextWanderTimer && agent.enabled) {
            Vector3 newPos = RandomNearPosition(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
            nextWanderTimer = Random.Range(wanderTimer.x, wanderTimer.y);
        }    
    }
 
    // Gets a random point in space on the navmesh within a certain radius of the given origin
    private static Vector3 RandomNearPosition(Vector3 origin, float dist, int layermask) {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition (randDirection, out navHit, dist, layermask);
        return navHit.position;
    }
    
    public void OnDrawGizmosSelected() {
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, transform.up, wanderRadius);
        if (Application.isPlaying) Gizmos.DrawLine(transform.position, agent.destination);
    }
}
