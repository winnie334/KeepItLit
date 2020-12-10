using System;
using System.Linq;
using Actions;
using UnityEngine;
using UnityEngine.AI;


public class Fire : MonoBehaviour {
    public float initalSizeFire;
    public float maximalSizeFire;
    public float speedDecreasing;
    public int densityFire;

    public float damageFire;
    public Light lightFire;
    public GameObject damageCol;
    public GameObject col;
    public PlayerMovement player;

    private ParticleSystem part;
    private ParticleSystem.ShapeModule sh;
    private ParticleSystem.EmissionModule em;

    void Start() {
        damageCol.transform.localScale = Vector3.one * initalSizeFire * 100;
        col.transform.localScale = Vector3.one * initalSizeFire * 150;
        part = GetComponent<ParticleSystem>();
        sh = part.shape;
        em = part.emission;
        sh.scale = Vector3.one * initalSizeFire;
        em.rateOverTime = (ParticleSystem.MinMaxCurve)(Math.Pow(initalSizeFire, 3) * densityFire);
    }

    void Update() {
        if (!part.isStopped) {
            if (sh.scale.z < 0) {
                part.Stop();
                lightFire.intensity = 0;
                GetComponent<NavMeshObstacle>().enabled = false;
            } else if (!part.isStopped) {
                // TODOdamageCollider size doesn't match up with fire
                damageCol.transform.localScale -= Vector3.one * (speedDecreasing * 100 * Time.deltaTime);
                col.transform.localScale -= Vector3.one * (speedDecreasing * 150 * Time.deltaTime);
                lightFire.range = damageCol.transform.localScale.magnitude;
                sh.scale -= Vector3.one * (speedDecreasing * Time.deltaTime);

                em.rateOverTime = (ParticleSystem.MinMaxCurve)(Math.Pow(sh.scale.magnitude, 3) * densityFire);
            }
        }
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
                    if (item.fuelSize > 0 && sh.scale.x < maximalSizeFire) {
                        damageCol.transform.localScale += Vector3.one * item.fuelSize * 100;
                        col.transform.localScale += Vector3.one * item.fuelSize * 150;

                        sh.scale += Vector3.one * item.fuelSize;
                        em.rateOverTime = (ParticleSystem.MinMaxCurve)(Math.Pow(sh.scale.magnitude, 3));
                        player.removeObject(other.gameObject);
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
