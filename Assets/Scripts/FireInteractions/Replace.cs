using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Replace : MonoBehaviour, IFireInteraction {
    public GameObject newObject;

    public void onFireInteraction() {
        if (newObject == null) return;
        if (transform.parent) { // This food is being carried
            var player = transform.GetComponentInParent<PlayerMovement>();
            player.removeObject(gameObject);
            var obj = Instantiate(newObject, transform.position, Quaternion.identity);
            obj.GetComponent<Rigidbody>().isKinematic = true;
            player.addObject(obj);
            // player.grabObject(obj);
        } else {
            Instantiate(newObject, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
