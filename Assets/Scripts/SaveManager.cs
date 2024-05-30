using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VInspector;
using Newtonsoft.Json;

public class SaveManager : MonoBehaviour
{
    [Header("References")]
    public Chessboard chessboard;
    public CombatManager combatManager;
    public ChessFigureSpawner chessFigureSpawner;

    [Button(color = "Orange")]
    public void SavePlayerFigures()
    {
        List<FigureSaveData> savedDatas = new List<FigureSaveData>();

        foreach (var figure in combatManager.chessFigures[ChessSide.WHITE])
        {
            var figureSaveData = new FigureSaveData();

            figureSaveData.figureType = figure.figureType;
            figureSaveData.boardX = figure.CurrentSquare.GetBoardPosition().x;
            figureSaveData.boardY = figure.CurrentSquare.GetBoardPosition().y;
            figureSaveData.attack = figure.figureStatistics.GetStatisticValue(FigureStatistic.BASE_DAMAGE);
            figureSaveData.health = figure.figureStatistics.GetStatisticValue(FigureStatistic.CURRENT_HEALTH);

            savedDatas.Add(figureSaveData);
        }

        string json = JsonConvert.SerializeObject(savedDatas, Formatting.Indented);
        SaveToFile("playerData.json", json);
    }

    [Button(color = "Blue")]
    public void LoadAndSetupPlayerFigures()
    {
        CleanupCurrentSetup();
        var loadedFigureStructs = LoadSavedFigures("playerData.json");

        foreach(var fStruct in loadedFigureStructs)
        {
            var spawnedPawn = chessFigureSpawner.SpawnPawn(chessboard.GetSquareAtPosition(new Vector2Int(fStruct.boardX, fStruct.boardY)), (ChessFigureType)fStruct.figureType, ChessSide.WHITE);
            spawnedPawn.SetupBasedOnFigureStruct(fStruct);
            spawnedPawn.AssignFigureToSquare(chessboard.GetSquareAtPosition(spawnedPawn.InitSquarePos));
        }
    }

    private void SaveToFile(string fileName, string jsonData)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.Write(jsonData);
        }

        Debug.Log($"Data saved to {path}");
    }

    private List<FigureSaveData> LoadSavedFigures(string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);

        if (!File.Exists(path))
        {
            Debug.LogError($"File {fileName} does not exist.");
            return null;
        }

        string jsonData = File.ReadAllText(path);

        List<FigureSaveData> loadedData = JsonConvert.DeserializeObject<List<FigureSaveData>>(jsonData);

        if (loadedData == null)
        {
            Debug.LogError("Failed to deserialize JSON data.");
            return null;
        }

        return loadedData;
    }

    private void CleanupCurrentSetup()
    {
        foreach (var square in chessboard.ChessboardGrid)
        {
            square.ClearSquare();
        }

        foreach (var figuresList in combatManager.chessFigures.Values)
        {
            foreach (var figure in figuresList)
            {
                figure.ClearCurrentSquare();
            }
        }

        for (int i = combatManager.chessFigures[ChessSide.WHITE].Count - 1; i >= 0; i--)
        {
            var figure = combatManager.chessFigures[ChessSide.WHITE][i];
            Destroy(figure.gameObject);
        }

        combatManager.chessFigures[ChessSide.WHITE].Clear();
    }
}