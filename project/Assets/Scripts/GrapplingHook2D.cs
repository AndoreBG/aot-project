using UnityEngine;

public class GrapplingHook2D : MonoBehaviour
{
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private float maxDistance = 15f;

    private DistanceJoint2D joint;
    private LineRenderer line;
    private Rigidbody2D rb;

    private Vector2 grapplePoint;

    void Start()
    {
        joint = GetComponent<DistanceJoint2D>();
        line = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();

        joint.enabled = false;
        line.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ShootGrapple();
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }

        if (joint.enabled)
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, grapplePoint);
        }
    }

    void ShootGrapple()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePos - (Vector2)transform.position;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, maxDistance, grappleLayer);

        if (hit.collider != null)
        {
            grapplePoint = hit.point;

            joint.enabled = true;
            joint.connectedAnchor = grapplePoint;
            joint.distance = Vector2.Distance(transform.position, grapplePoint);

            line.enabled = true;
            line.positionCount = 2;
        }
    }

    void StopGrapple()
    {
        joint.enabled = false;
        line.enabled = false;
    }
}