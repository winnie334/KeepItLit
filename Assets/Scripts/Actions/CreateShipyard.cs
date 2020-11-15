using System.Collections;
using System.Collections.Generic;
using Actions;
using UnityEngine;

public class CreateShipyard : MonoBehaviour, IAction
{
    public GameObject shipyard;

    public void execute()
    {
        var position = transform.position;
        Instantiate(shipyard, new Vector3(position.x + 1, 2, position.z + 1),
            Quaternion.identity);
        Destroy(this.gameObject);
    }
}