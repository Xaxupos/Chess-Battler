using VInspector;
using UnityEngine;

public class ChessFigureFormationController : MonoBehaviour
{
    public ChessFigure owner;
    [ReadOnly] public FigureActiveFormation currentFormation;

    public void SetCurrentFormationAndApplyBonus(FigureActiveFormation formation)
    {
        currentFormation = formation;
        formation.formationData.formationBonus.ApplyBonus(owner);
    }

    public void ClearCurrentFormationAndRevertBonus()
    {
        if (currentFormation == null || currentFormation.formationData == null) return;

        currentFormation.formationData.formationBonus.RevertBonus(owner);
        currentFormation = null;
    }


    public bool IsPartOfFormation()
    {
        return currentFormation != null && currentFormation.formationData != null;
    }
}