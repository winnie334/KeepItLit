using UnityEngine;

namespace Actions {
    public class Chop : MonoBehaviour, IAction {
        
        public AudioSource audioSource;
        
        public void execute(PlayerMovement playerMovement) {
            //TODO pick closest tree, or actually come up with a good way of detecting trees
            var hitCollider = Physics.OverlapSphere(transform.position + transform.rotation * Vector3.forward * 2, 2)[0];
            if (!hitCollider.gameObject.CompareTag("Tree")) return;
            Destroy(hitCollider.gameObject);
            audioSource.Play();
        }
    }
}
