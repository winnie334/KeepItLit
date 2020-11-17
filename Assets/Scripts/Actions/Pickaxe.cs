using UnityEngine;

namespace Actions {
    public class Pickaxe : MonoBehaviour, IAction {
        public void execute() {
            var rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.AddForce(new Vector3(0, 1000, 0));
        }
    }
}