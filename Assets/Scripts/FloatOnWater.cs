using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatOnWater : MonoBehaviour
{
     private Transform seaTransform;


    // Start is called before the first frame update
    void Start()
    {
        seaTransform = GameObject.Find("Sea").transform;  
    }

    // Update is called once per frame
    void Update()
    {
        var pos = transform.position;
        transform.position = new Vector3(pos.x, seaTransform.position.y, pos.z);
    }
}
