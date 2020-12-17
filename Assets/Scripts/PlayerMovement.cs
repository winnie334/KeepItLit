using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Actions;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour {
    public Transform cam;
    public CharacterController controller;

    public AudioSource audioSource;
    public AudioClip pickupSound;
    public AudioClip dropSound;

    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    public float pushPower = 2f;
    public float maxHealth = 100;
    public int maxToolsOnBack = 2;

    public HealthUI healthUI;

    [FormerlySerializedAs("weightLimit")] public float carryLimit = 3;

    private float turnSmoothVelocity;
    private float gravity;
    private List<GameObject> currentlyGrabbed = new List<GameObject>();
    private float currentHealth;
    private List<GameObject> toolsOnBack = new List<GameObject>(); //the tool the player has on its back

    private void Start() {
        currentHealth = maxHealth;
        healthUI.SetMaxHealth(maxHealth);
        healthUI.SetHealth(currentHealth);
    }

    // Update is called once per frame
    void Update() {
        handleMovement();
        if (Input.GetKeyDown("space")) handleGrab();
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) handleItemSwitch(true);
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) handleItemSwitch(false);
        if (Input.GetMouseButtonDown(0) && currentlyGrabbed.Count == 1)
            handleItemAction(); //currentlyGrabbed.Count condition is wonky xd

        // mainCamera.transform.position = transform.position + cameraOffset;
    }

    void handleMovement() {
        var originalPos = transform.position; // We save this to revert to it in case we do illegal movement (e.g drown)

        // Now we read the inputs and move our character accordingly
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        var direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (!(direction.magnitude >= 0.1f)) return;
        var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
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

    GameObject lookForClosestGrabbableItem(Item itemToCompare) {
        var curPos = transform.position;
        GameObject objectToGrab = Physics.OverlapSphere(curPos + transform.rotation * Vector3.forward * 3, 3)
            .Select(hit => hit.gameObject)
            .Where(obj => !(obj.GetComponent<ItemAssociation>() is null) && !currentlyGrabbed.Contains(obj) 
                                                                         && (itemToCompare is null || (itemToCompare.isTool ||
                                                                              obj.GetComponent<ItemAssociation>()
                                                                                  .item == itemToCompare)))
            .OrderBy(o => (o.transform.position - curPos).sqrMagnitude)
            .FirstOrDefault();
        return objectToGrab;
    }

    public void grabObject(GameObject objectToGrab) {
        if (objectToGrab is null) return; // Player tried to grab something when there was nothing in this range
        currentlyGrabbed.Add(objectToGrab);
        objectToGrab.transform.parent = transform; // One day we should make a better holding animation
        Vector3 localPosition = currentlyGrabbed.Count == 1
            ? new Vector3(0, 0, 1f)
            : new Vector3(0,
                (currentlyGrabbed.Count - 1) * objectToGrab.GetComponent<MeshFilter>().sharedMesh.bounds.size.y *
                objectToGrab.transform.localScale.y, 1f);
        objectToGrab.transform.localPosition = localPosition;
        objectToGrab.GetComponent<Rigidbody>().isKinematic = true;
        audioSource.PlayOneShot(pickupSound);
        objectToGrab.GetComponent<IOnEquip>()?.onEquip();
    }

    public void releaseObjects() {
        currentlyGrabbed.ForEach(grabbedItem => {
            grabbedItem.transform.parent = null;
            grabbedItem.GetComponent<Rigidbody>().isKinematic = false;
            grabbedItem.GetComponent<IOnEquip>()?.onUnEquip();
        });
        currentlyGrabbed = new List<GameObject>();
        audioSource.PlayOneShot(dropSound);
    }

    public void removeObject(GameObject obj) {
        obj.GetComponent<IOnEquip>()?.onEquip();
        currentlyGrabbed.Remove(obj);
    }

    void handleGrab() {
        if (currentlyGrabbed.Count == 0) grabObject(lookForClosestGrabbableItem(null));
        else {
            if (currentlyGrabbed.Count >= carryLimit) releaseObjects();
            else {
                var carriedItem = currentlyGrabbed[0].GetComponent<ItemAssociation>().item;
                var objectToGrab = lookForClosestGrabbableItem(carriedItem);
                if (objectToGrab is null) releaseObjects();
                else {
                    if (carriedItem.isTool) {
                        if (toolsOnBack.Count < maxToolsOnBack) {
                            putToolOnBack(currentlyGrabbed[0]);
                            currentlyGrabbed = new List<GameObject>();
                        }
                        else {
                            releaseObjects();
                            return;
                        }
                    }

                    grabObject(objectToGrab);
                }
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
        if (currentHealth - damage > 0) {
            currentHealth = Math.Max(currentHealth - damage, 0);
            healthUI.SetHealth(currentHealth);
        }
        else {
            Game.EndGame(false, "You died from damage");
        }
    }

    void putToolOnBack(GameObject tool) {
        toolsOnBack.Add(tool);
        tool.transform.parent = transform; // One day we should make a better holding animation
        tool.transform.localPosition = new Vector3(0, (toolsOnBack.Count - 1) * 0.5f, -1f);
    }

    void handleItemSwitch(bool isForwardScroll) {
        var indexOfTool = isForwardScroll ? 0 : toolsOnBack.Count - 1;
        if (currentlyGrabbed.Count > 0 && currentlyGrabbed[0].GetComponent<ItemAssociation>().item.isTool) {
            if (toolsOnBack.Count > 0) {
                var toolOnBack = toolsOnBack[indexOfTool];
                toolsOnBack.RemoveAt(indexOfTool);
                toolsOnBack.Insert(isForwardScroll ? toolsOnBack.Count : 0, currentlyGrabbed[0]);
                resetToolsOnBackPositions();
                currentlyGrabbed[0] = toolOnBack;
                currentlyGrabbed[0].transform.localPosition = new Vector3(0, 0, 1f);
            }
            else {
                putToolOnBack(currentlyGrabbed[0]);
                currentlyGrabbed = new List<GameObject>();
            }
        }
        else {
            if (toolsOnBack.Count == 0) return;
            releaseObjects();
            currentlyGrabbed.Add(toolsOnBack[indexOfTool]);
            toolsOnBack.RemoveAt(indexOfTool);
            if (indexOfTool == 0) resetToolsOnBackPositions();
            currentlyGrabbed[0].transform.localPosition = new Vector3(0, 0, 1f);
        }
    }

    void resetToolsOnBackPositions() {
        for (int i = 0; i < toolsOnBack.Count; i++) {
            toolsOnBack[i].transform.localPosition = new Vector3(0, i*0.5f, -1f);
        }
    }

    public void Heal(float life) {
        currentHealth = Math.Min(currentHealth + life, maxHealth);
        healthUI.SetHealth(currentHealth);
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