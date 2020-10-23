using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SphereMovment : MonoBehaviour
{
    public float speed; // Speed of the sphere
    public float jumpHeight; // Heigth of the jump of the sphere
    public bool canHold; // Boolean to know if it holds an object
    public GameObject obj; //Object to hold
    public Transform guide;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Get object this turn 
        Boolean grabbed = false;

        //Get input key to move the object
        float transx = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        float transy = Input.GetAxis("Vertical") * Time.deltaTime * speed;

        var nearest = float.MaxValue;
        GameObject nearestObj = null;

        Collider[] hitsCollider = Physics.OverlapSphere(transform.position, 3);
        foreach (var hitCollider in hitsCollider)
        {
            if (hitCollider.gameObject.tag == "Holdable")
            {
                var dist = Vector3.Distance(hitCollider.transform.position, transform.position);
                if ( dist < nearest)
                {
                    nearest = dist;
                    nearestObj = hitCollider.gameObject;
                }
            }
           
            
        }

        if (!obj && nearestObj && Input.GetKeyDown("g"))
        {
            obj =nearestObj;
            obj.transform.SetParent(guide);
            obj.GetComponent<Rigidbody>().useGravity = false;
            grabbed = true;
            Debug.Log("Grabbed " + obj.name);
        }

        if (Input.GetKeyDown("space"))
        {
            GetComponent<Rigidbody>().AddForce(0, jumpHeight, 0, ForceMode.VelocityChange);
        }

        if (Input.GetKeyDown("g") && obj && !grabbed)
        {
            obj.transform.parent = null;
            obj.GetComponent<Rigidbody>().useGravity = true;
            obj = null;
        }

        transform.Translate(transx, 0, transy, Space.World);
        if (obj)
            obj.transform.position = guide.position;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Holdable" && !obj)
        {
            obj = other.gameObject;
            obj.transform.SetParent(guide);
            obj.GetComponent<Rigidbody>().useGravity = false;

            Debug.Log("Collision");
        }
       
    }
}
