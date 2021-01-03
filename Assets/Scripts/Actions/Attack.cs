using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Actions {

    public class Attack : MonoBehaviour, IAction {
        public int damage;
        public int force;
        
        public void execute(PlayerMovement playerMov) {
            var animalsToAttack = new List<GameObject>(Physics
                .OverlapSphere(transform.position + transform.rotation * Vector3.forward * 2, 2)
                .Select(hit => hit.gameObject)
                .Where(obj => obj.CompareTag("Animal")));
            foreach (var animal in animalsToAttack) {
                animal.GetComponent<Animal>().takeDamage(damage, transform.position, force);
            }
        }
    }
}
