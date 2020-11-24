using System.Linq;
using UnityEngine;

namespace Actions {
    public class Food : MonoBehaviour, IAction {
        public float healthFill;
        public void execute(PlayerMovement playerMovement) {
            gameObject.transform.parent.gameObject.GetComponent<PlayerMovement>().Heal(healthFill);
            playerMovement.removeObject(this.gameObject);
            Destroy(gameObject);
        }
    }
}
