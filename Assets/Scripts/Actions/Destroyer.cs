using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Actions
{
    public class Destroyer : MonoBehaviour, IAction
    {
        public void execute(PlayerMovement playerMov)
        {
            Transform t;
            var animalsToAttack = new List<GameObject>(Physics
                .OverlapSphere((t = transform).position + t.rotation * Vector3.forward * 2, 2)
                .Select(hit => hit.gameObject)
                .Where(obj => obj.CompareTag("Animal")));
            foreach (var animal in animalsToAttack) {
                animal.GetComponent<Rigidbody>().AddForce(new Vector3(1000, 1000, 0));
            }
        }
    }
}
