using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyFigureShopButton : MonoBehaviour
{
    [Header("References")]
    public Button button;
    public TMP_Text buttonText;

    [Header("Settings")]
    public ChessFigureType figureToBuy;
    public int neededGold = 1;
    public bool isUnlocked = true;

    private ShopManager shopManager;

    private void OnValidate()
    {
        buttonText.text = $"{figureToBuy} <br> ({neededGold})";
    }

    public void InitButton(ShopManager shop)
    {
        shopManager = shop;
        button.interactable = isUnlocked;
        buttonText.text = $"{figureToBuy} <br> ({neededGold})";
    }

    public void TryBuyFigure()
    {
        if(GoldManager.Instance.HasEnoughGold(neededGold) && isUnlocked)
        {
            shopManager.figureSpawner.SetSideToSpawn((int)ChessSide.WHITE);
            shopManager.figureSpawner.SetFigureToSpawn((int)figureToBuy);
            shopManager.figureSpawner.FigureJustBought = true;

            GoldManager.Instance.RemoveGold(neededGold);
        }
    }
}