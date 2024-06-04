using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
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
    public List<Vector2Int> aoeAttacks = new List<Vector2Int>();

    [Tab("Values")]
    public float attackTimes = 1;
    public float aoeDamageMultiplier = 0.25f;
    public float moveTweenDuration = 0.4f;
    public float knockbackForce = 0.1f;

    public bool DidActionThisTurn { get; private set; }
    public bool EndedPerform { get; private set; }
    private int currentAttackTimes = 1;

    public void PerformBestAction()
    {
        DidActionThisTurn = false;
        EndedPerform = false;

        if (wholeLineMovement)
            InitValidLineMovesAndAttacks();

        ChessboardSquare squareToAttack = GetBestSquareToAttack();

        if (squareToAttack)
        {
            currentAttackTimes = 1;
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
            .OnStart(() =>
            {
                GameEffectsDatabase.Instance.PlaySFX(owner.figureSFX.moveType);
            })
            .OnComplete(() =>
            {
                if (wholeLineMovement)
                    InitValidLineMovesAndAttacks();

                EndedPerform = true;
            });
    }

    public void AttackSquare(ChessboardSquare square)
    {
        if (currentAttackTimes > attackTimes)
        {
            EndedPerform = true;
        }

        ChessFigure attacker = owner;
        ChessFigure defender = square.CurrentFigure;

        var calculatedDamage = attacker.figureStatistics.GetStatisticValue(FigureStatistic.BASE_DAMAGE);

        defender.figureStatistics.ChangeStatistic(FigureStatistic.CURRENT_HEALTH, -calculatedDamage);

        HandleAOEDamage(square, calculatedDamage);

        if (defender.figureHealthSystem.IsDead)
        {
            GameEffectsDatabase.Instance.PlaySFX(defender.figureSFX.dieType);
            GameEffectsDatabase.Instance.PlayVFX(defender.figureVFX.dieType, defender.transform.position);
            MoveToSquare(square);
        }
        else
        {
            Vector3 attackerInitPosition = attacker.transform.position;
            GameEffectsDatabase.Instance.PlaySFX(attacker.figureSFX.attackType);

            attacker.transform.DOMove(defender.transform.position, moveTweenDuration / 2.0f).SetEase(Ease.Linear).OnComplete(() =>
            {
                GameEffectsDatabase.Instance.PlaySFX(defender.figureSFX.takeDamageType);
                GameEffectsDatabase.Instance.PlayVFX(defender.figureVFX.takeDamageType, defender.transform.position);

                HandleKnockback(defender, attackerInitPosition);

                if (defender.figureType == ChessFigureType.KING)
                {
                    attacker.figureHealthSystem.ForceDie();
                    EndedPerform = true;
                }
                else
                {
                    attacker.transform.DOLocalMove(Vector2.zero, moveTweenDuration / 2.0f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        if (currentAttackTimes < attackTimes)
                        {
                            HandleExtraAttacks(square);
                        }
                        else
                        {
                            EndedPerform = true;
                        }
                    });
                }
            });
        }
    }

    private void HandleAOEDamage(ChessboardSquare square, float calculatedDamage)
    {
        bool aoeAllowed = true;
        ActionType sfxType = ActionType.UNIVERSAL_TAKE_DAMAGE;
        ActionType vfxType = ActionType.UNIVERSAL_TAKE_DAMAGE;
        

        if(owner.figureAbilities.IsAbilityUnlocked(AbilitiesEnum.KNIGHT_BOMB_AOE))
        {
            float randomValue = Random.value; //0 to 1
            float abilityTriggerChance = owner.figureAbilities.GetTriggerChance(AbilitiesEnum.KNIGHT_BOMB_AOE); //0 to 1

            sfxType = ActionType.KNIGHT_BOMB_ABILITY;
            vfxType = ActionType.KNIGHT_BOMB_ABILITY;
            aoeAllowed = randomValue <= abilityTriggerChance;
        }

        if (!aoeAllowed) return;

        if (aoeAttacks.Count > 0)
        {
            foreach (var aoe in aoeAttacks)
            {
                var squareToAttack = square.Chessboard.GetSquareAtPosition(square.GetBoardPosition() + aoe);
                if (!squareToAttack) continue;
                if (squareToAttack.IsEmpty()) continue;
                if (squareToAttack.CurrentFigure.FigureSide == owner.FigureSide) continue;

                float aoeDamage = calculatedDamage / 2;
                squareToAttack.CurrentFigure.figureStatistics.ChangeStatistic(FigureStatistic.CURRENT_HEALTH, -aoeDamage);

                if (!squareToAttack || !squareToAttack.CurrentFigure || !squareToAttack.CurrentFigure.figureHealthSystem) continue;

                GameEffectsDatabase.Instance.PlaySFX(sfxType);
                if (squareToAttack.CurrentFigure.figureHealthSystem.IsDead)
                {
                    GameEffectsDatabase.Instance.PlayVFX(squareToAttack.CurrentFigure.figureVFX.dieType, squareToAttack.CurrentFigure.transform.position);
                }
                else
                {
                    GameEffectsDatabase.Instance.PlayVFX(vfxType, squareToAttack.CurrentFigure.transform.position);
                }
            }
        }
    }

    private void HandleExtraAttacks(ChessboardSquare square)
    {
        float additionalAttackChance = 1.0f;
        float randomValue = Random.value;

        if (owner.figureAbilities.IsAbilityUnlocked(AbilitiesEnum.PAWN_DOUBLE_ATTACK))
        {
            additionalAttackChance = owner.figureAbilities.GetTriggerChance(AbilitiesEnum.PAWN_DOUBLE_ATTACK);
            if (owner.figureAbilities.IsAbilityUnlocked(AbilitiesEnum.PAWN_PRECISE_DOUBLE_ATTACK))
                additionalAttackChance = owner.figureAbilities.GetTriggerChance(AbilitiesEnum.PAWN_PRECISE_DOUBLE_ATTACK);
        }

        currentAttackTimes++;

        if (randomValue <= additionalAttackChance)
        {
            AttackSquare(square);
        }
        else
        {
            if (currentAttackTimes > attackTimes)
                EndedPerform = true;

            while(currentAttackTimes <= attackTimes)
            {
                HandleExtraAttacks(square);
            }
        }
    }

    private void HandleKnockback(ChessFigure defender, Vector3 attackerInitPosition)
    {
        Vector3 attackDirection = (defender.transform.position - attackerInitPosition).normalized;
        Vector3 knockbackPosition = defender.transform.position + attackDirection * knockbackForce;

        defender.transform.DOMove(knockbackPosition, moveTweenDuration / 4f).SetEase(Ease.OutQuad)
        .OnComplete(() =>
        {
            defender.transform.DOLocalMove(Vector2.zero, moveTweenDuration / 4f).SetEase(Ease.InQuad);
        });
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

        List<ChessboardSquare> killableSquares = new List<ChessboardSquare>();

        foreach (var squarePos in possibleAttacks)
        {
            var targetPosition = owner.CurrentSquare.GetBoardPosition() + squarePos;
            ChessboardSquare square = owner.ChessboardReference.GetSquareAtPosition(targetPosition);
            if (!square) continue;
            if (square.IsEmpty() || square.CurrentFigure.FigureSide == owner.FigureSide || square.CurrentFigure.figureHealthSystem.IsDead) continue;

            if(square.CurrentFigure.figureType == ChessFigureType.KING)
            {
                return square;
            }

            if(owner.figureStatistics.GetStatisticValue(FigureStatistic.BASE_DAMAGE) >= square.CurrentFigure.figureStatistics.GetStatisticValue(FigureStatistic.CURRENT_HEALTH))
            {
                killableSquares.Add(square);
            }

            if(killableSquares.Count == 0)
            {
                if (square.CurrentFigure.figureStatistics.GetStatisticValue(FigureStatistic.TARGETED_PRIORITY) > currentHighestPriority)
                {
                    currentHighestPriority = square.CurrentFigure.figureStatistics.GetStatisticValue(FigureStatistic.TARGETED_PRIORITY);
                    bestSquareToAttack = square;
                    continue;
                }
            }
            else
            {
                foreach(var killableSquare in killableSquares)
                {
                    if (killableSquare.CurrentFigure.figureStatistics.GetStatisticValue(FigureStatistic.TARGETED_PRIORITY) > currentHighestPriority)
                    {
                        currentHighestPriority = killableSquare.CurrentFigure.figureStatistics.GetStatisticValue(FigureStatistic.TARGETED_PRIORITY);
                        bestSquareToAttack = killableSquare;
                        continue;
                    }
                }
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