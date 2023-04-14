using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingshotController : MonoBehaviour
{
    [SerializeField]
    GameObject projectionPlaneObject;
    Plane projectionPlane;

    [SerializeField]
    GameObject projectilePrefab;
    GameObject projectile;
    Rigidbody projectileRigidbody;

    [SerializeField]
    [Tooltip("Position where projectile will be instantiated and collider used for click detection")]
    GameObject projectilePlacer;
    SphereCollider projectilePlacerCollider;

    [SerializeField]
    GameObject arrowPrefab;
    GameObject arrow;
    SpriteRenderer arrowSprRend;
    float arrowAspectRatio;

    [SerializeField]
    float shotForceMultiplier = 200;
    [SerializeField]
    float shotForceOffset = 200;


    bool clicked = false;

    Vector3 mousePos;
    float shotAngle;
    float mousePlacerDistance;


    // Start is called before the first frame update
    void Start()
    {
        projectionPlane = new Plane(projectionPlaneObject.transform.up, projectionPlaneObject.transform.position);
        projectilePlacerCollider = projectilePlacer.GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckOnClickEnter();

        CheckOnClick();

        CheckOnClickExit();
    }

    private void CheckOnClickEnter()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = GetClickPositionOnPlane().GetValueOrDefault();

            //Only detects if mouse position is inside placer bounding sphere
            if (projectilePlacerCollider.bounds.Contains(mousePos))
            {
                arrow = Instantiate(arrowPrefab, projectilePlacer.transform.position, Quaternion.FromToRotation(Vector3.up, transform.forward), gameObject.transform);
                arrowSprRend = arrow.GetComponent<SpriteRenderer>();
                arrowAspectRatio = arrowSprRend.size.x / arrowSprRend.size.y;
                clicked = true;
            }
        }
    }

    private void CheckOnClick()
    {
        if (clicked)
        {
            mousePos = GetClickPositionOnPlane().GetValueOrDefault();

            shotAngle = GetRotationAngle();
            arrow.transform.localRotation = Quaternion.Euler(90, shotAngle, 0);

            mousePlacerDistance = Vector3.Distance(mousePos, projectilePlacer.transform.position);

            float arrowHeight = mousePlacerDistance * 2;
            arrowSprRend.size = new Vector2(arrowHeight * arrowAspectRatio, arrowHeight);
        }
    }

    private void CheckOnClickExit()
    {
        if (Input.GetMouseButtonUp(0) && clicked)
        {
            ShotProjectile();
            Destroy(arrow);
            clicked = false;
        }
    }

    

    private Vector3? GetClickPositionOnPlane()
    {
        Ray cameraMouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        float rayDistance;
        if (projectionPlane.Raycast(cameraMouseRay, out rayDistance))
        {
            return cameraMouseRay.GetPoint(rayDistance);
        }

        return null;
    }

    private float GetRotationAngle()
    {
        float angle = Vector3.Angle(new Vector3(0, 0, -1), transform.InverseTransformPoint(mousePos));
        Vector3 cross = Vector3.Cross(new Vector3(0, 0, -1), transform.InverseTransformPoint(mousePos));
        if (cross.y < 0) angle = -angle;

        return angle;
    }

    public void DeleteAndCreateProjectile()
    {
        Destroy(projectile);
        projectile = Instantiate(projectilePrefab, projectilePlacer.transform.position, transform.rotation, gameObject.transform);
        projectileRigidbody = projectile.GetComponent<Rigidbody>();
        projectileRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        projectile.GetComponent<ProjectileController>().slingshotController = this;
    }

    private void ShotProjectile()
    {
        projectileRigidbody.constraints = RigidbodyConstraints.None;

        float shotForce = (shotForceMultiplier * mousePlacerDistance) + shotForceOffset;
        Vector3 forceVector = new Vector3(0, 0, shotForce);
        Quaternion forceDirection = Quaternion.Euler(0, shotAngle, 0);

        projectileRigidbody.AddRelativeForce(forceDirection * forceVector);

        //StartCoroutine(ResetProjectileOnFail());
    }

    //Wait times serializable?
    //TODO: Make one public funtion --->  IEnumerator WaitAndResetProjectile(time)
    private IEnumerator ResetProjectileOnFail()
    {
        yield return new WaitForSeconds(5f);
        DeleteAndCreateProjectile();
    }

    public IEnumerator ResetProjectileOnHit()
    {
        yield return new WaitForSeconds(0.5f);
        DeleteAndCreateProjectile();
    }

    public IEnumerator ResetProjectileOnFloor()
    {
        yield return new WaitForSeconds(2f);
        DeleteAndCreateProjectile();
    }

}




/*
if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
{
    touchPosWords = Input.GetTouch(0).position;
}
*/