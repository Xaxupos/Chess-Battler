using UnityEngine;

public class GameEffectsDatabase : MonoBehaviour
{
    public static GameEffectsDatabase Instance;

    public SFXDatabase sfxDatabase;

    public void PlaySFX(ActionType actionType)
    {
        sfxDatabase.PlayGameSound(actionType);
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}