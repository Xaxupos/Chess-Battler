using UnityEngine;
using VInspector;

[CreateAssetMenu(fileName = "VFX Database", menuName = "Databases/VFX Database")]
public class VFXDatabase : ScriptableObject
{
    public SerializedDictionary<ActionType, VFXDatabaseEntry> gameVFX;

    public void PlayGameVFX(ActionType actionType, Vector3 position)
    {
        if (!gameVFX.ContainsKey(actionType)) return;

        var dbEntry = gameVFX[actionType];

        var vfxGO = PoolManager.Instance.GetFromPool(dbEntry.objectType);

        if (vfxGO == null) return;

        var vfx = vfxGO.GetComponentInChildren<ParticleSystem>();

        if (vfx)
        {
            vfxGO.transform.position = position;
            vfx.Play();
            PoolManager.Instance.ReleaseToPool(dbEntry.objectType, vfxGO, 2.5f);
        }
    }
}

[System.Serializable]
public class VFXDatabaseEntry
{
    public ObjectType objectType;
}