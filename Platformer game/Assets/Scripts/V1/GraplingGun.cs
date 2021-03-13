using UnityEngine;
using UnityEngine.UI;

public class GraplingGun : MonoBehaviour
{
    [Space]
    [Header("Components Required")]
    public SpringJoint2D springJoint;
    public LineRenderer lineRenderer;
    public CircleCollider2D circleCollider;

    [Space]
    [Header("Data")]
    public float maxDistance;

    [Space]
    [Header("Behaviour")]
    public float targetDistance;
    public float targetFrequency;

    private GameObject grapplePoint;
    private Vector2 grappleDistanceVector;

    private RaycastHit2D hit;
    private bool grappleActive = false;

    private void Awake()
    {
        circleCollider.radius = maxDistance;
    }

    private void Start()
    {
        springJoint.enabled = false;
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        if (grappleActive)
            lineRenderer.SetPosition(0 , transform.position);
    }

    public bool Grapple()
    {
        if (grapplePoint) 
        {
            springJoint.autoConfigureDistance = false;

            springJoint.distance = targetDistance;
            springJoint.frequency = targetFrequency;
            springJoint.connectedAnchor = grapplePoint.transform.position;
            springJoint.enabled = true;

            lineRenderer.SetPosition(0 , transform.position);
            lineRenderer.SetPosition(1 , grapplePoint.transform.position);
            lineRenderer.enabled = true;

            grappleActive = true;

            return grappleActive;
        }

        return false;
    }

    public bool StopGrapple() 
    {
        lineRenderer.enabled = false;
        springJoint.enabled = false;
        grappleActive = false;

        return true;
    }

    public float GetDistanceToGrapplePoint() 
    {
        return ((Vector2)grapplePoint.transform.position - (Vector2)transform.position).magnitude;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!grapplePoint || grapplePoint == collision.gameObject)
            grapplePoint = collision.gameObject;
        else
        {
            float currentDistance = (grapplePoint.transform.position - transform.position).magnitude;
            float newDistance = (collision.transform.position - transform.position).magnitude;

            if (newDistance < currentDistance)
            {
                grapplePoint.GetComponent<GrapplePoint>().ChangeActiveState(false);
                grapplePoint = collision.gameObject;
            }
        }

        grapplePoint.GetComponent<GrapplePoint>().ChangeActiveState(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (grapplePoint == collision.gameObject) 
        {
            grapplePoint.GetComponent<GrapplePoint>().ChangeActiveState(false);
            grapplePoint = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position , maxDistance);
    }
}
