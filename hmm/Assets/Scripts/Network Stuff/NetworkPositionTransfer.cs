using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPositionTransfer : NetworkBehaviour
{
    private Vector2 networkPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            transform.position = networkPos;

            return;
        }

        SentPositionToServerRpc(transform.position);
    }

    [ServerRpc]
    private void SentPositionToServerRpc(Vector2 pos)
    {
        SentPositionFromClientRpc(pos);
    }

    [ClientRpc]
    private void SentPositionFromClientRpc(Vector2 pos)
    {
        if (IsOwner)
        {
            return;
        }

        networkPos = pos;
    }
}
