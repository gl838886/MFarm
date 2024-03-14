using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioDataList_SO", menuName = "Audio/AudioDataList_SO")]
public class AudioDataList_SO : ScriptableObject
{
    public List<AudioDetails> audioDetailsList = new List<AudioDetails>();
    public AudioDetails GetAudioDetails(SoundName soundName)
    {
        return audioDetailsList.Find(a => a.name == soundName);
    }
}

[System.Serializable]
public class AudioDetails
{
    public SoundName name;
    public AudioClip audioClip;

    [Range(0.1f, 1.5f)]
    public float soundPitchMin;

    [Range(0.1f, 1.5f)]
    public float soundPitchMax;

    [Range(0.1f, 1f)]
    public float soundVolume;
}
