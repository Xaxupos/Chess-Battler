using System;
using UnityEngine;
using UnityEngine.Events;

public class ChessFigureHealthEvents : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ChessFigure figure;
    [SerializeField] private ChessFigureStatistics figureStatistics;

    [Header("Events")]
    public UnityEvent OnTakeDamage;
    public UnityEvent OnHeal;
    public UnityEvent OnDie;
    public static Action<ChessFigure> OnAnyFigureDie;

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

    private void HandleHealthIncrease(FigureStatistic stat)
    {
        if(stat == FigureStatistic.CURRENT_HEALTH)
        {
            OnHeal?.Invoke();
        }
    }

    private void HandleHealthDecrease(FigureStatistic stat)
    {
        if (stat == FigureStatistic.CURRENT_HEALTH)
        {
            OnTakeDamage?.Invoke();

            if(figureStatistics.GetStatisticValue(stat) <=0)
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
        Destroy(figure.gameObject);
    }
}