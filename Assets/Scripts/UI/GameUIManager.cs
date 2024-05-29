using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [Header("References")]
    public GameUI betweenWaveUI;
    public GameUI waveUI;

    public void ShowWaveUI()
    {
        betweenWaveUI.ToggleUI(false);
        waveUI.ToggleUI(true);
    }

    public void ShowBetweenWaveUI()
    {
        waveUI.ToggleUI(false);
        betweenWaveUI.ToggleUI(true);
    }
}