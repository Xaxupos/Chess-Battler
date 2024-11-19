using System;
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

        owner.figureStatistics.OnStatisticIncrease.AddListener(HandleStatsUI);
        owner.figureStatistics.OnStatisticDecrease.AddListener(HandleStatsUI);
    }

    private void OnDestroy()
    {
        owner.figureStatistics.OnStatisticIncrease.RemoveListener(HandleStatsUI);
        owner.figureStatistics.OnStatisticDecrease.RemoveListener(HandleStatsUI);
    }

    private void HandleStatsUI(FigureStatistic stat)
    {
        switch (stat)
        {
            case FigureStatistic.CURRENT_HEALTH:
                SetHealthText();
                break;
            case FigureStatistic.MAX_HEALTH:
                SetHealthText();
                break;
            case FigureStatistic.BASE_DAMAGE:
                SetDamageText();
                break;
        }
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