using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
	public PlayerData Data;

    #region Variables

    //movement
    [Header("Movement")]
    public Vector2 moveInput;
    public Vector2 speed;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Transform rightWallCheckPoint;
    [SerializeField] private Transform leftWallCheckPoint;
    [SerializeField] private Vector2 groundCheckPointSize;
    [SerializeField] private Vector2 wallCheckPointSize;
    public bool isRunning = false;
    public bool canMove = true;
    [Space(10)]

    //physics & Layers
    [Header("Physics & Layers")]
    private Rigidbody2D rb;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private BoxCollider2D col;
    [SerializeField] private BoxCollider2D crouchCol;
    [HideInInspector] public bool jumped = false;
    [HideInInspector] public bool landed = false;
    public bool isCrouching = false;

    //gravity
    [Header("Gravity")]
    public bool isGrounded = false;
    [SerializeField] private bool isRightWalled = false;
    [SerializeField] private bool isLeftWalled = false;
    public int jumps = 1;
    [Space(10)]

    //dash
    [Header("Dash")]
    public bool canDash = true;
    public bool isDashing = false;
    public int dashes = 1;

    //wall sliding
    private bool isWallSliding = false;

    //timers
    private float lastOnGroundTime = 0;
    private float lastPressedJumpTime = 0;
    [HideInInspector] public float DashCoolDownTime = 0;
    [HideInInspector] public float DashTime = 0;

    #endregion

    private void Start()
    {
        //setting up veriables
        rb = GetComponent<Rigidbody2D>();

        //setup
        StartCoroutine(SetGravityScale(Data.gravityScale, 0));

        crouchCol.enabled = false;

        Physics2D.IgnoreLayerCollision(gameObject.layer, gameObject.layer);
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        speed = rb.velocity;

        #region Input

        //getting the input from the player
        float moveInputX = Input.GetAxisRaw("Horizontal");
        float moveInputY = Input.GetAxisRaw("Vertical");

        moveInput.x = moveInputX;
        moveInput.y = moveInputY;

        #endregion

        #region Timers

        lastOnGroundTime -= Time.deltaTime;
        lastPressedJumpTime -= Time.deltaTime;

        #endregion

        #region Collision Checks

        //check if he is grounded
        if (Physics2D.OverlapBox(groundCheckPoint.position, groundCheckPointSize, 0, groundLayer) && rb.velocity.y <= 0 && !isDashing)
        {
            //if so, set the lastGrounded to coyoteTime
            lastOnGroundTime = Data.GroundCoyoteTime;

            jumps = Data.jumps;
            dashes = Data.dashes;

            if (!isGrounded)
            {
                isGrounded = true;

                landed = true;
            }
        }
        else
        {
            if (isGrounded)
            {
                isGrounded = false;
            }
        }

        if (Physics2D.OverlapBox(rightWallCheckPoint.position, wallCheckPointSize, 0, wallLayer))
        {
            if (!isRightWalled)
            {
                isRightWalled = true;
            }
        }
        else
        {
            if (isRightWalled)
            {
                isRightWalled = false;
            }
        }

        if (Physics2D.OverlapBox(leftWallCheckPoint.position, wallCheckPointSize, 0, wallLayer))
        {
            if (!isLeftWalled)
            {
                isLeftWalled = true;
            }
        }
        else
        {
            if (isLeftWalled)
            {
                isLeftWalled = false;
            }
        }

        #endregion

        #region Jump

        //if the player presses the jump key during the buffertime
        if (lastPressedJumpTime > 0 && jumps > 0)
        {
            if (isGrounded)
            {
                //make the player jump
                Jump();

                lastPressedJumpTime = 0;
            }
            else
            {
                if (lastOnGroundTime > 0)
                {
                    //make the player jump
                    Jump();

                    lastPressedJumpTime = 0;

                    lastOnGroundTime = 0;
                }
            }

            jumps--;
        }

        //gravity mult
        if (!isDashing)
        {
            if (Mathf.Abs(rb.velocity.y) < Data.jumpGravityThreshHold)
            {
                StartCoroutine(SetGravityScale(Data.gravityScale * Data.jumpGravityMult, 0));
            }
            else
            {
                if (rb.gravityScale != Data.gravityScale && !isDashing)
                {
                    StartCoroutine(SetGravityScale(Data.gravityScale, 0));
                }
            }
        }

        #endregion

        #region Actions

        //jump
        if (Input.GetKeyDown(Data.jumpKey))
        {
            lastPressedJumpTime = Data.jumpCoyoteTime;
        }

        //dash
        if (Input.GetKeyDown(Data.dashKey) && canDash && dashes > 0)
        {
            DashTime = Data.dashTime;
            DashCoolDownTime = Data.dashTime + Data.dashCooldown;

            isDashing = true;
            canDash = false;
            canMove = false;

            dashes--;

            StartCoroutine(SetGravityScale(0, Data.dashDelay));
            StartCoroutine(SetPlayerVelocity(Vector2.zero, Data.dashDelay));

            Vector2 input = moveInput.normalized;

            StartCoroutine(Dash(input));
        }

        //crouch
        if (moveInputY < 0 && isGrounded)
        {
            if (!isCrouching)
            {
                isCrouching = true;
            }
        }
        else
        {
            if (isCrouching)
            {
                isCrouching = false;
            }
        }

        //wall sliding
        if ((isLeftWalled || isRightWalled) && !isGrounded && moveInput.x != 0)
        {
            isWallSliding = true;
        }
        else
        {
            if (isWallSliding)
            {
                isWallSliding = false;
            }
        }

        #endregion

        #region Flip

        if (!GetComponent<SpriteRenderer>().flipX && moveInputX < 0 && !isDashing)
        {
            Flip();
        }
        else
        {
            if (GetComponent<SpriteRenderer>().flipX && moveInputX > 0 && !isDashing)
            {
                Flip();
            }
        }

        #endregion

        #region Crouch

        if (isCrouching)
        {
            if (canMove || !crouchCol.enabled || col.enabled)
            {
                canMove = false;
                crouchCol.enabled = true;
                col.enabled = false;
            }
        }
        else
        {
            if (!canMove || crouchCol.enabled || !col.enabled)
            {
                canMove = true;
                crouchCol.enabled = false;
                col.enabled = true;
            }
        }

        #endregion

        #region Wall Sliding

        if (isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -Data.wallSlideSpeed, float.MaxValue));
        }

        #endregion

        #region Wall Jumping

        if (isWallSliding && Input.GetKeyDown(Data.jumpKey) && !isDashing)
        {
            WallJump();
        }

        #endregion

        #region Clamping

        Mathf.Clamp(rb.velocity.y, -Data.maxFallSpeed, float.MaxValue);

        #endregion
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
        {
            return;
        }

        #region Timers

        DashCoolDownTime -= Time.fixedDeltaTime;
        DashTime -= Time.fixedDeltaTime;

        #endregion

        #region Dash

        if (DashCoolDownTime < 0 && !canDash)
        {
            canDash = true;
        }

        #endregion

        #region Run

        if (!isDashing && canMove)
        {
            float targetSpeed = moveInput.x * Data.moveSpeed;

            //calculating the amount of speed the player needs to get to the desired speed
            float speedDif = targetSpeed - rb.velocity.x;

            #region Acceleration

            float accelRate;

            if (isGrounded)
            {
                accelRate = (moveInput.x != 0 && Mathf.Sign(moveInput.x) == Mathf.Sign(rb.velocity.x)) ? Data.acceleration : Data.deceleration;
            }
            else
            {
                accelRate = (moveInput.x != 0 && Mathf.Sign(moveInput.x) == Mathf.Sign(rb.velocity.x)) ? Data.airAcceleration : Data.airDeceleration;
            }

            #endregion

            //increasing the velocity over time
            float movement = speedDif * accelRate * Data.powerMult;

            //adding the force to the player
            rb.AddForce(movement * Vector2.right, ForceMode2D.Force);

            if (!isRunning && moveInput.x != 0)
            {
                isRunning = true;
            }
            else if (moveInput.x == 0 && isRunning)
            {
                isRunning = false;
            }
        }
        else if (isRunning)
        {
            isRunning = false;
        }

        #endregion
    }

    #region Important Mechanics

    private IEnumerator Dash(Vector2 input)
    {
        yield return new WaitForSecondsRealtime(Data.dashDelay);

        //if the player is not giving any inputs
        if (input == Vector2.zero)
        {
            input = new Vector2(((GetComponent<SpriteRenderer>().flipX) ? -1 : 1), 0);
        }

        //if the player in pressing down and left or right while being grounded
        if (input.y < 0 && input.x != 0 && isGrounded)
        {
            input = new Vector2(Mathf.Sign(input.x), 0);
        }

        rb.AddForce(input * Data.dashForce, ForceMode2D.Impulse);

        yield return new WaitForSecondsRealtime(Data.dashTime);

        isDashing = false;

        StartCoroutine(SetPlayerVelocity(Vector2.zero, Data.dashDelay));
    }

    public void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);

        //apply a force up to the player
        rb.AddForce(Vector2.up * Data.jumpForce, ForceMode2D.Impulse);

        //add animation
        jumped = true;
    }

    private void WallJump()
    {
        rb.velocity = Vector2.zero;

        int xValue = GetComponent<SpriteRenderer>().flipX ? 1 : -1;

        Vector2 dir = new Vector2(xValue, 1).normalized;

        rb.AddForce(dir * Data.wallJumpForce, ForceMode2D.Impulse);

        //add animation
        jumped = true;
    }

    #endregion

    #region Minor Functions

    private IEnumerator SetGravityScale(float gravityScale, float time)
    {
        float currentGravityScale = rb.gravityScale;

        float t = 0;

        while (t < time)
        {
            t += Time.deltaTime;
            float percent = Mathf.Clamp01(t / time);
            percent = Mathf.SmoothStep(0, 1, percent);

            rb.gravityScale = Mathf.Lerp(currentGravityScale, gravityScale, percent);

            yield return null;
        }

        rb.gravityScale = gravityScale;
    }

    private IEnumerator SetPlayerVelocity(Vector2 velocity, float time)
    {
        Vector2 currentVelocity = rb.velocity;

        float t = 0;

        while (t < time)
        {
            t += Time.deltaTime;
            float percent = Mathf.Clamp01(t / time);
            percent = Mathf.SmoothStep(0, 1, percent);

            rb.velocity = Vector2.Lerp(currentVelocity, velocity, percent);

            yield return null;
        }

        rb.velocity = velocity;
    }

    private void Flip()
    {
        if (!isDashing)
        {
            GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
        }
    }

    public void SoulCoreForce(float force)
    {
        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    #endregion
}