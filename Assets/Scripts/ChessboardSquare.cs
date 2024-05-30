using UnityEngine;
using VInspector;

public class ChessboardSquare : MonoBehaviour
{
    [ShowInInspector] public ChessFigure CurrentFigure { get; private set; }
    [ShowInInspector] private Vector2Int boardPosition;

    public Chessboard Chessboard { get; set; }


    public void AssignFigure(ChessFigure figure)
    {
        CurrentFigure = figure;
    }

    public void ClearSquare()
    {
        CurrentFigure = null;
    }

    public void SetBoardPosition(Vector2Int positionToSet)
    {
        boardPosition = positionToSet;
    }

    public bool IsEmpty()
    {
        return CurrentFigure == null;
    }

    public Vector2Int GetBoardPosition()
    {
        return boardPosition;
    }
}