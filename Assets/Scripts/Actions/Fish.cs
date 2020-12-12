using System;
using System.Linq;
using UnityEngine;

namespace Actions {
    public class Fish : MonoBehaviour, IAction {

        public GameObject FishObject;
        public GameObject Bobber;
        
        public int durability = 3;
        private GameObject currBobber;
        
        public void execute(PlayerMovement playerMovement) {
            if (currBobber) {
                if (currBobber.GetComponent<Bobber>().hasFish()) {
                    var boberPos = currBobber.transform.position;
                    var fish = Instantiate(FishObject, boberPos, Quaternion.identity);
                    var targetDir = (transform.position - boberPos).normalized;
                    fish.GetComponent<Rigidbody>().AddForce(new Vector3(targetDir.x * 1000, targetDir.y*2000, targetDir.z * 1000));
                    durability--;
                    if (durability == 0) {
                        playerMovement.releaseObjects();
                        Destroy(gameObject);
                    }
                    Destroy(currBobber);
                }
                else {
                    Destroy(currBobber);
                    throwBobber(playerMovement.transform.forward);
                }
            }
            else throwBobber(playerMovement.transform.forward);
        }

        void throwBobber(Vector3 dir) {
            var pos = transform.position;
            currBobber = Instantiate(Bobber, new Vector3(pos.x, pos.y + 0.2f, pos.z), Quaternion.identity);
            currBobber.GetComponent<Rigidbody>().AddForce(dir*1000);
        }
    }
}
