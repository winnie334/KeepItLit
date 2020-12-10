using System.Linq;
using UnityEngine;

namespace Actions {
    public class Food : MonoBehaviour, IAction {
        public float healthFill;
        
        public void execute(PlayerMovement playerMovement) {
            playerMovement.Heal(healthFill);
            playerMovement.removeObject(gameObject);
            Destroy(gameObject);
        }
    }
}
