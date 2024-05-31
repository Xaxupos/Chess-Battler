using System.Collections.Generic;
using UnityEngine;

public class ChessFigure : MonoBehaviour
{
    [Header("References")]
    public ChessFigureStatistics figureStatistics;
    public ChessFigureHealthEvents figureHealthSystem;
    public ChessFigureBrain figureBrain;
    public ChessFigureSFX figureSFX;
    public ChessFigureVFX figureVFX;
    public ChessFigureInvalidTimer figureInvalidTimer;
    public ChessFigureAbilities figureAbilities;
    public SpriteRenderer spriteRenderer;
    public Sprite whiteSprite;
    public Sprite blackSprite;

    [Header("Parameters")]
    public ChessFigureType figureType;

    public ChessSide FigureSide { get; private set; }
    public ChessboardSquare CurrentSquare { get; private set; }
    public Vector2Int InitSquarePos { get; set; }
    public Chessboard ChessboardReference { get; private set; }

    public void PerformTurn()
    {
        figureBrain.PerformBestAction();
    }

    public void InitChessFigure(ChessboardSquare initSquare, ChessSide initSide)
    {
        AssignFigureToSquare(initSquare);
        ChessboardReference = initSquare.Chessboard;
        FigureSide = initSide;
        InitSquarePos = initSquare.GetBoardPosition();

        spriteRenderer.sprite = FigureSide == ChessSide.WHITE ? whiteSprite : blackSprite;
        if(figureBrain.attacksSameAsMoves) figureBrain.possibleAttacks = new List<Vector2Int>(figureBrain.possibleMoves);

        if (FigureSide == ChessSide.BLACK && !figureBrain.wholeLineMovement) figureBrain.InvertMoves();

        if(figureBrain.wholeLineMovement)
            figureBrain.InitValidLineMovesAndAttacks();
    }

    public void AssignFigureToSquare(ChessboardSquare square, bool forceMove = false)
    {
        if(CurrentSquare)
            CurrentSquare.ClearSquare();

        CurrentSquare = square;
        CurrentSquare.AssignFigure(this);

        transform.SetParent(CurrentSquare.transform);
        transform.localScale = Vector2.one;

        if (forceMove)
        {
            transform.localPosition = Vector2.zero;
        }
    }

    public void ClearCurrentSquare()
    {
        CurrentSquare = null;
    }

    public void SetupBasedOnFigureStruct(FigureSaveData figureStruct)
    {
        InitSquarePos = new Vector2Int(figureStruct.boardX, figureStruct.boardY);
        figureStatistics.SetStatistic(FigureStatistic.CURRENT_HEALTH, figureStruct.health);
        figureStatistics.SetStatistic(FigureStatistic.BASE_DAMAGE, figureStruct.attack);
    }
}