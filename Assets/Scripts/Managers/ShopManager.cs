using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("References")]
    public ChessFigureSpawner figureSpawner;
    public List<BuyFigureShopButton> buyFigureButtons = new List<BuyFigureShopButton>();

    private void Awake()
    {
        foreach(var button in buyFigureButtons)
        {
            button.InitButton(this);
        }
    }
}