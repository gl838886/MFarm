using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Sound : MonoBehaviour
{
    public AudioSource audioSource;

    public void SetSound(AudioDetails soundDetails)
    {
        audioSource.clip = soundDetails.audioClip;
        audioSource.volume = soundDetails.soundVolume;
        audioSource.pitch = Random.Range(soundDetails.soundPitchMin, soundDetails.soundPitchMax);
    }
}
