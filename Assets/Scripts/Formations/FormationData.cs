using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Formation", menuName = "Formations/New Formation Data")]
public class FormationData : ScriptableObject
{
    [Serializable]
    public class PositionPiecePair
    {
        public Vector2Int position;
        public ChessFigureType pieceType;
    }

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

    public string formationName;
    public Sprite formationIcon;
    public ActionType formationActionType;
    public FormationBonus formationBonus;
    public PositionPiecePair[] positionPiecePairs;
}