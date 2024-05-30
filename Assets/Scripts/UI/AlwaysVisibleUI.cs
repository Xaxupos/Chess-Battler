using System;
using TMPro;
using UnityEngine;

public class AlwaysVisibleUI : GameUI
{
    [Header("References")]
    public TMP_Text goldAmountText;
    public TMP_Text kingLives;

    private void OnEnable()
    {
        GoldManager.Instance.OnGoldAdded.AddListener(UpdateGoldText);
        GoldManager.Instance.OnGoldRemoved.AddListener(UpdateGoldText);
        ChessFigureHealthEvents.OnKingHealthChanged += UpdateKingHealthText;
    }

    private void OnDisable()
    {
        GoldManager.Instance.OnGoldAdded.RemoveListener(UpdateGoldText);
        GoldManager.Instance.OnGoldRemoved.RemoveListener(UpdateGoldText);
        ChessFigureHealthEvents.OnKingHealthChanged -= UpdateKingHealthText;
    }

    public void UpdateGoldText()
    {
        goldAmountText.text = GoldManager.Instance.currentGold.ToString();
    }

    private void UpdateKingHealthText(ChessFigure king)
    {
        var currentHpToSet = Mathf.Max(0, king.figureStatistics.GetStatisticValue(FigureStatistic.CURRENT_HEALTH));
        kingLives.text = $"{(int)currentHpToSet}/{king.figureStatistics.GetStatisticValue(FigureStatistic.MAX_HEALTH)}";
    }
}