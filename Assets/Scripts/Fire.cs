using System;
using System.Linq;
using Actions;
using UnityEngine;
using UnityEngine.AI;


public class Fire : MonoBehaviour {
    public float initialSizeFire;
    public float speedDecreasing;
    public int densityFire;

    public float damageFire;
    public Light lightFire;
    public GameObject col;
    public PlayerMovement player;

    private ParticleSystem part;
    private ParticleSystem.ShapeModule sh;
    private ParticleSystem.EmissionModule em;

    void Start() {
        col.transform.localScale = Vector3.one * initialSizeFire * 100;
        part = GetComponent<ParticleSystem>();
        sh = part.shape;
        em = part.emission;
        sh.scale = Vector3.one * initialSizeFire;
        em.rateOverTime = (ParticleSystem.MinMaxCurve)(Math.Pow(initialSizeFire, 3) * densityFire);
    }

    void Update() {
        if (!part.isStopped) {
            if (sh.scale.z < 0) {
                part.Stop();
                lightFire.intensity = 0;
                GetComponent<NavMeshObstacle>().enabled = false;
            } else if (!part.isStopped) {
                // TODO collider size doesn't match up with fire
                col.transform.localScale -= Vector3.one * (speedDecreasing * 100 * Time.deltaTime);
                lightFire.range = col.transform.localScale.magnitude;
                sh.scale -= Vector3.one * (speedDecreasing * Time.deltaTime);

                em.rateOverTime = (ParticleSystem.MinMaxCurve)(Math.Pow(sh.scale.magnitude, 3) * densityFire);
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        if (sh.scale.magnitude < initialSizeFire && other.gameObject.CompareTag("Item")) {
            var item = other.gameObject.GetComponent<ItemAssociation>().item;
            if (item.fuelSize > 0) {
                col.transform.localScale += Vector3.one * item.fuelSize * 100;

                sh.scale += Vector3.one * item.fuelSize;
                em.rateOverTime = (ParticleSystem.MinMaxCurve)(Math.Pow(sh.scale.magnitude, 3));
                player.removeObject(other.gameObject);
                Destroy(other.gameObject);
            } else if (other.gameObject.GetComponent<Food>() != null) {
                var foodToCook = other.gameObject.GetComponent<Food>();
                foodToCook.cookFood();
            }
        }
    }


    private void OnTriggerStay(Collider other) {
        if (other.bounds.size.x > 10) return; //TODO this is a stupid hack to not trigger this function with the collider that is being used for automatic crafting ui
        if (other.gameObject.CompareTag("Player") && !part.isStopped) {
            other.gameObject.GetComponent<PlayerMovement>().TakeDamage(damageFire * Time.deltaTime);
        }
    }

}
