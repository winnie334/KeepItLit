using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Replace : MonoBehaviour, IFireInteraction {
    public GameObject newObject;
    
    public void onFireInteraction() {
        if (newObject == null) return;
        if (transform.parent) { // This food is being carried
            var player = transform.parent.GetComponent<PlayerMovement>();
            player.removeObject(gameObject);
            var instantiate = Instantiate(newObject, transform.position, Quaternion.identity);
            player.grabObject(instantiate);
        } else {
            Instantiate(newObject, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
