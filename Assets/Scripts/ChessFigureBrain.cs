using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VInspector;

public class ChessFigureBrain : MonoBehaviour
{
    [Tab("Core")]
    [Header("References")]
    public ChessFigure owner;

    [Header("Parameters")]
    public bool attacksSameAsMoves = true;
    public bool wholeLineMovement = false;
    [HideIf("wholeLineMovement")] public List<Vector2Int> possibleMoves = new List<Vector2Int>(); [EndIf]
    [ShowIf("wholeLineMovement")] public List<Vector2Int> possibleMovesDirections = new List<Vector2Int>(); [EndIf]
    [HideIf("attacksSameAsMoves")] public List<Vector2Int> possibleAttacks = new List<Vector2Int>(); [EndIf]

    [Tab("Values")]
    public float moveTweenDuration = 0.4f;
    public float knockbackForce = 0.1f;

    public bool DidActionThisTurn { get; private set; }
    public bool EndedPerform { get; private set; }

    public void PerformBestAction()
    {
        DidActionThisTurn = false;
        EndedPerform = false;

        if (wholeLineMovement)
            InitValidLineMovesAndAttacks();

        ChessboardSquare squareToAttack = GetBestSquareToAttack();

        if (squareToAttack)
        {
            AttackSquare(squareToAttack);
            DidActionThisTurn = true;
            return;
        }
        else
        {
            ChessboardSquare squareToMove = GetRandomSquareToMove();
            if (squareToMove)
            {
                MoveToSquare(squareToMove);
                DidActionThisTurn = true;
                return;
            }
        }

        EndedPerform = true;
    }

