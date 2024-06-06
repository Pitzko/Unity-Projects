using UnityEngine;

[CreateAssetMenu(menuName = "GameData")]
public class GameData : ScriptableObject
{
    [Header("Circular Grid")]
    public float radius = 2;
    public int startingNumberOfTiles = 3;
    [Space]

    [Header("Tiles")]
    public GameObject[] Tiles = new GameObject[3];
    public float spawnScaleDuration;
    public float spawnScaleMult;
    public float addTileMoveDuration;
    [Space]

    [Header("Dragging")]
    public float maxDragDistance;
    public float dragSpeed;
    [Space]

    [Header("Merging")]
    public GameObject[] MergeTiles = new GameObject[3];
    public float timeToMerge;
    public float timeBetweenMerges;
    public float mergeDelay;
    public float mergeTileProbability;
    [Space]

    [Header("Animations")]
    public float spawnAnimationTicks;
    public float spawnAnimationTickDuration;
    public float rearrangeDelay;

}