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

    public string formationName;
    public PositionPiecePair[] positionPiecePairs;
}