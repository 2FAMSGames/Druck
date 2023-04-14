using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupController : MonoBehaviour
{
    CupsController parentController;
    // Start is called before the first frame update
    void Start()
    {
        parentController = gameObject.GetComponentInParent<CupsController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            parentController.OnCupEntered();
        }
    }
}
