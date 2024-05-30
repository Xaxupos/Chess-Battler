using System.Collections.Generic;
using UnityEngine;

public class ChessFigureSFX : MonoBehaviour
{
    [Header("References")]
    public AudioSource audioSource;
    public List<AudioClip> moveClips;
    public List<AudioClip> attackClips;
    public List<AudioClip> takeDamageClips;
    public List<AudioClip> invalidDieClips;
    public List<AudioClip> invalidIncreaseClips;

    [Header("Settings")]
    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;

    public void PlayMoveClip()
    {
        SetPitch();
        audioSource.clip = moveClips.GetRandom();
        audioSource.Play();
    }

    public void PlayAttackClip()
    {
        SetPitch();
        audioSource.clip = attackClips.GetRandom();
        audioSource.Play();
    }

    public void PlayTakeDamageClip()
    {
        SetPitch();
        audioSource.transform.parent = null;
        audioSource.clip = takeDamageClips.GetRandom();
        audioSource.Play();
    }

    public void PlayInvalidDieClip()
    {
        SetPitch();
        audioSource.transform.parent = null;
        audioSource.clip = invalidDieClips.GetRandom();
        audioSource.Play();
    }

    public void PlayInvalidIncreaseClip()
    {
        SetPitch();
        audioSource.transform.parent = null;
        audioSource.clip = invalidIncreaseClips.GetRandom();
        audioSource.Play();
    }

    private void SetPitch()
    {
        audioSource.pitch = Random.Range(minPitch, maxPitch);
    }
}