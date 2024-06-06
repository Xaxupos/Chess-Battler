using UnityEngine;
using VInspector;

public class FigureMoverManager : MonoBehaviour
{
    public Chessboard chessboard;
    public ChessFigureSpawner figureSpawner;

    [ReadOnly] public ChessFigure currentlyDraggedFigure;
    public float clickDelay = 0.1f;

    public static FigureMoverManager Instance;

    private Vector2Int cachedCurrentDraggedPos;
    private float lastClickTime;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void Update()
    {
        if (figureSpawner.combatManager.waveManager.WaveInProgress) return;
        if (Time.time - lastClickTime < clickDelay) return;

        if (Input.GetMouseButtonDown(0) || (Input.GetMouseButtonUp(0) && currentlyDraggedFigure))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                ChessboardSquare square = hit.collider.GetComponent<ChessboardSquare>();

                if (square)
                {
                    lastClickTime = Time.time;
                    if (currentlyDraggedFigure)
                    {
                        if (square.IsEmpty())
                        {
                            MoveFigure(currentlyDraggedFigure, square);
                            GhostFigureManager.Instance.DropGhostFigure();
                            currentlyDraggedFigure.gameObject.SetActive(true);
                            currentlyDraggedFigure = null;

                            FormationsManager.Instance.FullScan();
                        }
                        else
                        {
                            if (square.CurrentFigure.figureType == ChessFigureType.KING) return;

                            SwapWithFigure(square.CurrentFigure);
                            GhostFigureManager.Instance.DropGhostFigure();
                            currentlyDraggedFigure.gameObject.SetActive(true);
                            currentlyDraggedFigure = null;

                            FormationsManager.Instance.FullScan();
                        }
                    }
                    else
                    {
                        if (!square.IsEmpty())
                        {
                            if (square.CurrentFigure.figureType == ChessFigureType.KING) return;

                            currentlyDraggedFigure = square.CurrentFigure;

                            if(currentlyDraggedFigure.formationController.IsPartOfFormation())
                            {
                                currentlyDraggedFigure.formationController.ClearCurrentFormationAndRevertBonus();
                            }
                            cachedCurrentDraggedPos = currentlyDraggedFigure.CurrentSquare.GetBoardPosition();
                            square.ClearSquare();
                            currentlyDraggedFigure.ClearCurrentSquare();
                            GhostFigureManager.Instance.ActivateGhostFigure(currentlyDraggedFigure.figureType);

                            FormationsManager.Instance.FullScan();
                            currentlyDraggedFigure.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    public void MoveFigure(ChessFigure figure, ChessboardSquare newSquare)
    {
        figure.AssignFigureToSquare(newSquare, true);
    }

    public void SwapWithFigure(ChessFigure figureToSwapWith)
    {
        var currentDraggedPos = cachedCurrentDraggedPos;
        var secondFigurePos = figureToSwapWith.CurrentSquare.GetBoardPosition();

        if(figureToSwapWith.formationController.IsPartOfFormation())
        {
            figureToSwapWith.formationController.ClearCurrentFormationAndRevertBonus();
        }

        currentlyDraggedFigure.AssignFigureToSquare(chessboard.GetSquareAtPosition(secondFigurePos), true, false);
        figureToSwapWith.AssignFigureToSquare(chessboard.GetSquareAtPosition(currentDraggedPos), true, false);
    }
}
