using TMPro;
using UnityEngine;
using UnityEngine.Events;
using VInspector;

public class ChessFigureInvalidTimer : MonoBehaviour
{
    [Tab("Settings")]
    [Header("References")]
    public ChessFigure owner;
    public int maxInvalidCount = 3;
    [ReadOnly] public int invalidCount = 0;

    [Header("UI")]
    public TMP_Text invalidText;

    [Tab("Events")]
    public UnityEvent OnInvalidIncrease;
    public UnityEvent OnInvalidReset;

    public void IncreaseInvalid()
    {
        invalidCount++;
        invalidText.text = $"{invalidCount}/{maxInvalidCount}";

        OnInvalidIncrease?.Invoke();
        if(invalidCount >= maxInvalidCount)
        {
            owner.figureSFX.PlayInvalidDieClip();
            owner.figureHealthSystem.ForceDie();
        }
        else
        {
            owner.figureSFX.PlayInvalidIncreaseClip();
        }
    }

    public void ResetInvalid()
    {
        invalidText.text = "";
        invalidCount = 0;
    }
}