using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkAnimationTransfer : NetworkBehaviour
{
    private PathHash networkPathHash;
    private bool networkFlipPlayer;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        if (IsOwner)
        {
            animator.SetBool("isOwner", true);
        }
        else
        {
            animator.SetBool("isOwner", false);
        }

        networkPathHash = new PathHash(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            if (networkPathHash.value != networkPathHash.lastValue)
            {
                networkPathHash.lastValue = networkPathHash.value;

                animator.Play(networkPathHash.value);
            }

            if (networkFlipPlayer && !GetComponent<SpriteRenderer>().flipX)
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                if (!networkFlipPlayer && GetComponent<SpriteRenderer>().flipX)
                {
                    GetComponent<SpriteRenderer>().flipX = false;
                }
            }

            return;
        }

        SentAnimationStateToServerRpc(animator.GetCurrentAnimatorStateInfo(0).fullPathHash);
        SentDirectionToServerRpc(GetComponent<SpriteRenderer>().flipX);
    }

    //path hash
    public class PathHash
    {
        public int value;
        public int lastValue;

        public PathHash(int v)
        {
            value = v;
            lastValue = v;
        }
    }

    //animation
    [ServerRpc]
    private void SentAnimationStateToServerRpc(int pathHash)
    {
        SentAnimationStateFromClientRpc(pathHash);
    }

    [ClientRpc]
    private void SentAnimationStateFromClientRpc(int pathHash)
    {
        if (IsOwner)
        {
            return;
        }

        networkPathHash.value = pathHash;
    }

    //direction
    [ServerRpc]
    private void SentDirectionToServerRpc(bool b)
    {
        SentDirectionFromClientRpc(b);
    }

    [ClientRpc]
    private void SentDirectionFromClientRpc(bool b)
    {
        if (IsOwner)
        {
            return;
        }

        networkFlipPlayer = b;
    }
}
