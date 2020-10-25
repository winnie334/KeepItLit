    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public float timeDecreasing;
    private float decrease;

    // Start is called before the first frame update
    void Start()
    {
        decrease = Time.time + timeDecreasing;   
    }

    // Update is called once per frame
     void Update()
    {
        var part = this.GetComponent<ParticleSystem>();
        var sh = part.shape;
        var em = part.emission;

        Collider[] woods = Physics.OverlapSphere(this.transform.position, sh.scale.x + 1);
        foreach (var wood in woods)
        {
            if (wood.gameObject.tag == "Wood" && !part.isStopped)
            {
                sh.scale *= 2;
                em.rateOverTime = em.rateOverTime.constant*2;
                Destroy(wood.gameObject);
            }
        }

        if (decrease < Time.time && !part.isStopped)
        {
            sh.scale /= 2;
            em.rateOverTime = em.rateOverTime.constant / 2;
            decrease = Time.time + timeDecreasing;
        }

        if (sh.scale.magnitude < 0.1)
        {
            part.Stop();
        }
         
    }
}
