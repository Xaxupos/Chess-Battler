using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FormationOverlayChild : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private FormationOverlay overlay;

    private void Start()
    {
        overlay = GetComponentInParent<FormationOverlay>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GhostFigureManager.Instance.GhostFigureActive) return;
        if (FormationOverlayTooltip.Instance.TooltipOpened()) return;

        FormationOverlayTooltip.Instance.DisplayTooltip(overlay.fData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        FormationOverlayTooltip.Instance.HideTooltip();
    }
}