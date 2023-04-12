using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pruebas : MonoBehaviour
{
    public float speed = 10f; // Velocidad de movimiento del objeto
    public float friction = 5f; // Fuerza de rozamiento del objeto
    public AudioSource audioSource;

    private Rigidbody rb;

    void Start()
    {
        // Obtener el componente Rigidbody del objeto
        rb = GetComponent<Rigidbody>();
        // Configurar la fricción del Rigidbody
        rb.drag = friction;
    }

    void FixedUpdate()
    {
        // Movimiento horizontal del objeto
        float horizontal = Input.GetAxis("Horizontal");
        rb.AddForce(Vector3.right * horizontal * speed, ForceMode.Force);

        // Movimiento vertical del objeto
        float vertical = Input.GetAxis("Vertical");
        rb.AddForce(Vector3.forward * vertical * speed, ForceMode.Force);
    }
    void OnCollisionEnter(Collision collision)
    {
        audioSource.Play();
    }

}
