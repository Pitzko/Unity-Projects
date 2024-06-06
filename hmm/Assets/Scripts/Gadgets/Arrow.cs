using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Arrow : NetworkBehaviour
{
    public PlayerData Data;

    [SerializeField] private Animator animator;
    [SerializeField] private float timeToGenerate;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsOwner)
        {
            return;
        }

        if (collision.transform.CompareTag("Player") && collision.transform.GetComponent<PlayerController>().dashes < Data.dashes)
        {
            collision.transform.GetComponent<PlayerController>().dashes = Data.dashes;

            animator.SetBool("isBroken", true);

            Invoke("Generate", timeToGenerate);
        }
    }

    private void Generate()
    {
        animator.SetBool("isBroken", false);
    }
}
