using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Set AOE Attack Ability", menuName = "Abilities/Set AOE Attack Ability")]
public class SetAoeAbility : ChessFigureAbility
{
    [Header("Settings")]
    public float aoeDamageMultiplier = 0.25f;
    public List<Vector2Int> aoeFromAttackedSquare = new List<Vector2Int>();

    public override void PerformAbility(ChessFigure figure)
    {
        figure.figureBrain.aoeAttacks.AddRange(aoeFromAttackedSquare);
        figure.figureBrain.aoeDamageMultiplier = aoeDamageMultiplier;
    }
}