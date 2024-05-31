using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class AbilityPanelUI : MonoBehaviour
{
    public List<AbilitiesTopNavbarElement> abilitiesNavbarElements;
    [ReadOnly] public AbilitiesContentPanel currentActiveContent;

    private void Awake()
    {
        foreach(var element in abilitiesNavbarElements)
        {
            element.InitElement(this);
        }
    }

    private void OnEnable()
    {
        if (currentActiveContent == null)
            OpenContent(abilitiesNavbarElements[0].content);
    }

    public void OpenHidePanel()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void OpenContent(AbilitiesContentPanel content)
    {
        if (currentActiveContent) currentActiveContent.CloseContent();
        currentActiveContent = content;
        currentActiveContent.OpenContent();
    }
}