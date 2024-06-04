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
    public GameObject lockIcon;
    public TMP_Text invalidText;

    [Tab("Events")]
    public UnityEvent OnInvalidIncrease;
    public UnityEvent OnInvalidReset;

    public void IncreaseInvalid()
    {
        invalidCount++;
        lockIcon.SetActive(true);
        invalidText.text = $"{invalidCount}";

        OnInvalidIncrease?.Invoke();
        if(invalidCount >= maxInvalidCount)
        {
            GameEffectsDatabase.Instance.PlaySFX(owner.figureSFX.invalidDieType);
            GameEffectsDatabase.Instance.PlayVFX(owner.figureVFX.dieType, owner.transform.position);
            owner.figureHealthSystem.ForceDie();
        }
        else
        {
            GameEffectsDatabase.Instance.PlaySFX(owner.figureSFX.invalidIncreaseType);
        }
    }

    public void ResetInvalid()
    {
        invalidText.text = "";
        invalidCount = 0;

        if(lockIcon)
            lockIcon.SetActive(false);
    }
}