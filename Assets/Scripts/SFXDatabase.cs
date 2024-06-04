using UnityEngine;
using VInspector;

[CreateAssetMenu(fileName = "SFX Database", menuName = "Databases/SFX Database")]
public class SFXDatabase : ScriptableObject
{
    public SerializedDictionary<ActionType, SFXDatabaseEntry> gameSFX;

    public void PlayGameSound(ActionType actionType)
    {
        var dbEntry = gameSFX[actionType];

        var audioSourceGO = PoolManager.Instance.GetFromPool(ObjectType.AUDIO_SOURCE);
        if(audioSourceGO.TryGetComponent(out AudioSource audioSource))
        {
            audioSource.pitch = Random.Range(dbEntry.minPitch, dbEntry.maxPitch);
            audioSource.volume = dbEntry.volume;
            audioSource.clip = dbEntry.clips.GetRandom();
            audioSource.Play();

            PoolManager.Instance.ReleaseToPool(ObjectType.AUDIO_SOURCE, audioSourceGO, audioSource.clip.length);
        }
    }
}

[System.Serializable]
public class SFXDatabaseEntry
{
    public AudioClip[] clips;
    [Range(0, 1)] public float volume;
    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;
}