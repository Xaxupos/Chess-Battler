using UnityEngine;
using System.Collections.Generic;

public class FormationsManager : MonoBehaviour
{
    public List<FormationData> formations;

    public static FormationsManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public bool IsFormationActive(FormationData formation, Chessboard chessboard, Vector2Int currentPosition)
    {
        foreach (var pair in formation.positionPiecePairs)
        {
            Vector2Int positionOffset = pair.position;
            Vector2Int targetPosition = currentPosition + positionOffset;
            ChessFigureType expectedPieceType = pair.pieceType;

            if (!IsPieceAtPosition(chessboard, targetPosition, expectedPieceType))
            {
                return false;
            }
        }

        Debug.Log(formation.name + " is active");
        return true;
    }

    public void ScanForFormations(Chessboard chessboard)
    {
        Vector2Int gridSize = chessboard.GetGridSize();

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                ChessboardSquare currentSquare = chessboard.GetSquareAtPosition(new Vector2Int(x, y));
                if (currentSquare != null && !currentSquare.IsEmpty())
                {
                    foreach (var formation in formations)
                    {
                        if (IsFormationActive(formation, chessboard, new Vector2Int(x, y)))
                        {
                            HighlightFormation(formation, chessboard, new Vector2Int(x, y));
                        }
                    }
                }
            }
        }
    }

    private bool IsPieceAtPosition(Chessboard chessboard, Vector2Int position, ChessFigureType expectedPieceType)
    {
        ChessboardSquare square = chessboard.GetSquareAtPosition(position);
        if (square != null && square.CurrentFigure != null && square.CurrentFigure.figureType == expectedPieceType)
        {
            return true;
        }
        return false;
    }

    private void HighlightFormation(FormationData formation, Chessboard chessboard, Vector2Int currentPosition)
    {
        foreach (var pair in formation.positionPiecePairs)
        {
            Vector2Int positionOffset = pair.position;
            Vector2Int targetPosition = currentPosition + positionOffset;
            ChessboardSquare square = chessboard.GetSquareAtPosition(targetPosition);
            if (square != null)
            {
                // Tutaj wykonaj akcje, np. podœwietlenie pól formacji
            }
        }
    }
}
