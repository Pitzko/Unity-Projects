using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    public PlayerData Data;

    [SerializeField] Animator animator;
    [SerializeField] float timeToGenerate;

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.transform.CompareTag("Player") && collision.transform.GetComponent<PlayerController>().dashes < Data.dashes)
    //    {
    //        collision.transform.GetComponent<PlayerController>().dashes = Data.dashes;

    //        animator.SetBool("isBroken", true);

    //        Invoke("Generate", timeToGenerate);
    //    }
    //}

    //private void Generate()
    //{
    //    animator.SetBool("isBroken", false);
    //}
}