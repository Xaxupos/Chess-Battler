using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesTooltipUI : MonoBehaviour
{
    private AbilityUIElement uiElement;

    [Header("Refereces")]
    public Image abilityIcon;
    public TMP_Text abilityName;
    public TMP_Text abilityDesc;
    public TMP_Text abilityCost;

    private void OnEnable()
    {
        FillTooltip();
    }

    public void InitTooltip(AbilityUIElement abilityElement)
    {
        uiElement = abilityElement;
    }

    public void FillTooltip()
    {
        var data = uiElement.ability;

        abilityIcon.sprite = data.abilityIcon;
        abilityName.text = data.abilityName;
        abilityDesc.text = data.abilityDesc;
        abilityCost.text = data.abilityUnlockCost.ToString();
    }

    public void ShowTooltip(bool show)
    {
        gameObject.SetActive(show);
    }
}