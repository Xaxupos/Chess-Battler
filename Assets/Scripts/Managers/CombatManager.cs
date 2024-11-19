using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using VInspector;

public class CombatManager : MonoBehaviour
{
    [Header("References")]
    public Chessboard chessboard;
    public WaveManager waveManager;
    public LoseGameManager loseGameManager;
    public SerializedDictionary<ChessSide, List<ChessFigure>> chessFigures = new SerializedDictionary<ChessSide, List<ChessFigure>>();

    [Header("Settings")]
    public float delayBeetwenMoves = 1.25f;

    public Queue<ChessFigure> figuresQueue;
    public event Action<Queue<ChessFigure>> OnTurnQueueUpdated;

    private void Awake()
    {
        ChessFigureHealthEvents.OnAnyFigureDie += HandleFigureDie;
    }

    private void OnDestroy()
    {
        ChessFigureHealthEvents.OnAnyFigureDie -= HandleFigureDie;
    }

    public void StartCombat()
    {
        InitQueue();
        StartCoroutine(HandleTurns());
        chessboard.EnableOverlaysParent(false);
    }

    public void AssignFigure(ChessFigure chessFigure)
    {
        if (chessFigures[chessFigure.FigureSide].Contains(chessFigure)) return;

        chessFigures[chessFigure.FigureSide].Add(chessFigure);
    }

    public IEnumerator ResetWhiteFiguresPositions()
    {
        foreach(var square in chessboard.ChessboardGrid)
        {
            square.ClearSquare();
        }

        foreach(var figuresList in chessFigures.Values)
        {
            foreach(var figure in figuresList)
            {
                figure.ClearCurrentSquare();
            }
        }

        yield return new WaitForSeconds(0.75f);

        var newX = waveManager.waveDatasForIndex[waveManager.currentWaveIndex].GetRandom().chessboardXSize;
        var newY = waveManager.waveDatasForIndex[waveManager.currentWaveIndex].GetRandom().chessboardYSize;

        if(newX != chessboard.GetGridSize().x || newY != chessboard.GetGridSize().y)
            chessboard.ResizeGrid(newX, newY);

        foreach (var whiteFigure in chessFigures[ChessSide.WHITE])
        {
            //ReassignFigureSquareReferences();
            whiteFigure.figureInvalidTimer.ResetInvalid();
            whiteFigure.AssignFigureToSquare(chessboard.GetSquareAtPosition(whiteFigure.InitSquarePos), true);
        }

        yield return new WaitForEndOfFrame();

        FormationsManager.Instance.FullScan();

        yield return new WaitForEndOfFrame();
        chessboard.EnableOverlaysParent(true);
    }

    private IEnumerator HandleTurns()
    {
        WaitForSeconds turnDelayFull = new WaitForSeconds(delayBeetwenMoves);
        WaitForSeconds turnDelayZero = new WaitForSeconds(0.5f);

        GameEffectsDatabase.Instance.PlaySFX(ActionType.COMBAT_START);
        yield return new WaitForSeconds(2);

        while (chessFigures[ChessSide.BLACK].Count > 0)
        {
            if (!waveManager.WaveInProgress)
            {
                StopAllCoroutines();
                yield break;
            }

            var currentFigure = figuresQueue.Dequeue();

            if (!currentFigure)
            {
                continue;
            }

            currentFigure.PerformTurn();

            //Died during his turn
            if (!currentFigure)
                continue;

            yield return new WaitUntil(() => currentFigure.figureBrain.EndedPerform);

            ResetFigureTurn(currentFigure);

            if (currentFigure.figureBrain.DidActionThisTurn)
            {
                currentFigure.figureInvalidTimer.ResetInvalid();
                yield return turnDelayFull;
            }
            else
            {
                currentFigure.figureInvalidTimer.IncreaseInvalid();
                yield return turnDelayZero;
            }
        }
    }

    private void HandleFigureDie(ChessFigure figure)
    {
        if (figure.figureType == ChessFigureType.KING)
        {
            loseGameManager.LoseGame();
            StopAllCoroutines();
            return;
        }

        List<ChessFigure> figuresList = figuresQueue.ToList();
        figuresList.Remove(figure);
        chessFigures[figure.FigureSide].Remove(figure);

        if (chessFigures[ChessSide.BLACK].Count == 0)
        {
            waveManager.EndWave();
            StopAllCoroutines();
            return;
        }

        figuresQueue = new Queue<ChessFigure>(figuresList);
        OnTurnQueueUpdated?.Invoke(figuresQueue);
    }

    private void ResetFigureTurn(ChessFigure currentFigure)
    {
        figuresQueue.Enqueue(currentFigure);
        OnTurnQueueUpdated?.Invoke(figuresQueue);
    }

    private void InitQueue()
    {
        List<ChessFigure> allFigures = new List<ChessFigure>();
        foreach(var figure in chessFigures[ChessSide.BLACK])
        {
            if (figure.figureType == ChessFigureType.KING) continue;
            allFigures.Add(figure);
        }
        foreach(var figure in chessFigures[ChessSide.WHITE])
        {
            if (figure.figureType == ChessFigureType.KING) continue;
            allFigures.Add(figure);
        }

        allFigures.Shuffle();

        allFigures.Sort((a, b) =>
            b.figureStatistics.GetStatisticValue(FigureStatistic.INITIATIVE)
            .CompareTo(a.figureStatistics.GetStatisticValue(FigureStatistic.INITIATIVE)));

        figuresQueue = new Queue<ChessFigure>(allFigures);
        OnTurnQueueUpdated?.Invoke(figuresQueue);
    }

    private void ReassignFigureSquareReferences()
    {
        Vector2Int initGridSize = chessboard.InitGridSize;
        Vector2Int currentGridSize = chessboard.GetGridSize();

        foreach (var whiteFigure in chessFigures[ChessSide.WHITE])
        {
            Vector2Int initSquarePos = whiteFigure.InitSquarePos;

            float scaleX = (float)currentGridSize.x / initGridSize.x;
            float scaleY = (float)currentGridSize.y / initGridSize.y;

            int newX = Mathf.RoundToInt(initSquarePos.x * scaleX);
            int newY = Mathf.RoundToInt(initSquarePos.y * scaleY);

            whiteFigure.InitSquarePos = new Vector2Int(newX, newY);
        }
    }
}