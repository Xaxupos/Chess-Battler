using UnityEngine;
using UnityEngine.Events;
using VInspector;

public class ChessFigureStatistics : MonoBehaviour
{
    [SerializeField] private SerializedDictionary<FigureStatistic, float> figureStatistics = new SerializedDictionary<FigureStatistic, float>();

    [Header("Events")]
    public UnityEvent<FigureStatistic> OnStatisticIncrease;
    public UnityEvent<FigureStatistic> OnStatisticDecrease;

    private void OnValidate()
    {
        if(figureStatistics.Count == 0)
        {
            figureStatistics.Add(FigureStatistic.INITIATIVE, 0);
            figureStatistics.Add(FigureStatistic.BASE_DAMAGE, 0);
            figureStatistics.Add(FigureStatistic.MAX_HEALTH, 0);
            figureStatistics.Add(FigureStatistic.CURRENT_HEALTH, 0);
            figureStatistics.Add(FigureStatistic.TARGETED_PRIORITY, 0);
        }
    }

    private void Awake()
    {
        figureStatistics[FigureStatistic.CURRENT_HEALTH] = figureStatistics[FigureStatistic.MAX_HEALTH];
    }

    public void ChangeStatistic(FigureStatistic statistic, float amount)
    {
        figureStatistics[statistic] += amount;

        if(amount > 0)
        {
            OnStatisticIncrease?.Invoke(statistic);

        }
        else if(amount < 0)
        {
            OnStatisticDecrease?.Invoke(statistic);
        }
    }


    public float GetStatisticValue(FigureStatistic statistic)
    {
        return figureStatistics[statistic];
    }
}