using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwayAndBobbing : MonoBehaviour
{
    public PlayerData Data;
    [SerializeField] private Transform player;

    //sway
    private Vector3 swayPos;
    private Vector3 swayEulerRot;
    private Vector2 lookInput;

    //bobbing
    float curveSin { get => Mathf.Sin(speedCurve); }
    float curveCos { get => Mathf.Cos(speedCurve); }
    private float speedCurve;
    private Vector3 bobPosition;
    private Vector2 moveInput;
    private Vector3 bobEulerRotation;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        #region Inputs

        //mouse
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        lookInput = new Vector2(mouseX, mouseY);

        //keyboard
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(horizontalInput, verticalInput);

        #endregion

        //sway & bobbing
        Sway();
        BobOffset();

        SwayRotation();
        BobRotation();

        //compose
        CompositePositionRotation();
    }

    #region Sway

    private void Sway()
    {
        Vector3 invertedLook = lookInput * -Data.step;

        invertedLook.x = Mathf.Clamp(invertedLook.x, -Data.maxStepDistance, Data.maxStepDistance);
        invertedLook.y = Mathf.Clamp(invertedLook.y, -Data.maxStepDistance, Data.maxStepDistance);

        swayPos = invertedLook;
    }

    private void SwayRotation()
    {
        Vector2 invertedLook = lookInput * -Data.rotationStep;

        invertedLook.x = Mathf.Clamp(invertedLook.x, -Data.maxRotationStep, Data.maxRotationStep);
        invertedLook.y = Mathf.Clamp(invertedLook.y, -Data.maxRotationStep, Data.maxRotationStep);

        swayEulerRot = new Vector3(invertedLook.y, invertedLook.x, invertedLook.x);
    }

    #endregion

    #region Bobbing

    private void BobOffset()
    {
        bool grounded = Physics.Raycast(transform.position, Vector3.down, Data.playerHeight * 0.5f + Data.groundCheckBoost, Data.WhatIsGround);

        speedCurve += Time.deltaTime * (grounded ? player.GetComponent<Rigidbody>().velocity.magnitude : 1) + 0.01f;

        bobPosition.x = (curveCos * Data.bobLimit.x * (grounded ? 1 : 0)) - (moveInput.x * Data.bobTravelLimit.x);

        bobPosition.y = (curveSin * Data.bobLimit.y) - (player.GetComponent<Rigidbody>().velocity.y * Data.bobTravelLimit.y);

        if (bobPosition.y > 0.1f)
        {
            bobPosition.y = 0.1f;
        }
        else
        {
            if (bobPosition.y < -0.1f)
            {
                bobPosition.y = -0.1f;
            }
        }

        bobPosition.z = -(moveInput.y * Data.bobTravelLimit.z);
    }

    private void BobRotation()
    {
        bobEulerRotation.x = (moveInput != Vector2.zero) ? Data.bobMultiplier.x * (Mathf.Sin(2 * speedCurve)) : 0;

        bobEulerRotation.y = (moveInput != Vector2.zero) ? Data.bobMultiplier.y * curveCos : 0;

        bobEulerRotation.z = (moveInput != Vector2.zero) ? Data.bobMultiplier.z * curveCos : 0;
    }

    #endregion

    private void CompositePositionRotation()
    {
        //position
        transform.localPosition = Vector3.Lerp(transform.localPosition, swayPos + bobPosition, Time.deltaTime * Data.smooth);

        if (!Physics.Raycast(player.position, Vector3.down, Data.playerHeight * 0.5f + Data.groundCheckBoost, Data.WhatIsGround) &&
            !player.GetComponent<Abilities>().isPunching)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(swayEulerRot) * Quaternion.Euler(bobEulerRotation), Time.deltaTime * Data.rotationSmooth / 2);

            return;
        }

        //rotation
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(swayEulerRot) * Quaternion.Euler(bobEulerRotation), Time.deltaTime * Data.rotationSmooth);
    }
}
