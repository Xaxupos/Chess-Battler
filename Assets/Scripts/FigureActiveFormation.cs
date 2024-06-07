using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FigureActiveFormation
{
    public GameObject overlay;
    public FormationData formationData;
    public List<Vector2Int> chessboardSquares;
}