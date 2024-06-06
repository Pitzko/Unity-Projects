using UnityEngine;

[CreateAssetMenu(menuName = "GameData")] //Create a new playerData object by right clicking in the Project Menu then Create/Player/Player Data and drag onto the player

public class GameData : ScriptableObject
{
    [Header("Elements Grid Data")]
    public float elementalSpaceBetweenGridsX;
    public float elementalSpaceBetweenGridsY;
    public Vector2 elementalGridScale;
    [Space]

    [Header("Main Grid Data")]
    public float mainSpaceBetweenGridsX;
    public float mainSpaceBetweenGridsY;
    public Vector2 mainGridScale;
    [Space]

    [Header("Elements")]
    public float dragDuration = 1f;
    public float scaleDuration = 1f;
    public Vector2 originalElementsScale;
    public Vector2 scaleMult;
}
