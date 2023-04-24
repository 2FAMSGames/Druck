using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AcelerometroManager : MonoBehaviour
{
    private Rigidbody rb;

    private float Speed = 30f;
    private float friction = 0f;
    private float maxHeight = 1f;
    private float lastCollision = 0f;    
    
    public GameObject ground;
    [SerializeField] private AudioClip clip;
    
    private bool soundOn = false; 

    // Start is called before the first frame update
    void Start()
    {
        rb=GetComponent<Rigidbody>();
        rb.drag = friction;
        GameState.Instance.audioSource.clip = clip;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 tilt = Input.acceleration;
        tilt = Quaternion.Euler(90, 0, 0) * tilt;
        rb.AddForce(tilt * Speed);

        float actualHeight = transform.position.y;

        if (actualHeight > maxHeight)
        {
            actualHeight = maxHeight;
        }
        transform.position = new Vector3(transform.position.x, actualHeight, transform.position.z);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (!GameState.Instance.audioSource.isPlaying && !soundOn && collision.gameObject != ground)
        {
            lastCollision = Time.time - lastCollision;
            
            if (lastCollision > 1f)
            {
                GameState.Instance.audioSource.Play();
                soundOn = true;
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (!GameState.Instance.audioSource.isPlaying && soundOn && collision.gameObject != ground)
        {
            soundOn = false;
        }
    }
}
