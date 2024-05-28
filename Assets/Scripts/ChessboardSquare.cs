using UnityEngine;

public class ChessboardSquare : MonoBehaviour
{
    public Vector2Int boardPosition;

    public void SetBoardPosition(Vector2Int positionToSet)
    {
        boardPosition = positionToSet;
    }
}