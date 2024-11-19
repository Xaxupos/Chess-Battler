using UnityEngine;

public class LoseGameManager : MonoBehaviour
{
    [Header("References")]
    public GameUIManager gameUIManager;

    public void LoseGame()
    {
        gameUIManager.ShowLoseUI();
    }
}