using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /*Private Variables*/
    private Rigidbody2D rb;
    private BetterJump betterJump;
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
    public bool grappleJump;

    [Space]
    [Header("Visualization")]
    public Vector2 rightOffset;
    public Vector2 leftOffset;
    public Vector2 bottomOffset;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        betterJump = GetComponent<BetterJump>();
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

        if (!grounded && grappleRequested && !grappled) 
        {
            grappled = gun.Grapple();
            canMove = !grappled;
        }

        if (grappled && jumpRequested) 
        {
            grappled = !gun.StopGrapple();
            canMove = !grappled;
            grappleJump = true;
            Jump(Vector2.up);
        }

        if (grappled && grappleRelease) 
        {
            grappled = !gun.StopGrapple();
            canMove = !grappled;
        }

        if (jumpRequested && grounded)
            Jump(Vector2.up);

        if (grounded) 
        {
            grappleJump = false;
            wallJumped = false;
        }
    }

    /// <summary>
    /// Detects the input from the player controller, gets called automatically at the beginning of the update function
    /// </summary>
    private void CheckInput() 
    {
        horMovement = Input.GetAxis("Horizontal");
        verMovement = Input.GetAxis("Vertical");
        jumpRequested = Input.GetButtonDown("Jump");
        grappleRequested = Input.GetButtonDown("Fire1");
        grappleRelease = Input.GetButtonUp("Fire1");
    }

    /// <summary>
    /// Checks collitions, 
    /// </summary>
    private void CheckCollisions() 
    {
        grounded = Physics2D.OverlapBox((Vector2)transform.position + bottomOffset , bottomDetection , 0.0f , detectionMask);

        airborne = !grounded;

        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset , detectionRadius , detectionMask);
        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset , detectionRadius , detectionMask);

        onWall = onLeftWall || onRightWall;
    }

    /// <summary>
    /// Handles the horizontal movement of the player
    /// </summary>
    /// <param name="direction"> a float detailing the direction where the player is moving (-1 left / 1 right) </param>
    private void Move(float direction)
    {
        if (!canMove) return;

        if (!wallJumped && !grappleJump)
            rb.velocity = new Vector2(direction * speed , rb.velocity.y);
        else if (wallJumped)
            rb.velocity = Vector2.Lerp(rb.velocity , (new Vector2(direction * speed , rb.velocity.y)) , wallJumpLerp * Time.deltaTime);
        else if (grappleJump)
            rb.velocity += new Vector2(direction * speed * Time.deltaTime , 0);
    }

    /// <summary>
    /// Handles the vertical movement of the player while jumping
    /// </summary>
    /// <param name="dir"> A vector2 detailing the general direction of the player's movement </param>
    private void Jump(Vector2 dir)
    {
        rb.velocity = new Vector2(rb.velocity.x , 0);
        rb.velocity += dir * jumpForce;
    }

    /// <summary>
    /// Dissables and enables the input of the player after x seconds
    /// </summary>
    /// <param name="time"> the time in seconds the function will wait bf enabling the input to be interpreted again</param>
    /// <returns></returns>
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