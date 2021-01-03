using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Actions;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour {
    public Transform cam;
    public Transform hand;
    public Transform back;
    public CharacterController controller;
    public Animator anim;

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
    private Vector3 impact = Vector3.zero;

    private void Start() {
        currentHealth = maxHealth;
        healthUI.SetMaxHealth(maxHealth);
        healthUI.SetHealth(currentHealth);
        Hints.displayHint("You can pick up things with space");
    }

    // Update is called once per frame
    void Update() {
        handleMovement();
        if (Input.GetKeyDown("space")) handleGrab();
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) handleItemSwitch(true);
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) handleItemSwitch(false);
        if (Input.GetMouseButtonDown(0) && currentlyGrabbed.Count == 1 && currentlyGrabbed[0].GetComponents<IAction>().Length > 0) {
            anim.SetInteger("Extract", anim.GetInteger("Extract") + 1);
        }

        // mainCamera.transform.position = transform.position + cameraOffset;
    }

    void handleMovement() {
        var originalPos = transform.position; // We save this to revert to it in case we do illegal movement (e.g drown)

        // Now we read the inputs and move our character accordingly
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        var direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (impact.magnitude > 0.2) controller.Move(impact * Time.deltaTime);
        else if (direction.magnitude >= 0.1f) {
            anim.SetBool("Walk", true);
            anim.SetInteger("Extract", 0);
            var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity,
                turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0);
            var moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * (speed * Time.deltaTime));
        } else {
            anim.SetBool("Walk", false);
        }

        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);

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

    public void grabObject() {
        var objectToGrab = currentlyGrabbed[currentlyGrabbed.Count - 1];
        if (objectToGrab is null) return; // Player tried to grab something when there was nothing in this range
        anim.Play("metarig|Grab");
        Hints.displayHintOnGrab(objectToGrab.GetComponent<ItemAssociation>().item.title);

        objectToGrab.transform.SetParent(hand); // One day we should make a better holding animation
        Vector3 localPosition = currentlyGrabbed.Count == 1
            ? new Vector3(0, 0, 0)
            : new Vector3(0,
                (currentlyGrabbed.Count - 1) * objectToGrab.GetComponent<MeshFilter>().sharedMesh.bounds.size.y *
                objectToGrab.transform.localScale.y, 0);
        objectToGrab.transform.localPosition = localPosition;
        objectToGrab.GetComponent<Rigidbody>().isKinematic = true;
        audioSource.PlayOneShot(pickupSound);
        objectToGrab.GetComponent<IOnEquip>()?.onEquip();
    }

    public void releaseObjects() {
        anim.SetBool("Grab", false);
        currentlyGrabbed.ForEach(grabbedItem => {
            grabbedItem.transform.parent = null;
            grabbedItem.GetComponent<Rigidbody>().isKinematic = false;
            grabbedItem.GetComponent<IOnEquip>()?.onUnEquip();
        });
        currentlyGrabbed = new List<GameObject>();
        audioSource.PlayOneShot(dropSound);
    }

    public void removeObject(GameObject obj) {
        anim.SetBool("Grab", false);
        obj.GetComponent<IOnEquip>()?.onUnEquip();
        currentlyGrabbed.Remove(obj);
    }

    public void handleGrab() {
        if (currentlyGrabbed.Count == 0) {
            var objectToGrab = lookForClosestGrabbableItem(null);
            if (objectToGrab is null) {
                return;
            }
            currentlyGrabbed.Add(objectToGrab);
            anim.SetBool("Grab", true);

        } else {
            if (currentlyGrabbed.Count >= carryLimit) releaseObjects();
            else {
                var carriedItem = currentlyGrabbed[0].GetComponent<ItemAssociation>().item;
                var objectToGrab = lookForClosestGrabbableItem(carriedItem);
                if (objectToGrab is null) releaseObjects();
                else {
                    if (carriedItem.isTool) {
                        releaseObjects();
                        return;
                    }

                    currentlyGrabbed.Add(objectToGrab);
                    anim.Play("metarig|Grab");
                }
            }
        }
    }

    void handleItemAction() {
        anim.SetInteger("Extract", anim.GetInteger("Extract") - 1);
        var actions = currentlyGrabbed[0].GetComponents<IAction>();
        foreach (var action in actions) {
            action.execute(this);
        }
    }

    public void TakeDamage(float damage) {
        if (currentHealth - damage > 0) {
            currentHealth = Math.Max(currentHealth - damage, 0);
            healthUI.SetHealth(currentHealth);
        } else {
            Game.EndGame(false, "You died...");
        }
    }

    public void takeDamageWithImpact(Vector3 dir, float force, float damage) {
        TakeDamage(damage);
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        impact += dir.normalized * force / 1;
    }


    void handleItemSwitch(bool isForwardScroll) {
        var indexOfTool = isForwardScroll ? 0 : toolsOnBack.Count - 1;
        anim.SetBool("PutOnBack", true);
        if (currentlyGrabbed.Count > 0 && currentlyGrabbed[0].GetComponent<ItemAssociation>().item.isTool) {
            if (toolsOnBack.Count < maxToolsOnBack) {
                toolsOnBack.Insert(isForwardScroll ? 0 : toolsOnBack.Count, currentlyGrabbed[0]);
                currentlyGrabbed = new List<GameObject>();
                anim.SetBool("Grab", false);
            } else {
                return;
            }
        } else {
            if (toolsOnBack.Count == 0) return;
            releaseObjects();
            currentlyGrabbed.Add(toolsOnBack[indexOfTool]);
            toolsOnBack.RemoveAt(indexOfTool);
            anim.SetBool("Grab", true);
        }
    }

    void resetToolsOnBackPositions() {
        for (int i = 0; i < toolsOnBack.Count; i++) {
            Debug.Log("reset back");
            toolsOnBack[i].transform.SetParent(back);
            toolsOnBack[i].transform.localPosition = new Vector3(0, 0, 0);
        }
        if (currentlyGrabbed.Count > 0) {
            Debug.Log("reset hand");
            currentlyGrabbed[0].transform.SetParent(hand);
        }


        anim.SetBool("PutOnBack", false);
    }

    public void Heal(float life) {
        currentHealth = Math.Min(currentHealth + life, maxHealth);
        healthUI.SetHealth(currentHealth);
    }

    public void playSound(AudioClip clip) {
        if (clip == null) {
            Debug.Log("Empty clip passed!");
            return;
        }
        audioSource.PlayOneShot(clip);
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