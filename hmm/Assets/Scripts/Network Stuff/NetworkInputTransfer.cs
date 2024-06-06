using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkInputTransfer : NetworkBehaviour
{
    public PlayerData Data;

    //interact button
    public bool pressedInteract = false;
    private bool networkPressedInteract = false;

    //move input
    public Vector2 moveInput;
    private Vector2 networkMoveInput;

    //grounded
    public bool isGrounded = false;
    private bool networkIsGrounded = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            pressedInteract = networkPressedInteract;
            moveInput = networkMoveInput;
            isGrounded = networkIsGrounded;

            return;
        }

        #region Checking Stuff

        if (Input.GetKey(Data.interactKey))
        {
            pressedInteract = true;
        }
        else
        {
            pressedInteract = false;
        }

        #endregion

        //interact
        SentPressedInteractKeyToServerRpc(pressedInteract);

        //move input
        moveInput = GetComponent<PlayerController>().moveInput;
        SentMoveInputToServerRpc(moveInput);

        //grounded
        isGrounded = GetComponent<PlayerController>().isGrounded;
        SentIsGroundedToServerRpc(isGrounded);
    }

    #region Interact Button

    [ServerRpc]
    private void SentPressedInteractKeyToServerRpc(bool b)
    {
        SentPressedInteractKeyFromClientRpc(b);
    }

    [ClientRpc]
    private void SentPressedInteractKeyFromClientRpc(bool b)
    {
        if (IsOwner)
        {
            return;
        }

        networkPressedInteract = b;
    }

    #endregion

    #region Move Input

    [ServerRpc]
    private void SentMoveInputToServerRpc(Vector2 input)
    {
        SentMoveInputFromClientRpc(input);
    }

    [ClientRpc]
    private void SentMoveInputFromClientRpc(Vector2 input)
    {
        if (IsOwner)
        {
            return;
        }

        networkMoveInput = input;
    }

    #endregion

    #region Grounded

    [ServerRpc]
    private void SentIsGroundedToServerRpc(bool b)
    {
        SentIsGroundedFromClientRpc(b);
    }

    [ClientRpc]
    private void SentIsGroundedFromClientRpc(bool b)
    {
        if (IsOwner)
        {
            return;
        }

        networkIsGrounded = b;
    }

    #endregion
}
