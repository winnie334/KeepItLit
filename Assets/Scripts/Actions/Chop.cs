using UnityEngine;

namespace Actions
{
    public class Chop : MonoBehaviour, IAction
    {
        public void execute()
        {
            //TODO we should check if we face the tree or not, and look at multiple collision instead of only the first one...
            Collider hitCollider = Physics.OverlapSphere(transform.position, 4)[0];
            if (hitCollider.gameObject.CompareTag("Tree")) Destroy(hitCollider.gameObject);
        }
    }
}
