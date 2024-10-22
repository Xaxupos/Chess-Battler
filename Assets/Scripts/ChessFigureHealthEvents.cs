using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ChessFigureHealthEvents : MonoBehaviour
{
    [Header("References")]
    public ChessFigure figure;
    public ChessFigureStatistics figureStatistics;

    [Header("Events")]
    public UnityEvent OnTakeDamage;
    public UnityEvent OnHeal;
    public UnityEvent OnDie;
    public static Action<ChessFigure> OnAnyFigureDie;
    public static Action<ChessFigure> OnKingHealthChanged;

    public bool IsDead { get; set; }

    private void Awake()
    {
        figureStatistics.OnStatisticIncrease.AddListener(HandleHealthIncrease);
        figureStatistics.OnStatisticDecrease.AddListener(HandleHealthDecrease);
    }

    private void OnDestroy()
    {
        figureStatistics.OnStatisticIncrease.RemoveListener(HandleHealthIncrease);
        figureStatistics.OnStatisticDecrease.RemoveListener(HandleHealthDecrease);
    }

    public void ForceDie()
    {
        Die();
    }

    private void HandleHealthIncrease(FigureStatistic stat)
    {
        if(stat == FigureStatistic.CURRENT_HEALTH)
        {
            OnHeal?.Invoke();
            if(figure.figureType == ChessFigureType.KING && figure.FigureSide == ChessSide.WHITE)
            {
                OnKingHealthChanged?.Invoke(figure);
            }
        }
    }

    private void HandleHealthDecrease(FigureStatistic stat)
    {
        if (stat == FigureStatistic.CURRENT_HEALTH)
        {
            OnTakeDamage?.Invoke();
            if (figure.figureType == ChessFigureType.KING && figure.FigureSide == ChessSide.WHITE)
            {
                OnKingHealthChanged?.Invoke(figure);
            }

            if (figureStatistics.GetStatisticValue(stat) <=0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        OnAnyFigureDie?.Invoke(figure);
        OnDie?.Invoke();
        IsDead = true;
        figure.CurrentSquare.ClearSquare();
        StartCoroutine(DelayedDestroy());
    }

    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        Destroy(figure.gameObject);
    }
}