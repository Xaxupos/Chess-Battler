using TMPro;
using UnityEngine;

public class FormationOverlayTooltip : MonoBehaviour
{
    public Vector3 offset;
    public TMP_Text formationName;
    public TMP_Text formationDesc;

    public static FormationOverlayTooltip Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public bool TooltipOpened()
    {
        return gameObject.activeSelf;
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            transform.position = Input.mousePosition + offset;
        }
    }

    public void DisplayTooltip(FormationData data)
    {
        gameObject.SetActive(true);

        formationName.text = data.formationName;
        formationDesc.text = data.formationDescription;
        GetComponent<CanvasGroup>().alpha = 1;
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
        GetComponent<CanvasGroup>().alpha = 0;
    }
}
