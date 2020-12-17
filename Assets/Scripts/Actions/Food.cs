using System.Linq;
using UnityEngine;

namespace Actions {
    public class Food : MonoBehaviour, IAction {
        public float healthFill;
        
        public void execute(PlayerMovement playerMov) {
            playerMov.Heal(healthFill);
            playerMov.removeObject(gameObject);
            Destroy(gameObject);
        }
    }
}
