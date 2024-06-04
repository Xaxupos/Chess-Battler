using UnityEngine;
using System.Collections.Generic;

public class FormationsManager : MonoBehaviour
{
    public List<FormationData> formations;

    public static FormationsManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
