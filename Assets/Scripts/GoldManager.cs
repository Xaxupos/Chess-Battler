using UnityEngine;
using UnityEngine.Events;

public class GoldManager : MonoBehaviour
{
    [Header("Parameters")]
    public int startingGold = 10;
    public int currentGold = 0;

    [Header("Events")]
    public UnityEvent OnGoldAdded;
    public UnityEvent OnGoldRemoved;

    public static GoldManager Instance;

    private void Awake()
    {
        if(Instance == null) Instance = this;
    }

    private void Start()
    {
        currentGold = startingGold;
        OnGoldAdded?.Invoke();
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void AddGold(int gold)
    {
        currentGold += gold;
        OnGoldAdded?.Invoke();
    }

    public void RemoveGold(int gold)
    {
        currentGold -= gold;
        OnGoldRemoved?.Invoke();
    }

    public bool HasEnoughGold(int amountToCheck)
    {
        return currentGold >= amountToCheck;
    }
}