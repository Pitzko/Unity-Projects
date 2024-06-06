using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GadgetsManager : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] gadgets;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            SpawnOnNetwork(gadgets[0], new Vector2(-13, -7));
        }
    }

    private void SpawnOnNetwork(GameObject obj, Vector2 pos)
    {
        Transform objTransform = Instantiate(obj.transform);

        objTransform.position = pos;

        objTransform.GetComponent<NetworkObject>().Spawn(true);
    }
}
