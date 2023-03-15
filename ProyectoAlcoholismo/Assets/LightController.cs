using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public Light lightToMove;

    void Update()
    {
        Vector3 newPosition = new Vector3(transform.position.x, lightToMove.transform.position.y, transform.position.z);
        lightToMove.transform.position = newPosition;
    }
}
