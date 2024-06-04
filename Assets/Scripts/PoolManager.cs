using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEngine;
using System;
using System.Collections;
using VInspector;

public class PoolManager : MonoBehaviour
{
    public SerializedDictionary<ObjectType, Pool> objectPools = new SerializedDictionary<ObjectType, Pool>();

    public static PoolManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void Start()
    {
        foreach (var pool in objectPools)
        {
            pool.Value.InitPool(() => {
                var obj = Instantiate(pool.Value.objectPrefabs[UnityEngine.Random.Range(0, pool.Value.objectPrefabs.Count)]);
                if (pool.Value.objectsHolderParent != null) { obj.transform.SetParent(pool.Value.objectsHolderParent); }
                return obj; ;
            },
            obj => { obj.SetActive(true); },
            obj => { obj.SetActive(false); },
            obj => { Destroy(obj); },
            pool.Value.startAmount, 12);

            List<GameObject> tmp = new List<GameObject>();
            foreach (var objPrefab in pool.Value.objectPrefabs)
            {
                for (int i = 0; i < pool.Value.startAmount; i++)
                {
                    tmp.Add(GetFromPool(pool.Key));
                }
            }
            foreach (var element in tmp)
            {
                ReleaseToPool(pool.Key, element);
            }
        }
    }

    public GameObject GetFromPool(ObjectType objectType)
    {
        if (objectPools.TryGetValue(objectType, out Pool returnedPool))
        {
            return returnedPool.objectPool.Get();
        }

        Debug.LogError($"Cannot get an object of type {objectType} from the Pool! Returned null!");
        return null;
    }

    public void ReleaseToPool(ObjectType objectType, GameObject objectToRelease, float delay = 0.0f)
    {
        if (objectPools.TryGetValue(objectType, out Pool returnedPool))
        {
            if (delay <= 0)
                returnedPool.objectPool.Release(objectToRelease);
            else
                StartCoroutine(DelayRelease(delay, returnedPool, objectToRelease));

            return;
        }

        Debug.LogError($"Cannot release an {objectToRelease.name} of type {objectType} back to the Pool!");
    }

    private IEnumerator DelayRelease(float delay, Pool poolToReturn, GameObject objToRelease)
    {
        yield return new WaitForSeconds(delay);

        if (poolToReturn == null || objToRelease == null) yield break;

        poolToReturn.objectPool.Release(objToRelease);
    }

    public void ReleaseAllToPool()
    {
        foreach (var currentPool in objectPools)
        {
            List<GameObject> tmp = new List<GameObject>();
            foreach (var objPrefab in currentPool.Value.objectPrefabs)
            {
                for (int i = 0; i < currentPool.Value.startAmount; i++)
                {
                    tmp.Add(GetFromPool(currentPool.Key));
                }
            }
            foreach (var element in tmp)
            {
                ReleaseToPool(currentPool.Key, element);
            }
        }
    }
}

[System.Serializable]
public class Pool
{
    public List<GameObject> objectPrefabs;
    public Transform objectsHolderParent;
    public int startAmount = 10;

    public ObjectPool<GameObject> objectPool { get; set; }

    public void InitPool(Func<GameObject> createAction, Action<GameObject> onGetAction,
        Action<GameObject> onReleaseAction, Action<GameObject> onDestroyAction, int defaultSize, int maxSize)
    {
        objectPool = new ObjectPool<GameObject>(createAction, onGetAction, onReleaseAction, onDestroyAction, true, defaultSize, maxSize);
    }
}

public enum ObjectType
{
    AUDIO_SOURCE,
    UNIVERSAL_MOVE_VFX,
    UNIVERSAL_TAKE_DAMAGE_VFX,
    UNIVERSAL_DIE_VFX,
    KNIGHT_BOMB_VFX
}