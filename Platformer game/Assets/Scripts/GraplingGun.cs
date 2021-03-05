using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;

public class GraplingGun : MonoBehaviour
{
    /*Private variables and Setters/Getters*/
    private LineRenderer lineRenderer;

    private SpringJoint2D joint;
    public SpringJoint2D Joint
    {
        set => joint = value;
        get => joint;
    }


    /*Public variables*/
    [Space]
    [Header("GunStats")]
    public float maxDistance;
    public LayerMask groundMask;
    public float launchSpeed;

    [HideInInspector] public Vector2 grapplePoint;
    [HideInInspector] public Vector2 grappleDistanceVector;
    
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        lineRenderer.enabled = false;
        Joint.enabled = false;
    }

    private void Update()
    {
        if (lineRenderer.enabled)
        {
            lineRenderer.SetPosition(0 , transform.position);
        }
    }

    public bool StartGrapple(Vector2 direction) 
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, maxDistance, groundMask);

        if (hit) 
        {
            grapplePoint = hit.point;
            grappleDistanceVector = hit.point - (Vector2)transform.position;

            lineRenderer.SetPosition(0 , transform.position);
            lineRenderer.SetPosition(1 , hit.point);

            Joint.autoConfigureDistance = false;
            Joint.connectedAnchor = hit.point;

            Joint.distance = grappleDistanceVector.magnitude;
            Joint.frequency = launchSpeed;

            lineRenderer.enabled = true;
            Joint.enabled = true;

            return true;
        }

        return false;
    }

    public void StopGrapple() 
    {
        lineRenderer.enabled = false;
        Joint.enabled = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector2)transform.position , maxDistance);
    }
}
