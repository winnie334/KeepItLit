using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Actions;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour {
    public CharacterController controller;
    public Transform mainCamera;
    public Vector3 cameraOffset;

    public AudioSource audioSource;
    public AudioClip pickupSound;
    public AudioClip dropSound;

    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    public float pushPower = 2f;
    public float maxHealth = 100;
    public float actualHealth = 100;

    public HealthUI healthUI;

    [FormerlySerializedAs("weightLimit")] public float carryLimit = 3;

    private float turnSmoothVelocity;
    private float gravity;
    private List<GameObject> currentlyGrabbed = new List<GameObject>();

    private void Start() {
        healthUI.SetMaxHealth(maxHealth);
        healthUI.SetHealth(actualHealth);
    }

    // Update is called once per frame
    void Update() {
        handleMovement();
        if (Input.GetKeyDown("space")) handleGrab();
        if (Input.GetMouseButtonDown(0) && currentlyGrabbed.Count == 1)
            handleItemAction(); //currentlyGrabbed condition is wonky xd

        // Move the camera to a position above the player
        mainCamera.transform.position = transform.position - cameraOffset;
    }

    void handleMovement() {

        var originalPos = transform.position; // We save this to revert to it in case we do illegal movement (e.g drown)

        // Now we read the inputs and move our character accordingly
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        var direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (!(direction.magnitude >= 0.1f)) return;
        var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity,
            turnSmoothTime);

        transform.rotation = Quaternion.Euler(0f, angle, 0);
        var moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        controller.Move(moveDir.normalized * (speed * Time.deltaTime));
        
        // First we move the controller down with gravity
        gravity -= 9.81f * Time.deltaTime;
        if (controller.isGrounded) gravity = 0;
        controller.Move(new Vector3(0, gravity, 0) * Time.deltaTime);

        if (outOfBounds(transform.position, -2.3f)) transform.position = originalPos;

    }

    // Raycast down from position to see if we would fall below the height parameter
    private bool outOfBounds(Vector3 pos, float height) {
        RaycastHit hit;
        if (Physics.Raycast(pos, Vector3.down, out hit, 100)) {
            return hit.point.y <= height;
        }

        return true;
    }

    GameObject lookForClosestGrabbableItem(GameObject itemToCompare) {
        var curPos = transform.position;
        GameObject objectToGrab = Physics.OverlapSphere(curPos + transform.rotation * Vector3.forward * 3, 3)
            .Select(hit => hit.gameObject)
            .Where(obj => obj.GetComponent<ItemAssociation>() != null
                          && (itemToCompare is null || (!currentlyGrabbed.Contains(obj) &&
                                                        obj.GetComponent<ItemAssociation>().item
                                                        == itemToCompare.GetComponent<ItemAssociation>().item)))
            .OrderBy(o => (o.transform.position - curPos).sqrMagnitude)
            .FirstOrDefault();
        ;
        return objectToGrab;
    }

    void grabObject(GameObject objectToGrab) {
        if (objectToGrab is null) return; // Player tried to grab something when there was nothing in this range
        currentlyGrabbed.Add(objectToGrab);
        objectToGrab.transform.parent = transform; // One day we should make a better holding animation
        Vector3 localPosition = currentlyGrabbed.Count == 1
            ? new Vector3(0, 0, 1f)
            : new Vector3(0, currentlyGrabbed.Count - 1, 1f);
        objectToGrab.transform.localPosition = localPosition;
        objectToGrab.GetComponent<Rigidbody>().isKinematic = true;
        audioSource.PlayOneShot(pickupSound);
    }

    void releaseObjects() {
        currentlyGrabbed.ForEach(grabbedItem =>
        {
            grabbedItem.transform.parent = null;
            grabbedItem.GetComponent<Rigidbody>().isKinematic = false;
        });
        currentlyGrabbed = new List<GameObject>();
        audioSource.PlayOneShot(dropSound);
    }

    public void removeObject(GameObject obj) {
        currentlyGrabbed.Remove(obj);
    }

    void handleGrab() {
        if (currentlyGrabbed.Count == 0) grabObject(lookForClosestGrabbableItem(null));
        else {
            if (currentlyGrabbed.Count >= carryLimit) releaseObjects();
            else {
                var objectToGrab = lookForClosestGrabbableItem(currentlyGrabbed[0]);
                if (objectToGrab is null) releaseObjects();
                else grabObject(objectToGrab);
            }
        }
    }

    void handleItemAction() {
        var actions = currentlyGrabbed[0].GetComponents<IAction>();
        foreach (var action in actions) {
            action.execute(this);
        }
    }

    public void TakeDamage(float damage) {
        actualHealth = Math.Max(actualHealth - damage, 0);
        healthUI.SetHealth(actualHealth);
    }

    public void Heal(float life) {
        actualHealth = Math.Min(actualHealth + life, maxHealth);
        healthUI.SetHealth(actualHealth);
    }

    // If we run up against something with a rigidbody, we move it
    void OnControllerColliderHit(ControllerColliderHit hit) {
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
        Gizmos.DrawSphere(transform.position + transform.rotation * Vector3.forward * 2, 2);
    }
}