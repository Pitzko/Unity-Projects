using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Keybinds")]
    public KeyCode JumpKey;
    public KeyCode punchKey;
    public KeyCode liftKey;
    [Space]

    [Header("Sway & Bobbing")]
    [Header("Sway")]
    public float step;
    public float maxStepDistance;
    public float rotationStep;
    public float maxRotationStep;
    public float smooth;
    public float rotationSmooth;
    [Header("Bobbing")]
    public Vector3 bobTravelLimit;
    public Vector3 bobLimit;
    public Vector3 bobMultiplier;
    [Space]

    [Header("Mouse Movement")]
    public float mouseSensitivity;

    [Header("Player Movement")]
    public float groundedPowerMult;
    public float airedPowerMult;
    public float movementSpeed;
    public float acceleration;
    public float deceleration;
    public float airAcceleration;
    public float airDeceleration;
    public float maxFallSpeed;
    [Space]

    [Header("Physics")]
    public float playerHeight;
    public float groundCheckBoost;
    public LayerMask WhatIsGround;
    public float gravityScale;
    [Space]

    [Header("Jump")]
    public int jumpsCount;
    public float jumpForce;
    public float jumpBufferTime;
    public float groundedBufferTime;
    [Space]

    [Header("Abilities")]
    [Header("Punch")]
    public float punchChargeSlowMultiplier;
    public float punchChargeTime;
    public float punchChargeBonusTime;
    public float punchMinForce;
    public float punchMaxForce;
    public float punchMinDuration;
    public float punchMaxDuration;
    public float punchVelocityLimit;
    public float punchGravityMultiplier;
    public float punchBoostForceMultiplier;
    public float punchBoostDurationMultiplier;
    public float punchBoostUpForce;
    public float punchSlipperyMultiplier;
    public float punchSlipperyRecoveryTime;
    [Header("Lift")]
    public float liftDistanceUp;
    public float liftForceForward;
    public float liftSlipperyMultiplier;
    public float liftSlipperyRecoveryTime;
    public float liftGravityMultiplier;
    public float liftGravityRecoveryTime;
    public float liftGravityChangeDelay;
}
