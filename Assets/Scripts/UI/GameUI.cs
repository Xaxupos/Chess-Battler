using UnityEngine;

public abstract class GameUI : MonoBehaviour
{
    public void ToggleUI(bool value)
    {
        gameObject.SetActive(value);
    }
}