using UnityEngine;

public class ChessboardSquare : MonoBehaviour
{
    public ChessFigure CurrentFigure { get; private set; }
    public Chessboard Chessboard { get; set; }

    private Vector2Int boardPosition;

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