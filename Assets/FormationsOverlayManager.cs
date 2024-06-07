using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationsOverlayManager : MonoBehaviour
{
    public static FormationsOverlayManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}