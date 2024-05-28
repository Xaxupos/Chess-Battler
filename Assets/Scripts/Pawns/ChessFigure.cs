using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class ChessFigure : MonoBehaviour
{
    [Header("References")]
    public SpriteRenderer spriteRenderer;
    public Sprite whiteSprite;
    public Sprite blackSprite;

    [Header("Parameters")]
    public bool attacksSameAsMoves = true;
    public List<Vector2Int> possibleMoves = new List<Vector2Int>();
    [HideIf("attacksSameAsMoves")] public List<Vector2Int> possibleAttacks = new List<Vector2Int>();

    [Space(10)]
    public ChessSide side;

    private ChessboardSquare currentSquare;
    private Chessboard chessboard;

    [Button]
    public void PerformTurn()
    {
        ChessboardSquare squareToAttack = GetBestSquareToAttack();

        if (squareToAttack)
        {
            AttackSquare(squareToAttack);
        }
        else
        {
            ChessboardSquare squareToMove = GetRandomSquareToMove();
            if (squareToMove)
            {
                MoveToSquare(squareToMove);
            }
        }
    }

    public void InitChessFigure(ChessboardSquare initSquare, ChessSide initSide)
    {
        AssignFigureToSquare(initSquare);
        chessboard = initSquare.Chessboard;
        side = initSide;

        spriteRenderer.sprite = side == ChessSide.WHITE ? whiteSprite : blackSprite;

        if (attacksSameAsMoves) possibleAttacks = new List<Vector2Int>(possibleMoves);
        if (side == ChessSide.BLACK) InvertMoves();
    }

    private void MoveToSquare(ChessboardSquare square)
    {
        AssignFigureToSquare((ChessboardSquare) square);
    }

    private void AttackSquare(ChessboardSquare square)
    {

    }

    private ChessboardSquare GetRandomSquareToMove()
    {
        List<ChessboardSquare> validMoves = new List<ChessboardSquare>();

        foreach (var move in possibleMoves)
        {
            Vector2Int targetPosition = currentSquare.GetBoardPosition() + move;
            if (chessboard.IsWithinBounds(targetPosition))
            {
                ChessboardSquare targetSquare = chessboard.GetSquareAtPosition(targetPosition);
                if (targetSquare.IsEmpty())
                {
                    validMoves.Add(targetSquare);
                }
            }
        }

        if (validMoves.Count > 0)
        {
            return validMoves[Random.Range(0, validMoves.Count)];
        }

        return null;
    }

    private ChessboardSquare GetBestSquareToAttack()
    {
        return null;
    }

    private void InvertMoves()
    {
        for (int i = 0; i < possibleMoves.Count; i++)
        {
            possibleMoves[i] = new Vector2Int(-possibleMoves[i].x, -possibleMoves[i].y);
        }

        for (int i = 0; i < possibleAttacks.Count; i++)
        {
            possibleAttacks[i] = new Vector2Int(-possibleAttacks[i].x, -possibleAttacks[i].y);
        }
    }

    private void AssignFigureToSquare(ChessboardSquare square)
    {
        currentSquare = square;
        currentSquare.AssignFigure(this);

        transform.SetParent(currentSquare.transform);
        transform.localPosition = Vector2.zero;
    }
}