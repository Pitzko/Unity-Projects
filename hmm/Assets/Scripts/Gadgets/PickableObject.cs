using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PickableObject : NetworkBehaviour
{
    public PlayerData Data;

    //stats & references
    [Header("Stats & References")]
    [SerializeField] private Vector2 grabOffset;
    [SerializeField] private float grabSpeed;
    [SerializeField] private float TossForce;
    [SerializeField] private Collider2D col;
    [SerializeField] private float gravityMult;
    [SerializeField] private float ignoreCollisionDuration;
    [Space(10)]

    //veriables
    private bool isGrabbing;
    private bool isGrabable;
    private List<GameObject> players;
    private GameObject player;
    private GameObject grabbingPlayer;

    // Start is called before the first frame update
    void Start()
    {
        isGrabbing = false;
        isGrabable = true;

        col.enabled = false;

        GetComponent<Rigidbody2D>().gravityScale = 0;

        players = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        #region Checking Stuff

        CheckIfInteractIsPressed(players);

        if (player && isGrabable)
        {
            grabbingPlayer = player;

            transform.position = Vector2.Lerp(transform.position, (Vector2)player.transform.position + grabOffset, Time.deltaTime * grabSpeed);

            if (!isGrabbing)
            {
                isGrabbing = true;
            }

            if (GetComponent<Floating>() && GetComponent<Floating>().enabled)
            {
                GetComponent<Floating>().enabled = false;
            }
        }
        else
        {
            if (isGrabbing)
            {
                Toss();

                isGrabbing = false;
                isGrabable = false;

                col.enabled = true;

                GetComponent<Rigidbody2D>().gravityScale = Data.gravityScale * gravityMult;
            }
        }

        #endregion
    }

    private void Toss()
    {
        if (!grabbingPlayer)
        {
            Debug.LogWarning("there is no player");

            return;
        }

        int dir = (grabbingPlayer.GetComponent<SpriteRenderer>().flipX) ? -1 : 1;

        Vector2 aim = (grabbingPlayer.GetComponent<NetworkInputTransfer>().moveInput == Vector2.zero || (grabbingPlayer.GetComponent<NetworkInputTransfer>().moveInput.y == -1
            && grabbingPlayer.GetComponent<NetworkInputTransfer>().isGrounded)) ? new Vector2(dir, 0) : grabbingPlayer.GetComponent<NetworkInputTransfer>().moveInput;

        GetComponent<Rigidbody2D>().AddForce(aim.normalized * TossForce, ForceMode2D.Impulse);

        StartCoroutine(IgnorePlayerCollision(ignoreCollisionDuration));
    }

    private IEnumerator IgnorePlayerCollision(float duration)
    {
        if (col && grabbingPlayer.GetComponent<Collider2D>())
        {
            Physics2D.IgnoreCollision(col, grabbingPlayer.GetComponent<Collider2D>());

            yield return new WaitForSecondsRealtime(duration);

            Physics2D.IgnoreCollision(col, grabbingPlayer.GetComponent<Collider2D>(), false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        players.Add(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        if (!isGrabbing)
        {
            players.Remove(collision.gameObject);
        }
    }

    private void CheckIfInteractIsPressed(List<GameObject> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (!list[i])
            {
                list.Remove(list[i]);
            }
            else
            {
                if (list[i].GetComponent<NetworkInputTransfer>().pressedInteract && (player == null || player == list[i]))
                {
                    player = list[i];

                    return;
                }
            }
        }

        player = null;
    }
}
