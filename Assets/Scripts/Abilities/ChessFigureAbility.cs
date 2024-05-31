using UnityEngine;

public abstract class ChessFigureAbility : ScriptableObject
{
    public bool onInitAbility;
    [Range(0,1.0f)] public float triggerChance = 1.0f;
    public string abilityName;
    public int abilityUnlockCost;

    public abstract void PerformAbility(ChessFigure figure);
}