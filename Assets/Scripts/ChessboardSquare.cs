using UnityEngine;

public class ChessboardSquare : MonoBehaviour
{
    public ChessFigure currentFigure;
    private Vector2Int boardPosition;

    public Chessboard Chessboard { get; set; }

    public void AssignFigure(ChessFigure figure)
    {
        currentFigure = figure;
    }

    public void ClearSquare()
    {
        currentFigure = null;
    }

    public void SetBoardPosition(Vector2Int positionToSet)
    {
        boardPosition = positionToSet;
    }

    public bool IsEmpty()
    {
        return currentFigure == null;
    }

    public Vector2Int GetBoardPosition()
    {
        return boardPosition;
    }
}