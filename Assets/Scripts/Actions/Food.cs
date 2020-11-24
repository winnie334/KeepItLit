using System.Linq;
using UnityEngine;

namespace Actions {
    public class Food : MonoBehaviour, IAction {
        public void execute() {
            var item = gameObject.GetComponent<ItemAssociation>();
            gameObject.transform.parent.gameObject.GetComponent<PlayerMovement>().Heal(item.item.healthFill);
            Destroy(gameObject);
        }
    }
}
