using UnityEngine;
using System;
using VInspector;

[CreateAssetMenu(fileName = "New Formation", menuName = "Formations/New Formation Data")]
public class FormationData : ScriptableObject
{
    [Tab("Core")]
    public ActionType formationActionType;
    public FormationBonus formationBonus;
    public PositionPiecePair[] positionPiecePairs;

    [Tab("Visuals")]
    public string formationName;
    [TextArea()]
    public string formationDescription;
    public Sprite formationBoardVisual;
    public Sprite formationIcon;

    public int GetPieceTypeAtPosition(Vector2Int position)
    {
        foreach (var pair in positionPiecePairs)
        {
            if (pair.position == position)
            {
                return (int)pair.pieceType;
            }
        }
        return -1;
    }
}

[Serializable]
public class PositionPiecePair
{
    public Vector2Int position;
    public ChessFigureType pieceType;
}