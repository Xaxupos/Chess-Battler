using UnityEngine;
using VInspector;

public class GhostFigureManager : MonoBehaviour
{
    [Header("References")]
    public SerializedDictionary<ChessFigureType, Transform> ghostFigures;

    private Transform currentGhostFigure;
    public bool GhostFigureActive { get; set; }
    public ChessFigureType GhostFigureTypeActive { get; set; }
    public static GhostFigureManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void ActivateGhostFigure(ChessFigureType figureType)
    {
        if (GhostFigureActive) return;

        GhostFigureActive = true;
        GhostFigureTypeActive = figureType;
        currentGhostFigure = ghostFigures[figureType];
        ghostFigures[figureType].gameObject.SetActive(true);
        currentGhostFigure.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (GhostFigureActive && currentGhostFigure != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.nearClipPlane;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            currentGhostFigure.position = new Vector3(worldPosition.x, worldPosition.y, currentGhostFigure.position.z);
        }
    }

    public void DropGhostFigure()
    {
        GhostFigureActive = false;
        currentGhostFigure.gameObject.SetActive(false);
        currentGhostFigure = null;
        ghostFigures[GhostFigureTypeActive].gameObject.SetActive(false);
    }
}