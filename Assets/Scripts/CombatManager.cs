using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VInspector;

public class CombatManager : MonoBehaviour
{
    [Header("References")]
    public SerializedDictionary<ChessSide, List<ChessFigure>> chessFigures = new SerializedDictionary<ChessSide, List<ChessFigure>>();

    [Header("Settings")]
    public float delayBeetwenMoves = 1.25f;

    private Queue<ChessFigure> figuresQueue;
    public event Action<Queue<ChessFigure>> OnTurnQueueUpdated;

    private void Awake()
    {
        ChessFigureHealthEvents.OnAnyFigureDie += HandleFigureDie;
    }

    private void OnDestroy()
    {
        ChessFigureHealthEvents.OnAnyFigureDie -= HandleFigureDie;
    }

    [Button]
    public void StartCombat()
    {
        InitQueue();
        StartCoroutine(HandleTurns());
    }

    public void AssignFigure(ChessFigure chessFigure)
    {
        if (chessFigures[chessFigure.GetFigureSide].Contains(chessFigure)) return;

        chessFigures[chessFigure.GetFigureSide].Add(chessFigure);
    }

    private IEnumerator HandleTurns()
    {
        WaitForSeconds turnDelayFull = new WaitForSeconds(delayBeetwenMoves);
        WaitForSeconds turnDelayZero = new WaitForSeconds(0);

        while (chessFigures[ChessSide.WHITE].Count > 0 && chessFigures[ChessSide.BLACK].Count > 0)
        {
            var currentFigure = figuresQueue.Peek();
            currentFigure.PerformTurn();

            ResetFigureTurn();

            if(currentFigure.DidActionThisTurn)
                yield return turnDelayFull;
            else
                yield return turnDelayZero;
        }
    }

    private void HandleFigureDie(ChessFigure figure)
    {
        List<ChessFigure> figuresList = figuresQueue.ToList();
        figuresList.Remove(figure);
        chessFigures[figure.GetFigureSide].Remove(figure);

        figuresQueue = new Queue<ChessFigure>(figuresList);
        OnTurnQueueUpdated?.Invoke(figuresQueue);
    }

    private void ResetFigureTurn()
    {
        var thisTurnFigure = figuresQueue.Peek();
        figuresQueue.Dequeue();
        figuresQueue.Enqueue(thisTurnFigure);

        OnTurnQueueUpdated?.Invoke(figuresQueue);
    }

    private void InitQueue()
    {
        List<ChessFigure> allFigures = new List<ChessFigure>(chessFigures[ChessSide.WHITE]);
        allFigures.AddRange(new List<ChessFigure>(chessFigures[ChessSide.BLACK]));

        allFigures.Sort((a, b) =>
            b.GetComponentInChildren<ChessFigureStatistics>().GetStatisticValue(FigureStatistic.INITIATIVE)
            .CompareTo(a.GetComponentInChildren<ChessFigureStatistics>().GetStatisticValue(FigureStatistic.INITIATIVE)));

        figuresQueue = new Queue<ChessFigure>(allFigures);
        OnTurnQueueUpdated?.Invoke(figuresQueue);
    }
}