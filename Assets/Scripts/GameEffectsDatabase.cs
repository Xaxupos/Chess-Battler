using UnityEngine;

public class GameEffectsDatabase : MonoBehaviour
{
    public static GameEffectsDatabase Instance;

    public Chessboard chessboard;
    public SFXDatabase sfxDatabase;
    public VFXDatabase vfxDatabase;

    public void PlaySFX(ActionType actionType)
    {
        sfxDatabase.PlayGameSound(actionType);
    }

    public void PlayVFX(ActionType actionType, Vector3 position, Transform parent = null)
    {
        vfxDatabase.PlayGameVFX(actionType, position, parent);
        chessboard.transform.localScale = chessboard.transform.localScale;
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