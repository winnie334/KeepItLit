using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Actions {

    public class Attack : MonoBehaviour, IAction {
        public float attackRange = 4;
        public int damage;
        public int force;

        public void execute(PlayerMovement playerMov) {
            var animalsToAttack = new List<GameObject>(Physics
                .OverlapSphere(playerMov.transform.position + playerMov.transform.rotation * Vector3.forward, attackRange)
                .Select(hit => hit.gameObject)
                .Where(obj => obj.CompareTag("Animal")));
            foreach (var animal in animalsToAttack) {
                animal.GetComponent<Animal>().takeDamage(damage, transform.position, force);
            }
        }
    }
    
}
