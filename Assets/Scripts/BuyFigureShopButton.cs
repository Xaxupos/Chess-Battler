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
        if(shopManager.figureSpawner.combatManager.chessboard.GetLowerHalfEmptySquaresCount() != 0 && GoldManager.Instance.HasEnoughGold(neededGold) && isUnlocked && !shopManager.figureSpawner.FigureJustBought)
        {
            shopManager.figureSpawner.SetSideToSpawn((int)ChessSide.WHITE);
            shopManager.figureSpawner.SetFigureToSpawn((int)figureToBuy);
            shopManager.figureSpawner.FigureJustBought = true;

            buySFX.Play();
            GoldManager.Instance.RemoveGold(neededGold);
            GhostFigureManager.Instance.ActivateGhostFigure(figureToBuy);
        }
        else
        {
            buyInvalidSFX.Play();
        }
    }
}