 
using System.Collections;
 
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>
{
    [Header("�������Ͽ�")]
    public AudioDataList_SO audioDataList;
    public SceneAudioList_SO sceneAudioList;

    [Header("�������")]
    public AudioSource gameSource;
    public AudioSource ambientSource;

    //���ּ���������
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
        //���������꣬��ó���������sceneAudioList��Ѱ�Ҷ�Ӧ��������SceneAudio
        //��ͨ��SceneAudio��soundNameȥaudioDataListѰ�Ҷ�Ӧ������

        string sceneName = SceneManager.GetActiveScene().name;
        SceneAudio sceneAudio = sceneAudioList.GetSceneAudio(sceneName); 

        if (sceneAudio == null)
            return;

        AudioDetails gameAudioDetails = audioDataList.GetAudioDetails(sceneAudio.music);
        AudioDetails ambientAudioDetails = audioDataList.GetAudioDetails(sceneAudio.ambient);

        //��������
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
        muteSnapShots.TransitionTo(1f); //����
        //ֹͣЭ��
        if(soundRoutine != null)
        {
            StopCoroutine(soundRoutine);
        }
    }

    private IEnumerator PlaySoundRoutinue(AudioDetails gameAudio, AudioDetails ambientAudio)
    {
        //ʹ��Э����ϣ�������л�ʱ�����ν��м��м��
        if(gameAudio != null && ambientAudio != null)
        {
            PlayAmbientMusic(ambientAudio);
            yield return new WaitForSeconds(randomSeconds);
            PlayGameMusic(gameAudio);
        }
    }

    /// <summary>
    /// ���ű�����Ч
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
    /// ���Ż�����
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
        //0.1-1��Ӧ-80-20
    }

    public void SetSliderVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume * 100 - 80);
    }
}
