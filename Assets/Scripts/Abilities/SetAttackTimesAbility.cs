using UnityEngine;

[CreateAssetMenu(fileName = "Set Attack Times Ability", menuName = "Abilities/Set Attack Times Ability")]
public class SetAttackTimesAbility : ChessFigureAbility
{
    [Header("Settings")]
    public int attackTimesToSet = 2;

    public override void PerformAbility(ChessFigure figure)
    {
        figure.figureBrain.attackTimes = attackTimesToSet;
    }
}