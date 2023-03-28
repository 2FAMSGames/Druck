using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderManager : MonoBehaviour
{    void OnTriggerEnter(Collider other)
    {
        Debug.Log("¡HAS LLEGADO A LA META!");
    }
}
