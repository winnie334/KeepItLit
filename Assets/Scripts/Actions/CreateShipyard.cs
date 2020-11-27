using System.Collections;
using System.Collections.Generic;
using Actions;
using UnityEngine;

public class CreateShipyard : MonoBehaviour, IAction
{
    public GameObject shipyard;

    public void execute(PlayerMovement player)
    {
        var position = transform.position;
        var targetLoc = new Vector3(position.x + 1, position.y - 1.5f, position.z + 1);
        if (targetLoc.y > 0) return;

        Instantiate(shipyard, targetLoc, Quaternion.identity);
        player.removeObject(this.gameObject);
        Destroy(this.gameObject);
    }
}