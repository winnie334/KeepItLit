using System.Linq;
using UnityEngine;

namespace Actions {
    public class Fish : MonoBehaviour, IAction {

        public GameObject FishObject;
        public int durability = 3;

        public void execute(PlayerMovement playerMovement) {
            if (playerMovement.transform.position.y < 0) {
                var pos = transform.position;
                Instantiate(FishObject, new Vector3(pos.x, pos.y + 2, pos.z), Quaternion.identity);
                durability--;
                if (durability == 0) {
                    playerMovement.releaseObjects();
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
