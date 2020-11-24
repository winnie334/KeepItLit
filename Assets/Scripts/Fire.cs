using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Fire : MonoBehaviour {
    public float scaleFire;
    public float speedDecreasing;
    public float minScale;
    public int densityFire;
    private ParticleSystem part;
    private ParticleSystem.ShapeModule sh;
    private ParticleSystem.EmissionModule em;

    void Start() {
        part = GetComponent<ParticleSystem>();
        sh = part.shape;
        em = part.emission;
        sh.scale = Vector3.ClampMagnitude(Vector3.one, scaleFire);
        em.rateOverTime = (ParticleSystem.MinMaxCurve)(System.Math.Pow(scaleFire, 3) * densityFire);
    }

    void Update() {
        if (!part.isStopped) {
            var collids = Physics.OverlapSphere(transform.position, sh.scale.magnitude);
            collids = collids.Where(x => x.CompareTag("Item")).ToArray<Collider>();

            foreach (var collid in collids) {
                var item = collid.gameObject.GetComponent<ItemAssociation>().item;
                if (item.fuelSize > 0) {
                    sh.scale += Vector3.ClampMagnitude(Vector3.one, item.fuelSize);
                    em.rateOverTime = (ParticleSystem.MinMaxCurve)(System.Math.Pow(sh.scale.magnitude, 3));
                    // todo fix particle amount (or real fire animation)
                    // Todo update light size
                    Destroy(collid.gameObject);
                }
            }

            if (sh.scale.magnitude < minScale) {
                part.Stop();
                GetComponent<NavMeshObstacle>().enabled = false; // Todo verify this works
            } else if (!part.isStopped) {
                sh.scale -= Vector3.ClampMagnitude(Vector3.one, speedDecreasing * Time.deltaTime);
                em.rateOverTime = (ParticleSystem.MinMaxCurve)(System.Math.Pow(sh.scale.magnitude, 3) * densityFire);
            }
        }
    }
}
