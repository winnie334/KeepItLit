using System.Linq;
using UnityEngine;

namespace Actions
{
    public class Destroyer : MonoBehaviour, IAction
    {
        public void execute(PlayerMovement playerMovement)
        {
            //TODO we should check if we face the tree or not, and look at multiple collision instead of only the first one...
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 4);
            Debug.Log(hitColliders);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.name == "Terrain" || hitCollider.gameObject.name == "Player" || hitCollider.gameObject.name.Contains("Destroyer")) continue;
                Destroy(hitCollider.gameObject);
            }
        }
    }
}
