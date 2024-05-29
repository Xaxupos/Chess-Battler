using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VInspector;

[CreateAssetMenu(fileName = "Wave Data", menuName = "Waves/New Wave Data")]
public class WaveTemplateData : ScriptableObject
{
    public int minimumEnemies = 1;
    public int maximumEnemies = 3;
    public SerializedDictionary<ChessFigureType, int> minimumFigureDefine;
    public SerializedDictionary<ChessFigureType, int> maximumFigureDefine;
    public SerializedDictionary<ChessFigureType, int> spawnWeightChance;

    public SerializedDictionary<ChessFigureType, int> SeedWaveEnemies()
    {
        SerializedDictionary<ChessFigureType, int> seededWave = new SerializedDictionary<ChessFigureType, int>();
        int totalEnemies = Random.Range(minimumEnemies, maximumEnemies + 1);
        int enemyCount = 0;

        foreach (var kvp in minimumFigureDefine)
        {
            seededWave[kvp.Key] = kvp.Value;
            enemyCount += kvp.Value;
        }

        List<ChessFigureType> figureTypes = new List<ChessFigureType>(spawnWeightChance.Keys);
        List<int> weights = new List<int>(spawnWeightChance.Values);

        int totalWeight = weights.Sum();

        while (enemyCount < totalEnemies && figureTypes.Count > 0)
        {
            ChessFigureType selectedFigure = GetRandomFigureType(figureTypes, weights, totalWeight);

            if (!seededWave.ContainsKey(selectedFigure))
            {
                seededWave[selectedFigure] = 0;
            }

            if (seededWave[selectedFigure] < maximumFigureDefine[selectedFigure])
            {
                seededWave[selectedFigure]++;
                enemyCount++;
            }
            else
            {
                totalWeight -= spawnWeightChance[selectedFigure];
                figureTypes.Remove(selectedFigure);
            }
        }

        return seededWave;
    }

    private ChessFigureType GetRandomFigureType(List<ChessFigureType> figureTypes, List<int> weights, int totalWeight)
    {
        int randomValue = Random.Range(0, totalWeight);
        int currentWeight = 0;

        for (int i = 0; i < figureTypes.Count; i++)
        {
            currentWeight += weights[i];
            if (randomValue < currentWeight)
            {
                return figureTypes[i];
            }
        }
        return ChessFigureType.PAWN;
    }
}