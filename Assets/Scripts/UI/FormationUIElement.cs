using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FormationUIElement : MonoBehaviour
{
    public TMP_Text formationDescription;
    public TMP_Text formationName;
    public Image formationIcon;

    public void InitFormationElement(FormationData formationData)
    {
        formationName.text = formationData.formationName;
        formationDescription.text = formationData.formationDescription;
        formationIcon.sprite = formationData.formationBoardVisual;
    }
}