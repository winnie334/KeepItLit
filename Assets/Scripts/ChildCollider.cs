using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCollider : MonoBehaviour {
    public Fire parent;
    public string type;

    private void OnTriggerStay(Collider other) {
        parent.OnChildTriggerStay(type, other);
    }
}
