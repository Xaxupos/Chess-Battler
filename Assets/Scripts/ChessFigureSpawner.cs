using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class ChessFigureSpawner : MonoBehaviour
{
    [Header("References")]
    public AudioSource playerSpawnSfx;
    public CombatManager combatManager;
    public SerializedDictionary<ChessFigureType, ChessFigure> figureMap;

    public bool FigureJustBought { get; set; }
    private ChessFigureType figureToSpawn;
    private ChessSide sideToSpawn;

    private void Start()
    {
        SpawnPlayerKing();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && FigureJustBought)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                ChessboardSquare chessSquare = hit.collider.GetComponent<ChessboardSquare>();
                if (chessSquare != null && chessSquare.IsEmpty() && chessSquare.Chessboard.GetLowerHalf().Contains(chessSquare))
                {
                    playerSpawnSfx.Play();
                    SpawnPlayerPawn(chessSquare);
                    FigureJustBought = false;
                }
            }
        }
    }

    public void SetFigureToSpawn(int enumIndex)
    {
        figureToSpawn = (ChessFigureType)enumIndex;
    }

    public void SetSideToSpawn(int enumIndex)
    {
        sideToSpawn = (ChessSide)enumIndex;
    }

    public void SpawnPlayerPawn(ChessboardSquare square)
    {
        ChessFigure pawn = Instantiate(figureMap[figureToSpawn], square.transform.position, Quaternion.identity);
        pawn.InitChessFigure(square, sideToSpawn);
        combatManager.AssignFigure(pawn);
    }

    public void SpawnPawn(ChessboardSquare square, ChessFigureType figureType, ChessSide forcedSide)
    {
        ChessFigure pawn = Instantiate(figureMap[figureType], square.transform.position, Quaternion.identity);
        pawn.InitChessFigure(square, forcedSide);
        combatManager.AssignFigure(pawn);
    }

    private void SpawnPlayerKing()
    {
        var lowerHalf = combatManager.chessboard.GetLowerHalf();

        List<ChessboardSquare> firstRowSquares = new List<ChessboardSquare>();
        foreach (var square in lowerHalf)
        {
            if (square.GetBoardPosition().y == 0)
            {
                firstRowSquares.Add(square);
            }
        }

        ChessboardSquare kingSquare = firstRowSquares.GetRandom();
        SpawnPawn(kingSquare, ChessFigureType.KING, ChessSide.WHITE);
    }
}