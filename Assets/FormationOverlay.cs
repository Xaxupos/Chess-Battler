using UnityEngine;
using UnityEngine.EventSystems;

public class FormationOverlay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public void InitAndSetPosition(Vector3 worldPosition)
    {
        transform.position = worldPosition;
        transform.SetParent(FormationsManager.Instance.chessboard.transform, false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }
}