    public void InitValidLineMovesAndAttacks()
    {
        if (attacksSameAsMoves)
            possibleAttacks.Clear();

        possibleMoves.Clear();

        foreach (var moveDirection in possibleMovesDirections)
        {
            for (int i = 1; i < Mathf.Max(owner.ChessboardReference.GetGridSize().x, owner.ChessboardReference.GetGridSize().y); i++)
            {
                Vector2Int newPosition = moveDirection * i;

                if (owner.ChessboardReference.IsWithinBounds(owner.CurrentSquare.GetBoardPosition() + newPosition))
                {
                    ChessboardSquare targetSquare = owner.ChessboardReference.GetSquareAtPosition(owner.CurrentSquare.GetBoardPosition() + newPosition);

                    if (targetSquare.IsEmpty())
                    {
                        possibleMoves.Add(newPosition);
                    }
                    else
                    {
                        if (targetSquare.CurrentFigure.FigureSide != owner.FigureSide)
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

    public void MoveToSquare(ChessboardSquare square)
    {
        owner.AssignFigureToSquare(square);

        owner.transform.DOLocalMove(Vector2.zero, moveTweenDuration)
            .SetEase(Ease.Linear)
            .OnStart(() => owner.figureSFX.PlayMoveClip())
            .OnComplete(() =>
            {
                if (wholeLineMovement)
                    InitValidLineMovesAndAttacks();

                EndedPerform = true;
            });
    }

    public void AttackSquare(ChessboardSquare square)
    {
        ChessFigure attacker = owner;
        ChessFigure defender = square.CurrentFigure;

        var calculatedDamage = attacker.figureStatistics.GetStatisticValue(FigureStatistic.BASE_DAMAGE);

        defender.figureStatistics.ChangeStatistic(FigureStatistic.CURRENT_HEALTH, -calculatedDamage);

        if (defender.figureHealthSystem.IsDead)
        {
            MoveToSquare(square);
        }
        else
        {
            Vector3 attackerInitPosition = attacker.transform.position;
            attacker.figureSFX.PlayAttackClip();

            attacker.transform.DOMove(defender.transform.position, moveTweenDuration/2.0f).SetEase(Ease.Linear).OnComplete(() =>
            {
                Vector3 attackDirection = (defender.transform.position - attackerInitPosition).normalized;
                Vector3 knockbackPosition = defender.transform.position + attackDirection * knockbackForce;
                defender.figureSFX.PlayTakeDamageClip();

                defender.transform.DOMove(knockbackPosition, moveTweenDuration / 4f).SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    defender.transform.DOLocalMove(Vector2.zero, moveTweenDuration / 4f).SetEase(Ease.InQuad);
                });

                if (defender.figureType == ChessFigureType.KING)
                {
                    attacker.figureHealthSystem.ForceDie();
                    EndedPerform = true;
                }
                else
                {
                    attacker.transform.DOLocalMove(Vector2.zero, moveTweenDuration / 2.0f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        EndedPerform = true;
                    });
                }
            });
        }
    }

    public ChessboardSquare GetRandomSquareToMove()
    {
        List<ChessboardSquare> validMoves = new List<ChessboardSquare>();
        foreach (var move in possibleMoves)
        {
            Vector2Int targetPosition = owner.CurrentSquare.GetBoardPosition() + move;
            if (owner.ChessboardReference.IsWithinBounds(targetPosition))
            {
                ChessboardSquare targetSquare = owner.ChessboardReference.GetSquareAtPosition(targetPosition);
                if (targetSquare.IsEmpty())
                {
                    validMoves.Add(targetSquare);
                }
            }
        }

        if (validMoves.Count > 0)
        {
            ChessboardSquare bestSquareForNextTurn = null;
            float currentBiggestPriority = -1;

            foreach(var validMove in validMoves)
            {
                var listOfTargetsFromValidSquare = GetPossibleAttackSquaresFromSquare(validMove);

                if(listOfTargetsFromValidSquare.Count > 0)
                {
                    foreach(var targetSquare in listOfTargetsFromValidSquare)
                    {
                        var figureInTargetSquare = targetSquare.CurrentFigure;
                        float priorityOfTargetFigure = figureInTargetSquare.figureStatistics.GetStatisticValue(FigureStatistic.TARGETED_PRIORITY);
                        if(priorityOfTargetFigure >= currentBiggestPriority)
                        {
                            currentBiggestPriority = priorityOfTargetFigure;
                            bestSquareForNextTurn = validMove;
                        }
                    }
                }
            }

            if (bestSquareForNextTurn) return bestSquareForNextTurn;
            return validMoves[Random.Range(0, validMoves.Count)];
        }

        return null;
    }

    public ChessboardSquare GetBestSquareToAttack()
    {
        ChessboardSquare bestSquareToAttack = null;
        float currentHighestPriority = -1;

        foreach (var squarePos in possibleAttacks)
        {
            var targetPosition = owner.CurrentSquare.GetBoardPosition() + squarePos;
            ChessboardSquare square = owner.ChessboardReference.GetSquareAtPosition(targetPosition);
            if (!square) continue;
            if (square.IsEmpty() || square.CurrentFigure.FigureSide == owner.FigureSide) continue;

            if (square.CurrentFigure.figureStatistics.GetStatisticValue(FigureStatistic.TARGETED_PRIORITY) > currentHighestPriority)
            {
                currentHighestPriority = square.CurrentFigure.figureStatistics.GetStatisticValue(FigureStatistic.TARGETED_PRIORITY);
                bestSquareToAttack = square;
                continue;
            }
        }
        return bestSquareToAttack;
    }

    public void InvertMoves()
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

    public List<ChessboardSquare> GetPossibleAttackSquaresFromSquare(ChessboardSquare fromSquare)
    {
        List<ChessboardSquare> possibleAttackSquares = new List<ChessboardSquare>();
        Vector2Int fromPosition = fromSquare.GetBoardPosition();

        List<Vector2Int> attacks = attacksSameAsMoves ? possibleMoves : possibleAttacks;

        foreach (var attack in attacks)
        {
            Vector2Int targetPosition = fromPosition + attack;

            if (owner.ChessboardReference.IsWithinBounds(targetPosition))
            {
                ChessboardSquare targetSquare = owner.ChessboardReference.GetSquareAtPosition(targetPosition);

                if (!targetSquare.IsEmpty() && targetSquare.CurrentFigure.FigureSide != owner.FigureSide)
                {
                    possibleAttackSquares.Add(targetSquare);
                }
            }
        }

        return possibleAttackSquares;
    }
}