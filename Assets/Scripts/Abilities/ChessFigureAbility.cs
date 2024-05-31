using UnityEngine;
using VInspector;

public abstract class ChessFigureAbility : ScriptableObject
{
    [Tab("Core")]
    public AbilitiesEnum abilityEnum;
    public bool onInitAbility;
    [Range(0,1.0f)] public float triggerChance = 1.0f;

    [Tab("Visuals")]
    public string abilityName;
    public string abilityDesc;
    public Sprite abilityIcon;
    public int abilityUnlockCost;

    public abstract void PerformAbility(ChessFigure figure);
}