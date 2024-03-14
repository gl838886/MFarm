using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public List<GameObject> poolPrefabs;

    //��һ��������Ҫһ�������
    private List<ObjectPool<GameObject>> poolEffectList = new List<ObjectPool<GameObject>>();
    //��Ч�����
    private Queue<GameObject> poolSoundQueue = new Queue<GameObject>();


    //һ��������˼����ʵ��Transform�಻������������Ϸ�����λ��������ת��
    //������������Ϸ����ĸ�������������֮��Ĺ�ϵ
    //ֱ��дtransform���ɻ�ýű����������transform

    private void OnEnable()
    {
        EventHandler.ParticleEffectEvent += OnParticleEffectEvent;
        EventHandler.InitSound += InitSoundEffect;
    }

    private void OnDisable()
    {
        EventHandler.ParticleEffectEvent -= OnParticleEffectEvent;
        EventHandler.InitSound -= InitSoundEffect;
    }

    //ִ��������Ч
    private void OnParticleEffectEvent(ParticleEffectType effectType, Vector3 effectPosition)
    {
        var objPool = effectType switch
        {
            ParticleEffectType.LeaveFall01 => poolEffectList[0],
            ParticleEffectType.LeaveFall02 => poolEffectList[1],
            ParticleEffectType.RockFall => poolEffectList[2],
            ParticleEffectType.GrassParticle => poolEffectList[3], 
            _=>null
        };

        GameObject obj = objPool.Get();
        obj.transform.position = effectPosition;
        //���ﲻ��ֱ���ͷţ���Ҫһ��Э�̣�����Ϊ��������Ч����ִ���ͷ���
        StartCoroutine(releaseObject(objPool, obj));
    }

    //�ͷŶ���ض����
    private IEnumerator releaseObject(ObjectPool<GameObject> pool, GameObject obj)
    {
        yield return new WaitForSeconds(1.5f);
        pool.Release(obj);
    }

    private void Start()
    {
        CreatPool();
    }

    /// <summary>
    /// ���ɶ����
    /// </summary>
    private void CreatPool()
    {
        foreach(GameObject item in poolPrefabs)
        {
            Transform parent = new GameObject(item.name).transform;
            parent.SetParent(transform); //parent�ĸ�����ΪpoolManager

            //������lambda���ʽ��д
            var newPool = new ObjectPool<GameObject>(
                () => Instantiate(item, parent),
                e => e.SetActive(true),
                e => e.SetActive(false),
                e => Destroy(e)
                );
            poolEffectList.Add(newPool);
        }
    }

    private void CreateSoundPool()
    {
        var parent = new GameObject(poolEffectList[4].ToString()).transform;

        parent.SetParent(transform);

        //����20���ն���
        for(int i =0;i<20;i++)
        {
            GameObject newObj = Instantiate(poolPrefabs[4], parent);
            newObj.SetActive(false);
            poolSoundQueue.Enqueue(newObj);
        }
    }

    private GameObject GetSoundPoolObject()
    {
        if (poolSoundQueue.Count < 2)
            CreateSoundPool();
        return poolSoundQueue.Dequeue();
    }

    private void InitSoundEffect(AudioDetails audioDetails)
    {
        var soundObj = GetSoundPoolObject();
        soundObj.GetComponent<Sound>().SetSound(audioDetails);
        soundObj.SetActive(true);
        StartCoroutine(DisableSound(soundObj, audioDetails.audioClip.length));
    }

    private IEnumerator DisableSound(GameObject soundObj, float duration)
    {
        yield return new WaitForSeconds(duration);
        soundObj.SetActive(false);
        poolSoundQueue.Enqueue(soundObj);
    }
}
