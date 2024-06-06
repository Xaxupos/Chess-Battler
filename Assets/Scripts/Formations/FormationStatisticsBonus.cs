using UnityEngine;
using VInspector;

[CreateAssetMenu(fileName = "New Formation Statistics Bonus", menuName = "Formations/Bonuses/Statistics Bonus")]
public class FormationStatisticsBonus : FormationBonus
{
    public SerializedDictionary<FigureStatistic, float> statisticsToChange;

    public override void ApplyBonus(ChessFigure target)
    {
        foreach (var statKVP in statisticsToChange)
        {
            target.figureStatistics.ChangeStatistic(statKVP.Key, statKVP.Value, true);
        }
    }

    public override void RevertBonus(ChessFigure target)
    {
        foreach (var statKVP in statisticsToChange)
        {
            target.figureStatistics.ChangeStatistic(statKVP.Key, -statKVP.Value, true);
        }
    }
}