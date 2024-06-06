using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Animations;

public class PlayerAnimatorController : NetworkBehaviour
{
    public PlayerData Data;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController playerController;
    [Space(10)]

    [Header("Squish Effect")]
    //jump
    private Vector2 jumpSquishedScale;
    private bool isJumpSquishing = false;
    private bool isJumpReturning = false;
    //land
    private Vector2 landSquishedScale;
    private bool isLandSquishing = false;
    private bool isLandReturning = false;
    //crouch
    private bool pressedCrouch = false;
    private bool unpressedCrouch = false;
    private bool canCrouchSquish = true;
    private float mult;
    //
    private float squishTimer = 0;
    private float returnTimer = 0;
    private Vector2 startScale;

    // Start is called before the first frame update
    void Start()
    {
        mult = 1;

        startScale = transform.localScale;

        jumpSquishedScale = new Vector2(startScale.x * Data.jumpSquish.x, startScale.y * Data.jumpSquish.y);
        landSquishedScale = new Vector2(startScale.x * Data.landSquish.x, startScale.y * Data.landSquish.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        #region Run

        if (playerController.isRunning)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        #endregion

        #region Crouch

        if (playerController.isCrouching)
        {
            if (!animator.GetBool("isCrouching"))
            {
                animator.SetBool("isCrouching", true);
            }
        }
        else
        {
            if (animator.GetBool("isCrouching"))
            {
                animator.SetBool("isCrouching", false);
            }
        }

        #endregion

        #region Jump & Land

        //jumping
        if (playerController.jumped)
        {
            animator.SetBool("isJumping", true);

            playerController.jumped = false;
        }
        else
        {
            if (animator.GetBool("isJumping"))
            {
                animator.SetBool("isJumping", false);
            }
        }

        //falling
        if (GetComponent<Rigidbody2D>().velocity.y < 0)
        {
            if (!animator.GetBool("isFalling"))
            {
                animator.SetBool("isFalling", true);
            }
        }
        else
        {
            if (animator.GetBool("isFalling"))
            {
                animator.SetBool("isFalling", false);
            }
        }

        //landing
        if (playerController.landed)
        {
            animator.SetBool("isLanding", true);

            playerController.landed = false;
        }
        else
        {
            if (animator.GetBool("isLanding"))
            {
                animator.SetBool("isLanding", false);
            }
        }

        #endregion

        //#region Squish

        ////jump squish
        //if ((playerController.jumped || unpressedCrouch) && !isLandSquishing)
        //{
        //    playerController.jumped = false;
        //    unpressedCrouch = false;

        //    isJumpSquishing = true;
        //    isJumpReturning = false;

        //    isLandSquishing = false;
        //    isLandReturning = false;

        //    squishTimer = 0;
        //    returnTimer = 0;
        //}

        //if (isJumpSquishing)
        //{
        //    squishTimer += Time.deltaTime;

        //    float t = Mathf.Clamp01(squishTimer / (Data.squishTimeJump * mult / 2));

        //    JumpSquish(startScale, jumpSquishedScale, t);
        //}
        //else
        //{
        //    if (isJumpReturning)
        //    {
        //        returnTimer += Time.deltaTime;

        //        float t = Mathf.Clamp01(returnTimer / (Data.squishTimeJump * mult / 2));

        //        JumpReturnSquish(jumpSquishedScale, startScale, t);
        //    }
        //}

        ////land squish
        //if (playerController.landed || pressedCrouch)
        //{
        //    playerController.landed = false;
        //    pressedCrouch = false;

        //    isLandSquishing = true;
        //    isLandReturning = false;

        //    isJumpSquishing = false;
        //    isJumpReturning = false;

        //    squishTimer = 0;
        //    returnTimer = 0;
        //}

        //if (isLandSquishing)
        //{
        //    squishTimer += Time.deltaTime;

        //    float t = Mathf.Clamp01(squishTimer / (Data.squishTimeLand / 2));

        //    LandSquish(startScale, landSquishedScale, t);
        //}
        //else
        //{
        //    if (isLandReturning)
        //    {
        //        returnTimer += Time.deltaTime;

        //        float t = Mathf.Clamp01(returnTimer / (Data.squishTimeLand / 2));

        //        LandReturnSquish(landSquishedScale, startScale, t);
        //    }
        //}

        //#endregion

        ////crouch land squish
        //if (playerController.isCrouching && canCrouchSquish)
        //{
        //    pressedCrouch = true;

        //    canCrouchSquish = false;
        //}
        //else
        //{
        //    if (!playerController.isCrouching && !canCrouchSquish)
        //    {
        //        unpressedCrouch = true;

        //        canCrouchSquish = true;
        //    }
        //}

        ////make it so that the animation is quicker when crouching
        //if ((pressedCrouch || unpressedCrouch) && mult != Data.crouchSquishMult)
        //{
        //    mult = Data.crouchSquishMult;
        //}
        //else
        //{
        //    if (mult != 1)
        //    {
        //        mult = 1;
        //    }
        //}
    }

    #region Minor Functions

    //jump
    private void JumpSquish(Vector2 start, Vector2 final, float t)
    {
        if (t >= 1f)
        {
            isJumpReturning = true;
            isJumpSquishing = false;

            squishTimer = 0;
            returnTimer = 0;
        }

        transform.localScale = Vector2.Lerp(start, final, t);
    }

    private void JumpReturnSquish(Vector2 start, Vector2 final, float t)
    {
        if (t >= 1f)
        {
            isJumpReturning = false;
            isJumpSquishing = false;

            squishTimer = 0;
            returnTimer = 0;
        }

        transform.localScale = Vector2.Lerp(start, final, t);
    }

    //land
    private void LandSquish(Vector2 start, Vector2 final, float t)
    {
        if (t >= 1f)
        {
            isLandReturning = true;
            isLandSquishing = false;

            squishTimer = 0;
            returnTimer = 0;
        }

        transform.localScale = Vector2.Lerp(start, final, t);
    }

    private void LandReturnSquish(Vector2 start, Vector2 final, float t)
    {
        if (t >= 1f)
        {
            isLandReturning = false;
            isLandSquishing = false;

            squishTimer = 0;
            returnTimer = 0;
        }

        transform.localScale = Vector2.Lerp(start, final, t);
    }

    #endregion
}