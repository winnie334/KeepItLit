using System.Linq;
using UnityEngine;

namespace Actions {
    public class Chop : MonoBehaviour, IAction {

        public int damage;

        public void execute(PlayerMovement playerMovement) {
            //TODO pick closest tree, or actually come up with a good way of detecting trees
            var playerTransform = playerMovement.gameObject.transform;
            var resources = Physics.OverlapSphere(playerTransform.position + playerTransform.rotation * Vector3.forward * 2, 2)
                            .Where(hit => hit.gameObject.CompareTag("Resource"));
            foreach (var resource in resources) {
                resource.gameObject.GetComponent<Loot>().Extract(gameObject.GetComponent<ItemAssociation>().item, damage);
            }
        }
    }
}
