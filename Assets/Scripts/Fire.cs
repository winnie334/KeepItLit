using UnityEngine;

public class Fire : MonoBehaviour {
    public float scaleFire;
    public float speedDreasing;
    public float minScale;
    public int densityFire;
    private ParticleSystem part;
    private ParticleSystem.ShapeModule sh;
    private ParticleSystem.EmissionModule em;

    void Start() {
        part = this.GetComponent<ParticleSystem>();
        sh = part.shape;
        em = part.emission;
        sh.scale = Vector3.ClampMagnitude(Vector3.one, scaleFire);
        em.rateOverTime = (ParticleSystem.MinMaxCurve)(System.Math.Pow(scaleFire, 3) * densityFire);
    }

    void Update() {
        Collider[] woods = Physics.OverlapSphere(this.transform.position, sh.scale.x + 1);
        foreach (var wood in woods) {
            if (!part.isStopped && wood.gameObject.GetComponent<Wood>()) {
                sh.scale += Vector3.ClampMagnitude(Vector3.one, wood.gameObject.GetComponent<Wood>().size);
                em.rateOverTime = (ParticleSystem.MinMaxCurve)(System.Math.Pow(sh.scale.magnitude, 3));
                Destroy(wood.gameObject);
            }
        }

        if (!part.isStopped && sh.scale.magnitude < minScale) {
            part.Stop();
        } else if (!part.isStopped) {
            sh.scale -= Vector3.ClampMagnitude(Vector3.one, speedDreasing * Time.deltaTime);
            em.rateOverTime = (ParticleSystem.MinMaxCurve)(System.Math.Pow(sh.scale.magnitude, 3) * densityFire);
        }

    }
}
