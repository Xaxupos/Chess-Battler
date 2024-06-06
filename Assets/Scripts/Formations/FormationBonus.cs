using UnityEngine;

public abstract class FormationBonus : ScriptableObject
{
    public abstract void ApplyBonus(ChessFigure figure);
    public abstract void RevertBonus(ChessFigure figure);
}