using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Fire : MonoBehaviour {
    public float scaleFire;
    public float speedDecreasing;
    public float minScale;
    public int densityFire;
    public float damageFire;
    public GameObject col;
    private ParticleSystem part;
    private ParticleSystem.ShapeModule sh;
    private ParticleSystem.EmissionModule em;

    void Start() {
        col.transform.localScale = Vector3.one * scaleFire * 100;
        part = GetComponent<ParticleSystem>();
        sh = part.shape;
        em = part.emission;
        sh.scale = Vector3.one * scaleFire;
        em.rateOverTime = (ParticleSystem.MinMaxCurve)(System.Math.Pow(scaleFire, 3) * densityFire);
    }

    void Update() {
        if (!part.isStopped) {
            if (sh.scale.magnitude < minScale) {
                part.Stop();
                GetComponent<NavMeshObstacle>().enabled = false; // Todo verify this works
            } else if (!part.isStopped) {
                col.transform.localScale -= Vector3.one * speedDecreasing * 100 * Time.deltaTime;

                sh.scale -= Vector3.one * speedDecreasing * Time.deltaTime;
                em.rateOverTime = (ParticleSystem.MinMaxCurve)(System.Math.Pow(sh.scale.magnitude, 3) * densityFire);
            }
        }
    }

    void OnTriggerEnter(Collider collider) {
        Debug.Log("Something is in the fire");
        if (collider.gameObject.CompareTag("Item")) {
            var item = collider.gameObject.GetComponent<ItemAssociation>().item;
            if (item.fuelSize > 0) {
                col.transform.localScale += Vector3.one * item.fuelSize * 100;

                sh.scale += Vector3.one * item.fuelSize;
                em.rateOverTime = (ParticleSystem.MinMaxCurve)(System.Math.Pow(sh.scale.magnitude, 3));
                Destroy(collider.gameObject);
            }

        } else if (collider.gameObject.CompareTag("Player")) {
            collider.gameObject.GetComponent<PlayerMovement>().TakeDamage(damageFire * Time.deltaTime);
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            other.gameObject.GetComponent<PlayerMovement>().TakeDamage(damageFire * Time.deltaTime);
        }
    }
}
