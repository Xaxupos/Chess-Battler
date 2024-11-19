using UnityEngine;

public class FormationOverlay : MonoBehaviour
{
    public FormationData fData { get; set; }

    public void InitAndSetPosition(Vector3 worldPosition, FormationData formationData)
    {
        fData = formationData;
        transform.position = worldPosition;
        transform.SetParent(FormationsManager.Instance.chessboard.overlaysParent.transform, false);
    }

    public void OnPointerEnter()
    {
        if (GhostFigureManager.Instance.GhostFigureActive) return;
        if (FormationOverlayTooltip.Instance.TooltipOpened()) return;

        FormationOverlayTooltip.Instance.DisplayTooltip(fData);
    }

    public void OnPointerExit()
    {
        FormationOverlayTooltip.Instance.HideTooltip();
    }
}