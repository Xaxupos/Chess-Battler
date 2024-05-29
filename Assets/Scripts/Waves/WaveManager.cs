using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VInspector;

public class WaveManager : MonoBehaviour
{
    [Tab("References")]
    public Chessboard chessboard;
    public CombatManager combatManager;
    public ChessFigureSpawner figureSpawner;

    [Tab("Wave Datas")]
    public SerializedDictionary<int, List<WaveTemplateData>> waveDatasForIndex;

    [Tab("Wave Preview")]
    public int currentWaveIndex = 0;
    public Wave currentWave;

    [Tab("Events")]
    public UnityEvent OnStartWave;
    public UnityEvent OnEndWave;

    public bool WaveInProgress { get; private set; }

    public void StartWave()
    {
        if (WaveInProgress) return;

        WaveInProgress = true;

        OnStartWave?.Invoke();
        SeedCurrentWave();
        DistributeWave();
        combatManager.StartCombat();
    }

    public void EndWave()
    {
        WaveInProgress = false;
        OnEndWave?.Invoke();

        currentWaveIndex++;
        if (currentWaveIndex >= waveDatasForIndex.Count)
            currentWaveIndex = waveDatasForIndex.Count - 1;

        StartCoroutine(combatManager.ResetWhiteFiguresPositions());
    }

    private void SeedCurrentWave()
    {
        var waveTemplateData = waveDatasForIndex[currentWaveIndex].GetRandom();
        var seededData = waveTemplateData.SeedWaveEnemies();
        currentWave.InitWave(seededData);
    }

    private void DistributeWave()
    {
        var upperHalf = chessboard.GetUpperHalf();

        List<ChessboardSquare> emptySquares = new List<ChessboardSquare>();

        foreach (var square in upperHalf)
        {
            if (square.IsEmpty())
            {
                emptySquares.Add(square);
            }
        }

        foreach (var figureType in currentWave.chessFigures)
        {
            if (emptySquares.Count > 0)
            {
                int randomIndex = Random.Range(0, emptySquares.Count);
                ChessboardSquare randomSquare = emptySquares[randomIndex];

                figureSpawner.SpawnPawn(randomSquare, figureType, ChessSide.BLACK);
                emptySquares.RemoveAt(randomIndex);
            }
        }
    }
}