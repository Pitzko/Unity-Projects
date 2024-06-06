using UnityEngine;

[CreateAssetMenu(menuName = "Player Data")] //Create a new playerData object by right clicking in the Project Menu then Create/Player/Player Data and drag onto the player
public class PlayerData : ScriptableObject
{
    [Header("KeyCodes")]
    public KeyCode jumpKey;
    public KeyCode dashKey;
    public KeyCode interactKey;
    [Space(10)]

    [Header("Movement")]
    public float moveSpeed;
    public float acceleration;
    public float deceleration;
    public float powerMult;
    public float airAcceleration;
    public float airDeceleration;
    public float maxFallSpeed = -50;
    public int jumps = 1;
    [Space(10)]

    [Header("Gravity")]
    public float jumpForce;
    public float jumpCoyoteTime;
    public float GroundCoyoteTime;
    public float gravityScale;
    public float jumpGravityThreshHold;
    public float jumpGravityMult;
    [Space(10)]

    [Header("Dashing")]
    public float dashDelay;
    public float dashForce;
    public float dashTime;
    public float dashCooldown;
    public int dashes = 1;
    [Space(10)]

    [Header("Wall Sliding / Jumping")]
    public float wallSlideSpeed;
    public float wallJumpForce;

    [Header("Animations")]
    public Vector2 jumpSquish;
    public float squishTimeJump;
    [Space]
    public Vector2 landSquish;
    public float squishTimeLand;
    [Space]
    public float crouchSquishMult = 0.5f;
}
