using UnityEngine;
using VInspector;

public class ChessFigureAbilities : MonoBehaviour
{
    [Header("References")]
    public ChessFigure owner;
    public SerializedDictionary<AbilitiesEnum, ChessFigureAbilityWrapper> figureAbilities = new SerializedDictionary<AbilitiesEnum, ChessFigureAbilityWrapper>();

    public bool IsAbilityUnlocked(AbilitiesEnum ability)
    {
        if (!figureAbilities.ContainsKey(ability)) return false;
        return figureAbilities[ability].isUnlocked;
    }

    public void TryPerformAbility(AbilitiesEnum ability)
    {
        if(IsAbilityUnlocked(ability))
        {
            figureAbilities[ability].ability.PerformAbility(owner);
        }
    }

    public float GetTriggerChance(AbilitiesEnum ability)
    {
        return figureAbilities[ability].ability.triggerChance;
    }
}