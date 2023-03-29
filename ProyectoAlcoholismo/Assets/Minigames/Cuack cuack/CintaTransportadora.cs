using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CintaTransportadora : MonoBehaviour
{
    public Vector3 velocidad;
    public bool mov = false;

    private void FixedUpdate()
    {
        if (mov)
        {
            foreach (Transform child in transform)
            {
                child.GetComponent<Rigidbody>().MovePosition(child.position + velocidad);
                if (child.position.x < -15)
                {
                    child.GetComponent<Rigidbody>().MovePosition(new Vector3(10, child.position.y, child.position.z));
                }
            }
        }
    }
}
