using UnityEngine;

public class AbilitiesTopNavbarElement : MonoBehaviour
{
    public GameObject selectedSprite;
    public bool isOpened = false;
    public AbilitiesContentPanel content;

    private AbilityPanelUI abilityPanelUI;

    public void InitElement(AbilityPanelUI abilityPanel)
    {
        abilityPanelUI = abilityPanel;
    }

    public void TryOpenContent()
    {
        if (isOpened) return;

        abilityPanelUI.OpenContent(content);
    }
}