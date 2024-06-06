using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public PlayerData Data;

    //mouse
    [HideInInspector] public bool lockedCamera = false;
    [SerializeField] private Transform arms;
    private float xRotation;
    private float yRotation;
    private float originalYRotation;
    private Quaternion originalArmRotation;
    private bool justLockedCamera;

    //player
    [SerializeField] private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //getting the mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * 0.01f * Data.mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * 0.01f * Data.mouseSensitivity;

        if (!lockedCamera)
        {
            yRotation += mouseX;
            xRotation -= mouseY;

            xRotation = Mathf.Clamp(xRotation, -90, 90);

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            player.rotation = Quaternion.Euler(0, yRotation, 0);

            if (!justLockedCamera)
            {
                justLockedCamera = true;
            }
        }
        else
        {
            if (justLockedCamera)
            {
                originalArmRotation = transform.rotation;
                originalYRotation = yRotation;

                justLockedCamera = false;
            }

            yRotation += mouseX;
            xRotation -= mouseY;

            xRotation = Mathf.Clamp(xRotation, -90, 90);
            yRotation = Mathf.Clamp(yRotation, originalYRotation - 90, originalYRotation + 90);

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);

            arms.rotation = originalArmRotation;
        }
    }
}