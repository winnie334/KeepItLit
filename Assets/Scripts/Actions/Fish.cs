using System;
using System.Linq;
using UnityEngine;

namespace Actions {
    public class Fish : MonoBehaviour, IAction {

        public Animator anim;
        public GameObject FishObject;
        public GameObject Bobber;
        public Transform top;

        public AudioClip fishCatchSound;
        public AudioClip throwBobberSound;
        public AudioClip cancelFishingSound;

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

        public void execute(PlayerMovement playerMov) {
            if (currBobber && anim.GetBool("IsFishing")) {
                anim.SetBool("IsFishing", false);
                if (currBobber.GetComponent<Bobber>().hasFish()) {
                    Debug.Log("has fish");
                    var bobberPos = currBobber.transform.position;
                    var fish = Instantiate(FishObject, bobberPos, Quaternion.identity);
                    Debug.Log("Fish instanciated");
                    var targetDir = (transform.position - bobberPos).normalized;
                    fish.GetComponent<Rigidbody>().AddForce(new Vector3(targetDir.x * 1000, targetDir.y * 2000, targetDir.z * 1000));
                    durability--;
                    if (durability == 0) {
                        playerMov.releaseObjects();
                        Destroy(gameObject);
                        Hints.displayHint("Shoot, my fishing rod broke");
                    }
                    Destroy(currBobber);
                    playerMov.playSound(fishCatchSound);
                } else Destroy(currBobber);
                playerMov.playSound(cancelFishingSound);
                lr.enabled = false;
            } else if (!anim.GetBool("IsFishing")) {
                anim.SetBool("IsFishing", true);
                throwBobber(playerMov.transform.forward);
                playerMov.playSound(throwBobberSound);
                lr.enabled = true;
            }
        }

        void throwBobber(Vector3 dir) {
            currBobber = Instantiate(Bobber, top.position, Quaternion.identity);
            currBobber.GetComponent<Rigidbody>().AddForce(dir * 1000);
        }
    }
}
