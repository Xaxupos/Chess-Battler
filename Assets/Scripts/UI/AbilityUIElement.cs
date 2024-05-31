using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VInspector;

public class AbilityUIElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tab("Core")]
    public int buyCost = 10;
    public ChessFigureAbility ability;

    public bool defaultUnlocked = false;

    public List<AbilityUIElement> abilitiesToUnlock = new List<AbilityUIElement>();
    public List<AbilityUIElement> abilitiesToLock = new List<AbilityUIElement>();

    public UnityEvent OnUnlockSucess;
    public UnityEvent OnUnlockFail;

    [Tab("UI References")]
    public AbilitiesTooltipUI tooltip;
    public Image abilityIcon;
    public Button buyButton;
    public GameObject backgroundUnlocked;
    public GameObject backgroundConditions;

    private void OnValidate()
    {
        if (abilityIcon && ability)
        {
            abilityIcon.sprite = ability.abilityIcon;
            buyCost = ability.abilityUnlockCost;
        }
    }

    private void Awake()
    {
        buyButton.interactable = defaultUnlocked;
        tooltip.InitTooltip(this);
    }

    public void UnlockAbility()
    {
        backgroundConditions.SetActive(false);
        buyButton.interactable = true;
    }

    public void LockAbility()
    {
        backgroundConditions.SetActive(true);
        buyButton.interactable = false;
    }

    public void TryBuyAbility()
    {
        if (GoldManager.Instance.HasEnoughGold(buyCost))
        {
            if (!FiguresAbilitiesManager.Instance.IsUnlocked(ability.abilityEnum))
            {
                FiguresAbilitiesManager.Instance.UnlockAbility(ability.abilityEnum);

                foreach (var ability in abilitiesToUnlock)
                    ability.UnlockAbility();

                foreach (var ability in abilitiesToLock)
                    ability.LockAbility();

                backgroundUnlocked.SetActive(true);
                OnUnlockSucess?.Invoke();
                GoldManager.Instance.RemoveGold(buyCost);
            }
            else
            {
                OnUnlockFail?.Invoke();
            }
        }
        else
        {
            OnUnlockFail?.Invoke();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.ShowTooltip(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.ShowTooltip(false);
    }
}