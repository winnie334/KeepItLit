using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public CharacterController controller;
    public Transform mainCamera;
    public Vector3 cameraOffset;

    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    public float pushPower = 2f;

    private float turnSmoothVelocity;

    private float gravity;
    private GameObject currentlyGrabbed; // is either null or the object we are holding

    // Update is called once per frame
    void Update() {
        handleMovement();
        if (Input.GetKeyDown("space")) handleGrab();

        // Move the camera to a position above the player
        mainCamera.transform.position = transform.position - cameraOffset;
    }

    void handleMovement() {
        // First we move the controller down with gravity
        gravity -= 9.81f * Time.deltaTime;
        if (controller.isGrounded) gravity = 0;
        controller.Move(new Vector3(0, gravity, 0) * Time.deltaTime);

        // Now we read the inputs and move our character accordingly
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        var direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f) {
            var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity,
                turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0);
            var moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * (speed * Time.deltaTime)); //deltaTime to make game frame rate independent
        }
    }

    void handleGrab() {
        if (currentlyGrabbed == null) { // We are trying to pick up a new item
            var curPos = transform.position;
            GameObject objectToGrab = Physics.OverlapSphere(curPos + transform.rotation * Vector3.forward * 3, 3)
                .Select(hit => hit.gameObject)
                .Where(obj => obj.GetComponent<ItemAssociation>() != null)
                .OrderBy(o => (o.transform.position - curPos).sqrMagnitude)
                .FirstOrDefault();;
            if (objectToGrab == null) return; // Player tried to grab something when there was nothing in this range
            currentlyGrabbed = objectToGrab;
            objectToGrab.transform.parent = transform; // One day we should make a better holding animation
            objectToGrab.transform.localPosition = new Vector3(0, 0, 1f);
            objectToGrab.GetComponent<Rigidbody>().isKinematic = true; 
        } else { // We are dropping our current item
            currentlyGrabbed.transform.parent = null;
            currentlyGrabbed.GetComponent<Rigidbody>().isKinematic = false; 
            currentlyGrabbed = null;
        }
    }

    // If we run up against something with a rigidbody, we move it
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic) return;

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3) return;

        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.velocity = pushDir * pushPower;
    }
    
        
    void OnDrawGizmosSelected() {
        Gizmos.color = new Color(1, 1, 0, 0.75F);
        Gizmos.DrawSphere(transform.position+ transform.rotation * Vector3.forward * 2, 2);
    }
}