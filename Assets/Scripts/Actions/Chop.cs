using UnityEngine;

namespace Actions {
    public class Chop : MonoBehaviour, IAction {

        public int damage;
        public AudioSource audioSource;

        public void execute(PlayerMovement playerMovement) {
            //TODO pick closest tree, or actually come up with a good way of detecting trees
            var playerTransform = playerMovement.gameObject.transform;
            var hitCollider = Physics.OverlapSphere(playerTransform.position + playerTransform.rotation * Vector3.forward * 2, 2)[0];
            if (!hitCollider.gameObject.CompareTag("Resource")) return;
            hitCollider.gameObject.GetComponent<Loot>().Extract(gameObject.GetComponent<ItemAssociation>().item, damage);
            //audioSource.Play();
        }
    }
}
