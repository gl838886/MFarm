 
using System.Collections;
 
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>
{
    [Header("音乐资料库")]
    public AudioDataList_SO audioDataList;
    public SceneAudioList_SO sceneAudioList;

    [Header("音乐组件")]
    public AudioSource gameSource;
    public AudioSource ambientSource;

    //音乐间隔（随机）
    private float randomSeconds => Random.Range(5f, 15f);

    public Coroutine soundRoutine;

    [Header("AudioMixer")]
    public AudioMixer audioMixer;

    [Header("SnapShots")]
    public AudioMixerSnapshot normalSnapShots;
    public AudioMixerSnapshot ambientSnapShots;
    public AudioMixerSnapshot muteSnapShots;

    private void OnEnable()
    {
        EventHandler.AfterLoadSceneEvent += OnAfterLoadSceneEvent;
        EventHandler.PlaySoundEvent += OnPlaySoundEvent;
        EventHandler.EndCurrentGame += OnEndCurrentGame;
    }

    private void OnDisable()
    {
        EventHandler.AfterLoadSceneEvent -= OnAfterLoadSceneEvent;
        EventHandler.PlaySoundEvent -= OnPlaySoundEvent;
        EventHandler.EndCurrentGame -= OnEndCurrentGame;
    }


    private void OnPlaySoundEvent(SoundName soundName)
    {
        AudioDetails audioDetails = audioDataList.GetAudioDetails(soundName);
        if(audioDetails != null)
        {
            EventHandler.CallInitSound(audioDetails);
        }
    }

    private void OnAfterLoadSceneEvent()
    {
        //场景加载完，获得场景名，到sceneAudioList中寻找对应场景名的SceneAudio
        //再通过SceneAudio的soundName去audioDataList寻找对应的音乐

        string sceneName = SceneManager.GetActiveScene().name;
        SceneAudio sceneAudio = sceneAudioList.GetSceneAudio(sceneName); 

        if (sceneAudio == null)
            return;

        AudioDetails gameAudioDetails = audioDataList.GetAudioDetails(sceneAudio.music);
        AudioDetails ambientAudioDetails = audioDataList.GetAudioDetails(sceneAudio.ambient);

        //播放音乐
        //PlayGameMusic(gameAudioDetails);
        //PlayGameMusic(ambientAudioDetails);

        if(soundRoutine!=null)
        {
            StopCoroutine(soundRoutine);
        }
        soundRoutine = StartCoroutine(PlaySoundRoutinue(gameAudioDetails, ambientAudioDetails));
    }


    private void OnEndCurrentGame()
    {
        muteSnapShots.TransitionTo(1f); //静音
        //停止协程
        if(soundRoutine != null)
        {
            StopCoroutine(soundRoutine);
        }
    }

    private IEnumerator PlaySoundRoutinue(AudioDetails gameAudio, AudioDetails ambientAudio)
    {
        //使用协程是希望场景切换时音乐衔接中间有间隔
        if(gameAudio != null && ambientAudio != null)
        {
            PlayAmbientMusic(ambientAudio);
            yield return new WaitForSeconds(randomSeconds);
            PlayGameMusic(gameAudio);
        }
    }

    /// <summary>
    /// 播放背景音效
    /// </summary>
    /// <param name="audioDetails"></param>
    private void PlayGameMusic(AudioDetails audioDetails)
    {
        audioMixer.SetFloat("MusicVolume", ConvertSoundVolume(audioDetails.soundVolume));
        gameSource.clip = audioDetails.audioClip;
        if(gameSource.isActiveAndEnabled)
            gameSource.Play();

        normalSnapShots.TransitionTo(8f); //
    }

    /// <summary>
    /// 播放环境音
    /// </summary>
    /// <param name="audioDetails"></param>
    private void PlayAmbientMusic(AudioDetails audioDetails)
    {
        audioMixer.SetFloat("AmbientVolume", ConvertSoundVolume(audioDetails.soundVolume));
        ambientSource.clip = audioDetails.audioClip;
        if (ambientSource.isActiveAndEnabled)
            ambientSource.Play();

        ambientSnapShots.TransitionTo(1f);
    }

    private float ConvertSoundVolume(float volume)
    {
        return (volume * 100 - 80);
        //0.1-1对应-80-20
    }

    public void SetSliderVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume * 100 - 80);
    }
}
