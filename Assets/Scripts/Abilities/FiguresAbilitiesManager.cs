using UnityEngine;
using VInspector;

public class FiguresAbilitiesManager : MonoBehaviour
{
    [Header("References")]
    public SerializedDictionary<ChessFigureType, SerializedDictionary<AbilitiesEnum, ChessFigureAbilityWrapper>> allAbilities;

    public static FiguresAbilitiesManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void AssignAbilityValueToFigure(ChessFigure figure)
    {
        if (!allAbilities.ContainsKey(figure.figureType)) return;

        figure.figureAbilities.figureAbilities.Clear();
        var fAbilities = figure.figureAbilities.figureAbilities;
        var figureType = figure.figureType;
        var abilitiesForThatFigureType = allAbilities[figureType];


        foreach(var abilityKVP in abilitiesForThatFigureType)
        {
            fAbilities.Add(abilityKVP.Key, abilityKVP.Value);
        }
    }
}