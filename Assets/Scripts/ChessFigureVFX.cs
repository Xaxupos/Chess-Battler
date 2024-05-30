using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class ChessFigureVFX : MonoBehaviour
{
    [Header("References")]
    public List<ParticleSystem> takeDamageVFX;
    public List<ParticleSystem> dieVFX;
    public List<ParticleSystem> attackVFX;
    public List<ParticleSystem> moveVFX;

    [Button(color = "Green")]
    public void PlayTakeDamageVFX()
    {
        if (takeDamageVFX.Count == 0) return;
        var vfx = takeDamageVFX.GetRandom();
        vfx.Play();
    }

    [Button(color = "Green")]
    public void PlayDieVFX()
    {
        if (dieVFX.Count == 0) return;
        var vfx = dieVFX.GetRandom();
        vfx.transform.parent = null;
        vfx.Play();
    }
}