using System.Linq;
using UnityEngine;

namespace Actions {
    public class Food : MonoBehaviour, IAction {
        public float healthFill;
        public GameObject cookedVersion;
        
        public void execute(PlayerMovement playerMovement) {
            playerMovement.Heal(healthFill);
            playerMovement.removeObject(gameObject);
            Destroy(gameObject);
        }

        public void cookFood() {
            if (cookedVersion == null) return;
            if (transform.parent) { // This food is being carried
                var player = transform.parent.GetComponent<PlayerMovement>();
                player.removeObject(gameObject);
                var cookedFood = Instantiate(cookedVersion, transform.position, Quaternion.identity);
                player.grabObject(cookedFood);
            } else {
                Instantiate(cookedVersion, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}
