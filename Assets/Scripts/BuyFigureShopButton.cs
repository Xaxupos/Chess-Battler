using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyFigureShopButton : MonoBehaviour
{
    [Header("References")]
    public Button button;
    public TMP_Text goldAmounText;
    public AudioSource buySFX;
    public AudioSource buyInvalidSFX;

    [Header("Settings")]
    public ChessFigureType figureToBuy;
    public int neededGold = 1;
    public bool isUnlocked = true;

    private ShopManager shopManager;

    private void OnValidate()
    {
        if(goldAmounText != null)
            goldAmounText.text = $"{neededGold}";
    }

    public void InitButton(ShopManager shop)
    {
        shopManager = shop;
        button.interactable = isUnlocked;
        goldAmounText.text = $"{neededGold}";
    }

    public void TryBuyFigure()
    {
        if(GoldManager.Instance.HasEnoughGold(neededGold) && isUnlocked)
        {
            shopManager.figureSpawner.SetSideToSpawn((int)ChessSide.WHITE);
            shopManager.figureSpawner.SetFigureToSpawn((int)figureToBuy);
            shopManager.figureSpawner.FigureJustBought = true;

            buySFX.Play();
            GoldManager.Instance.RemoveGold(neededGold);
        }
        else
        {
            buyInvalidSFX.Play();
        }
    }
}