using TMPro;
using UnityEngine;

public class ChessFigureUI : MonoBehaviour
{
    [Header("References")]
    public ChessFigure owner;
    public TMP_Text attackText;
    public TMP_Text healthText;

    private void Start()
    {
        SetHealthText();
        SetDamageText();
    }

    public void SetHealthText()
    {
        healthText.text = owner.figureStatistics.GetStatisticValue(FigureStatistic.CURRENT_HEALTH).ToString();
    }

    public void SetDamageText()
    {
        attackText.text = owner.figureStatistics.GetStatisticValue(FigureStatistic.BASE_DAMAGE).ToString();
    }
}