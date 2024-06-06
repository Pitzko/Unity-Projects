using UnityEngine;

[CreateAssetMenu(menuName = "FirstBossData")] //Create a new playerData object by right clicking in the Project Menu then Create/Player/Player Data and drag onto the player
public class FirstBossData : ScriptableObject
{
    [Header("FIRST PHASE")]
    [Space]
    [Header("Floating")]
    public float floatSpeed;
    public float distanceMult;
    [Space]
    [Header("General Data")]
    public int numberOfAttacks;
    public Vector2 timeBetweenAttacks;
    [Space]
    [Header("Attack 1")]
    public float returnToIdleTime = 0.5f;
    public float rollDuration1 = 0.7f;
    public float delayBeforeDrop = 0.2f;
    public float yGroundLevel;
    public float dropDuration = 0.3f;
    public AnimationCurve dropCurve;
    public AnimationCurve rollCurve;
    public int boxRolls1;
}