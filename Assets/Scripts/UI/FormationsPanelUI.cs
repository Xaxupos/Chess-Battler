using System.Collections.Generic;
using VInspector;
using UnityEngine;

public class FormationsPanelUI : MonoBehaviour
{
    public FormationUIElement[] formationUIElements;

    private void Start()
    {
        foreach(var  formationUIElement in formationUIElements)
        {
            formationUIElement.gameObject.SetActive(false);
        }

        for(int i = 0; i < FormationsManager.Instance.allGameFormations.Count; i++)
        {
            formationUIElements[i].gameObject.SetActive(true);
            formationUIElements[i].InitFormationElement(FormationsManager.Instance.allGameFormations[i]);
        }
    }

    public void OpenHidePanel()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}