using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [Header("References")]
    public GameUI betweenWaveUI;
    public GameUI waveUI;
    public GameUI alwaysVisibleUI;
    public GameUI loseUI;

    public static GameUIManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

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

    public void ShowLoseUI()
    {
        loseUI.ToggleUI(true);
        waveUI.ToggleUI(false);
        betweenWaveUI.ToggleUI(false);
    }
}