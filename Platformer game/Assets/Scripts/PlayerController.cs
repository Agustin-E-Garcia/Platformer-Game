using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{
    /*Private Variables*/
    private Rigidbody2D rb;
    [SerializeField] private GraplingGun gun;

    /*Input variables*/
    private float horMovement;
    private float verMovement;
    private bool jumpRequested;
    private bool grappleRequested;
    private bool grappleRelease;

    /*Public Variables*/
    [Space]
    [Header("Stats")]
    public float speed;
    public float jumpForce;
    public float wallSlideSpeed;
    public float wallJumpLerp;

    [Space]
    [Header("Collision Values")]
    public float detectionRadius;
    public Vector2 bottomDetection;
    public LayerMask detectionMask;

    [Space]
    [Header("States")]
    public bool canMove;
    public bool grounded;
    public bool airborne;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public bool wallJumped;
    public bool grappled;

    [Space]
    [Header("Visualization")]
    public Vector2 rightOffset;
    public Vector2 leftOffset;
    public Vector2 bottomOffset;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gun.Joint = GetComponent<SpringJoint2D>();
    }

    private void Start()
    {
        canMove = true;
    }

    private void Update()
    {
        CheckInput();
        CheckCollisions();

        Move(horMovement);

        if (onWall && !grounded && canMove) 
        {
            if (horMovement > 0.2f && onRightWall || horMovement < 0.2f && onLeftWall)
            {
                rb.velocity = new Vector2(0 , -wallSlideSpeed);

                if (jumpRequested && onWall)
                {
                    StopCoroutine(DisableMovement(0));
                    StartCoroutine(DisableMovement(0.1f));

                    Jump(Vector2.up + (onRightWall ? Vector2.left : Vector2.right));
                    wallJumped = true;
                }
            }
            else
                rb.velocity = new Vector2(rb.velocity.x , rb.velocity.y);
        }

        if (jumpRequested && grounded)
            Jump(Vector2.up);

        if (jumpRequested && grappled) 
        {
            StopGrapple();
            Jump(Vector2.up);
        }

        if (airborne && grappleRequested)
        {
            RequestGrapple();
        }

        if (grappled && grappleRelease) 
        {
            StopGrapple();
        }

        if (grounded)
            wallJumped = false;
    }

    private void CheckInput() 
    {
        horMovement = Input.GetAxis("Horizontal");
        verMovement = Input.GetAxis("Vertical");
        jumpRequested = Input.GetButtonDown("Jump");
        grappleRequested = Input.GetButtonDown("Fire1");
        grappleRelease = Input.GetButtonUp("Fire1");
    }

    private void CheckCollisions() 
    {
        grounded = Physics2D.OverlapBox((Vector2)transform.position + bottomOffset , bottomDetection , 0.0f , detectionMask);

        airborne = !grounded;

        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset , detectionRadius , detectionMask);
        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset , detectionRadius , detectionMask);

        onWall = onLeftWall || onRightWall;
    }

    private void Move(float direction)
    {
        if (!canMove) return;

        if (!wallJumped)
            rb.velocity = new Vector2(direction * speed , rb.velocity.y);
        else
            rb.velocity = Vector2.Lerp(rb.velocity , (new Vector2(direction * speed , rb.velocity.y)) , wallJumpLerp * Time.deltaTime);
    }

    private void Jump(Vector2 dir)
    {
        rb.velocity = new Vector2(rb.velocity.x , 0);
        rb.velocity += dir * jumpForce;
    }

    private void RequestGrapple() 
    {
        grappled = gun.StartGrapple(new Vector2(horMovement , verMovement));
        rb.AddForce(new Vector2(horMovement , verMovement) * gun.launchSpeed);
        canMove = !grappled;
    }

    private void StopGrapple() 
    {
        gun.StopGrapple();
        grappled = false;
        canMove = true;
    }

    IEnumerator DisableMovement(float time) 
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + bottomOffset , bottomDetection);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset , detectionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset , detectionRadius);

        Gizmos.DrawLine(transform.position , (Vector2)transform.position + new Vector2(horMovement , verMovement).normalized * 0.5f);
    }
}

