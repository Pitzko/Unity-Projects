using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerData Data;

    //movement
    [SerializeField] private Vector3 speed;
    private float horizontalInput;
    private float verticalInput;
    private Vector2 moveInput;

    //physics
    private bool isGrounded;
    private Rigidbody rb;
    private int jumps;

    //timers
    private float isGroundedTimer = 0;
    private float jumpBufferTimer = 0;

    [SerializeField] private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        StartCoroutine(SetGravityScale(Data.gravityScale, 0));
    }

    // Update is called once per frame
    void Update()
    {
        speed = rb.velocity;

        #region Inputs

        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(horizontalInput, verticalInput);

        #endregion

        #region Ground Check

        isGrounded = Physics.Raycast(transform.position, Vector3.down, Data.playerHeight * 0.5f + Data.groundCheckBoost, Data.WhatIsGround);

        if (isGrounded && isGroundedTimer < Data.groundedBufferTime)
        {
            isGroundedTimer = Data.groundedBufferTime;

            if (jumps != Data.jumpsCount && rb.velocity.y < 0)
            {
                jumps = Data.jumpsCount;
            }
        }

        #endregion

        #region Timers

        isGroundedTimer -= Time.deltaTime;
        jumpBufferTimer -= Time.deltaTime;

        #endregion

        #region Limit Speed

        if (Mathf.Abs(rb.velocity.y) > Data.maxFallSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Sign(rb.velocity.y) * Data.maxFallSpeed, rb.velocity.z);
        }

        if (moveInput.x != 0 && moveInput.y != 0 && isGrounded && !GetComponent<Abilities>().isPunching)
        {
            Vector3 newVelocity = Vector3.ClampMagnitude(new Vector3(rb.velocity.x, 0, rb.velocity.z), Data.movementSpeed);
            rb.velocity = new Vector3(newVelocity.x, rb.velocity.y, newVelocity.z);
        }

        if (Mathf.Abs(rb.velocity.x) < 0.1f && rb.velocity.x != 0)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
        }

        if (Mathf.Abs(rb.velocity.z) < 0.1f && rb.velocity.z != 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);
        }

        if (Mathf.Abs(rb.velocity.y) < 0.1f && rb.velocity.y != 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }

        #endregion

        #region Jump

        if (Input.GetKeyDown(Data.JumpKey))
        {
            jumpBufferTimer = Data.jumpBufferTime;
        }

        if (jumpBufferTimer > 0 && isGroundedTimer > 0 && jumps > 0)
        {
            Jump();
        }

        #endregion
    }

    private void FixedUpdate()
    {
        #region Run

        if ((new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude < 0.1f && moveInput == Vector2.zero) ||
            GetComponent<Abilities>().isPunching)
        {
            return;
        }

        float mult;

        if (isGrounded)
        {
            mult = Data.groundedPowerMult;
        }
        else
        {
            mult = Data.airedPowerMult;
        }

        Vector3 velocity = player.InverseTransformDirection(rb.velocity);

        #region X axis

        float targetSpeedX = moveInput.x * Data.movementSpeed * Mathf.Sqrt(2);

        //calculating the amount of speed the player needs to get to the desired speed
        float speedDifX = targetSpeedX - velocity.x;

        #region Acceleration

        float accelRateX;

        if (isGrounded)
        {
            accelRateX = (moveInput.x != 0 && Mathf.Sign(moveInput.x) != -Mathf.Sign(velocity.x)) ? Data.acceleration : Data.deceleration;
        }
        else
        {
            accelRateX = (moveInput.x != 0 && Mathf.Sign(moveInput.x) != -Mathf.Sign(velocity.x)) ? Data.airAcceleration : Data.airDeceleration;
        }

        #endregion

        //increasing the velocity over time
        float movementX = speedDifX * accelRateX * mult;

        //adding the force to the player
        //rb.AddForce(movementX * player.right * 50 * Time.fixedDeltaTime, ForceMode.Force);

        #endregion

        #region Z axis

        float targetSpeedZ = moveInput.y * Data.movementSpeed * Mathf.Sqrt(2);

        //calculating the amount of speed the player needs to get to the desired speed
        float speedDifZ = targetSpeedZ - velocity.z;

        #region Acceleration

        float accelRateZ;

        if (isGrounded)
        {
            accelRateZ = (moveInput.y != 0 && Mathf.Sign(moveInput.y) == Mathf.Sign(velocity.z)) ? Data.acceleration : Data.deceleration;
        }
        else
        {
            accelRateZ = (moveInput.y != 0 && Mathf.Sign(moveInput.y) == Mathf.Sign(velocity.z)) ? Data.airAcceleration : Data.airDeceleration;
        }

        #endregion

        //increasing the velocity over time
        float movementZ = speedDifZ * accelRateZ * mult;

        //adding the force to the player
        //rb.AddForce(movementZ * player.forward * 50 * Time.fixedDeltaTime, ForceMode.Force);

        #endregion

        float limiter = 1;

        if (moveInput.x != 0 && moveInput.y != 0)
        {
            limiter = Mathf.Sqrt(2);
        }

        rb.AddForce((movementX * player.right + movementZ * player.forward) * 50 * Time.fixedDeltaTime / limiter, ForceMode.Force);

        #endregion
    }

    #region Basic Methods

    private void Jump()
    {
        jumps--;

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        rb.AddForce(transform.up * Data.jumpForce, ForceMode.Impulse);
    }

    #endregion

    #region

    private IEnumerator SetGravityScale(float desiredGravity, float time)
    {
        float currentGravity = GetComponent<ConstantForce>().force.y;
        float newGravityScale = currentGravity;

        float t = 0;

        while (time > 0)
        {
            t += Time.deltaTime;
            float percent = Mathf.Clamp01(t / time);
            percent = Mathf.SmoothStep(0, 1, percent);

            newGravityScale = Mathf.Lerp(currentGravity, desiredGravity, percent);

            GetComponent<ConstantForce>().force = new Vector3(GetComponent<ConstantForce>().force.x, newGravityScale, GetComponent<ConstantForce>().force.z);

            yield return null;
        }

        GetComponent<ConstantForce>().force = new Vector3(GetComponent<ConstantForce>().force.x, desiredGravity, GetComponent<ConstantForce>().force.z);
    }

    #endregion
}
