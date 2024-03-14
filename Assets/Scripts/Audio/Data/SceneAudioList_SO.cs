using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneAudioList_SO", menuName = "Audio/SceneAudioList_SO")]
public class SceneAudioList_SO : ScriptableObject
{
    public List<SceneAudio> sceneAudioList = new List<SceneAudio>();
    public SceneAudio GetSceneAudio(string sceneName)
    {
        return sceneAudioList.Find(s => s.sceneName == sceneName);
    }
}

[System.Serializable]
public class SceneAudio 
{
    public string sceneName;
    public SoundName ambient;
    public SoundName music;
}