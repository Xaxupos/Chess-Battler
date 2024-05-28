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
    public bool wholeLineMovement = false;
    [HideIf("wholeLineMovement")] public List<Vector2Int> possibleMoves = new List<Vector2Int>();
    [ShowIf("wholeLineMovement")] public List<Vector2Int> possibleMovesDirections = new List<Vector2Int>();
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
        if(attacksSameAsMoves) possibleAttacks = new List<Vector2Int>(possibleMoves);

        if (side == ChessSide.BLACK && !wholeLineMovement) InvertMoves();

        if(wholeLineMovement)
            InitValidLineMovesAndAttacks();
    }

    private void InitValidLineMovesAndAttacks()
    {
        if (attacksSameAsMoves)
            possibleAttacks.Clear();

        possibleMoves.Clear();

        foreach (var moveDirection in possibleMovesDirections)
        {
            for (int i = 1; i < Mathf.Max(chessboard.GetGridSize().x, chessboard.GetGridSize().y); i++)
            {
                Vector2Int newPosition = moveDirection * i;

                if (chessboard.IsWithinBounds(currentSquare.GetBoardPosition() + newPosition))
                {
                    ChessboardSquare targetSquare = chessboard.GetSquareAtPosition(currentSquare.GetBoardPosition() + newPosition);

                    if (targetSquare.IsEmpty())
                    {
                        possibleMoves.Add(newPosition);
                    }
                    else
                    {
                        if (targetSquare.currentFigure.side != side)
                        {
                            if (attacksSameAsMoves)
                                possibleAttacks.Add(newPosition);
                        }
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }

    private void MoveToSquare(ChessboardSquare square)
    {
        AssignFigureToSquare(square);

        if(wholeLineMovement)
            InitValidLineMovesAndAttacks();
    }

    private void AttackSquare(ChessboardSquare square)
    {

    }

    private ChessboardSquare GetRandomSquareToMove()
    {
        if(wholeLineMovement)
            InitValidLineMovesAndAttacks();

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
        if(currentSquare)
            currentSquare.ClearSquare();

        currentSquare = square;
        currentSquare.AssignFigure(this);

        transform.SetParent(currentSquare.transform);
        transform.localPosition = Vector2.zero;
    }
}