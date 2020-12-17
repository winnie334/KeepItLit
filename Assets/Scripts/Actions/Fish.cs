using System;
using System.Linq;
using UnityEngine;

namespace Actions {
    public class Fish : MonoBehaviour, IAction {

        public GameObject FishObject;
        public GameObject Bobber;
        public Transform top;
        
        public int durability = 3;
        private GameObject currBobber;
        private LineRenderer lr;


        private void Start() {
            lr = GetComponent<LineRenderer>();
        }

        private void Update() {
            if (currBobber) {
                lr.SetPosition(0, top.position);
                lr.SetPosition(1, currBobber.transform.position);
            }
        }

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
                else Destroy(currBobber);
                lr.enabled = false;
            }
            else {
                throwBobber(playerMovement.transform.forward);
                lr.enabled = true;
            }
        }

        void throwBobber(Vector3 dir) {
            var pos = transform.position;
            currBobber = Instantiate(Bobber, top.position, Quaternion.identity);
            currBobber.GetComponent<Rigidbody>().AddForce(dir*1000);
        }
    }
}
