using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum NoteType
{
    None = -1,
    Red = 0,
    Blue = 1,
    Green = 2
}

public class Tile : MonoBehaviour
{
    //characteristics
    [HideInInspector] public bool isMerge;
    [HideInInspector] public bool isCharged;
    [SerializeField] private GameData Data;
    public NoteType noteType;
    private quaternion rotation;

    private void Start()
    {
        rotation = transform.rotation;
    }

    private void Update()
    {
        transform.rotation = rotation;
    }

    #region IEnumerators

    #endregion
}