using UnityEngine;
using UnityEngine.EventSystems;

public class FormationOverlay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public FormationData fData { get; set; }

    public void InitAndSetPosition(Vector3 worldPosition, FormationData formationData)
    {
        fData = formationData;
        transform.position = worldPosition;
        transform.SetParent(FormationsManager.Instance.chessboard.overlaysParent.transform, false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GhostFigureManager.Instance.GhostFigureActive) return;
        if (FormationOverlayTooltip.Instance.TooltipOpened()) return;

        FormationOverlayTooltip.Instance.DisplayTooltip(fData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        FormationOverlayTooltip.Instance.HideTooltip();
    }
}