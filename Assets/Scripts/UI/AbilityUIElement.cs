using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AbilityUIElement : MonoBehaviour
{
    public int buyCost = 10;
    public AbilitiesEnum ability;

    public bool defaultUnlocked = false;
    public Button buyButton;
    public GameObject backgroundUnlocked;
    public GameObject backgroundConditions;

    public List<AbilityUIElement> abilitiesToUnlock = new List<AbilityUIElement>(); 
    public List<AbilityUIElement> abilitiesToLock = new List<AbilityUIElement>(); 

    public UnityEvent OnUnlockSucess;
    public UnityEvent OnUnlockFail;

    private void Awake()
    {
        buyButton.interactable = defaultUnlocked;
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
        if(GoldManager.Instance.HasEnoughGold(buyCost))
        {
            if(!FiguresAbilitiesManager.Instance.IsUnlocked(ability))
            {
                FiguresAbilitiesManager.Instance.UnlockAbility(ability);

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
}