using System;
using UnityEngine;
using UnityEngine.AI;


public class Fire : MonoBehaviour {
    public float fireSize; // The current size of the fire, also used for initial value
    public float maximalSizeFire;
    public float speedDecreasing;
    public int densityFire;
    public int damageColliderSize = 100;
    public int fireColliderSize = 150;

    public float damageFire;
    public Light lightFire;
    public float lightRange;
    public GameObject damageCol;
    public GameObject col;
    public PlayerMovement player;

    private ParticleSystem part;
    private ParticleSystem.ShapeModule sh;
    private ParticleSystem.EmissionModule em;

    void Start() {
        part = GetComponent<ParticleSystem>();
        sh = part.shape;
        em = part.emission;
        updateParts();
    }

    void Update() {
        if (part.isStopped) return;
        if (sh.scale.z < 0) {
            part.Stop();
            lightFire.intensity = 0;
            GetComponent<NavMeshObstacle>().enabled = false;
        } else {
            fireSize -= speedDecreasing * Time.deltaTime;
            updateParts();
        }
    }

    // Updates the particle system and the colliders
    void updateParts() {
        damageCol.transform.localScale = Vector3.one * (fireSize * damageColliderSize);
        col.transform.localScale = Vector3.one * (Math.Max(fireSize, 0.5f) * fireColliderSize);
        lightFire.range = Math.Min(fireSize, maximalSizeFire) * lightRange;
        sh.scale = Vector3.one * fireSize;
        em.rateOverTime = (ParticleSystem.MinMaxCurve)(Math.Pow(sh.scale.magnitude, 3) * densityFire);
    }

    public void OnChildTriggerStay(string type, Collider other) {
        switch (type) {
            case "damage":
                if (other.bounds.size.x > 10) return; //TODO this is a stupid hack to not trigger this function with thedamageCollider that is being used for automatic crafting ui
                if (other.gameObject.CompareTag("Player") && !part.isStopped) {
                    other.gameObject.GetComponent<PlayerMovement>().TakeDamage(damageFire * Time.deltaTime);
                }
                break;

            default:
                if (other.gameObject.CompareTag("Item")) {
                    var item = other.gameObject.GetComponent<ItemAssociation>().item;
                    if (item.fuelSize > 0 && fireSize < maximalSizeFire) {
                        fireSize += item.fuelSize;
                        player.removeObject(other.gameObject);
                        updateParts();
                        Destroy(other.gameObject);
                    } else if (other.gameObject.GetComponent<IFireInteraction>() != null) {
                        var interactableItem = other.gameObject.GetComponent<IFireInteraction>();
                        interactableItem.onFireInteraction();
                    }
                }
                break;

        }

    }

}
