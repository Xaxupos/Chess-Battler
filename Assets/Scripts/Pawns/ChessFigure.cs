using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class ChessFigure : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ChessFigureStatistics figureStatistics;
    [SerializeField] private ChessFigureHealthSystem figureHealthSystem;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite whiteSprite;
    [SerializeField] private Sprite blackSprite;

    [Header("Parameters")]
    [SerializeField] private bool attacksSameAsMoves = true;
    [SerializeField] private bool wholeLineMovement = false;
    [HideIf("wholeLineMovement")][SerializeField] private List<Vector2Int> possibleMoves = new List<Vector2Int>();
    [ShowIf("wholeLineMovement")][SerializeField] private List<Vector2Int> possibleMovesDirections = new List<Vector2Int>();
    [HideIf("attacksSameAsMoves")][SerializeField] private List<Vector2Int> possibleAttacks = new List<Vector2Int>();

    [Space(10)]
    [SerializeField] private ChessSide figureSide;

    public ChessboardSquare CurrentSquare { get; private set; }
    private Chessboard chessboard;

    [Button]
    public void PerformTurn()
    {
        if (wholeLineMovement)
            InitValidLineMovesAndAttacks();

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
        figureSide = initSide;

        spriteRenderer.sprite = figureSide == ChessSide.WHITE ? whiteSprite : blackSprite;
        if(attacksSameAsMoves) possibleAttacks = new List<Vector2Int>(possibleMoves);

        if (figureSide == ChessSide.BLACK && !wholeLineMovement) InvertMoves();

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

                if (chessboard.IsWithinBounds(CurrentSquare.GetBoardPosition() + newPosition))
                {
                    ChessboardSquare targetSquare = chessboard.GetSquareAtPosition(CurrentSquare.GetBoardPosition() + newPosition);

                    if (targetSquare.IsEmpty())
                    {
                        possibleMoves.Add(newPosition);
                    }
                    else
                    {
                        if (targetSquare.CurrentFigure.figureSide != figureSide)
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
        ChessFigure attacker = this;
        ChessFigure defender = square.CurrentFigure;

        var calculatedDamage = attacker.figureStatistics.GetStatisticValue(FigureStatistic.BASE_DAMAGE);

        defender.figureStatistics.ChangeStatistic(FigureStatistic.CURRENT_HEALTH, -calculatedDamage);

        if(defender.figureHealthSystem.IsDead)
        {
            MoveToSquare(square);
        }
    }

    private ChessboardSquare GetRandomSquareToMove()
    {
        List<ChessboardSquare> validMoves = new List<ChessboardSquare>();
        foreach (var move in possibleMoves)
        {
            Vector2Int targetPosition = CurrentSquare.GetBoardPosition() + move;
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
        ChessboardSquare bestSquareToAttack = null;
        float currentHighestPriority = -1;

        foreach(var squarePos in possibleAttacks)
        {
            var targetPosition = CurrentSquare.GetBoardPosition() + squarePos;
            ChessboardSquare square = chessboard.GetSquareAtPosition(targetPosition);
            if (square.IsEmpty() || square.CurrentFigure.figureSide == figureSide) continue;

            if(square.CurrentFigure.figureStatistics.GetStatisticValue(FigureStatistic.TARGETED_PRIORITY) > currentHighestPriority)
            {
                bestSquareToAttack = square;
                continue;
            }
        }

        return bestSquareToAttack;
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
        if(CurrentSquare)
            CurrentSquare.ClearSquare();

        CurrentSquare = square;
        CurrentSquare.AssignFigure(this);

        transform.SetParent(CurrentSquare.transform);
        transform.localPosition = Vector2.zero;
    }
}