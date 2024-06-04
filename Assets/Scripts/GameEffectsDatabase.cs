using UnityEngine;

public class GameEffectsDatabase : MonoBehaviour
{
    public static GameEffectsDatabase Instance;

    public SFXDatabase sfxDatabase;
    public VFXDatabase vfxDatabase;

    public void PlaySFX(ActionType actionType)
    {
        sfxDatabase.PlayGameSound(actionType);
    }

    public void PlayVFX(ActionType actionType, Vector3 position)
    {
        vfxDatabase.PlayGameVFX(actionType, position);
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