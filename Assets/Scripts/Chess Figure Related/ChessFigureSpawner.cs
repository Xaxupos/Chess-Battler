using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class ChessFigureSpawner : MonoBehaviour
{
    [Header("References")]
    public CombatManager combatManager;
    public SerializedDictionary<ChessFigureType, ChessFigure> figureMap;

    public bool FigureJustBought { get; set; }
    private ChessFigureType figureToSpawn;
    private ChessSide sideToSpawn;


    public static ChessFigureSpawner Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void Start()
    {
        SpawnPlayerKing();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && FigureJustBought)
        {
            FormationOverlayTooltip.Instance.HideTooltip();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                ChessboardSquare chessSquare = hit.collider.GetComponent<ChessboardSquare>();
                if (chessSquare != null && chessSquare.IsEmpty() && chessSquare.Chessboard.GetLowerHalf().Contains(chessSquare))
                {
                    var pawn = SpawnPlayerPawn(chessSquare);
                    GameEffectsDatabase.Instance.PlaySFX(pawn.figureSFX.spawnType);
                    GhostFigureManager.Instance.DropGhostFigure();
                    FigureJustBought = false;
                    FormationsManager.Instance.FullScan();
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

    public ChessFigure SpawnPlayerPawn(ChessboardSquare square)
    {
        ChessFigure pawn = Instantiate(figureMap[figureToSpawn], square.transform.position, Quaternion.identity);
        pawn.InitChessFigure(square, sideToSpawn);
        combatManager.AssignFigure(pawn);

        return pawn;
    }

    public ChessFigure SpawnPawn(ChessboardSquare square, ChessFigureType figureType, ChessSide forcedSide)
    {
        ChessFigure pawn = Instantiate(figureMap[figureType], square.transform.position, Quaternion.identity);
        pawn.InitChessFigure(square, forcedSide);
        combatManager.AssignFigure(pawn);
        return pawn;
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
        var king = SpawnPawn(kingSquare, ChessFigureType.KING, ChessSide.WHITE);
        GameUIManager.Instance.alwaysVisibleUI.GetComponent<AlwaysVisibleUI>().UpdateKingHealthText(king);
    }
}