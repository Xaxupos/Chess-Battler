using System.Collections.Generic;
using UnityEngine;

public struct FigureSaveData
{
    public int boardX;
    public int boardY;
    public float attack;
    public float health;
    public ChessFigureType figureType;
}

[System.Serializable]
public class FigureSaveDataList
{
    public List<FigureSaveData> figures;

    public FigureSaveDataList(List<FigureSaveData> figures)
    {
        this.figures = figures;
    }
}