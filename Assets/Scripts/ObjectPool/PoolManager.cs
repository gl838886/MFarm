using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public List<GameObject> poolPrefabs;

    //我一个物体需要一个对象池
    private List<ObjectPool<GameObject>> poolEffectList = new List<ObjectPool<GameObject>>();
    //音效对象池
    private Queue<GameObject> poolSoundQueue = new Queue<GameObject>();


    //一个很有意思的事实是Transform类不仅用来管理游戏物体的位置缩放旋转，
    //还用来管理游戏物体的父物体与子物体之间的关系
    //直接写transform即可获得脚本挂载物体的transform

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

    //执行粒子特效
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
        //这里不能直接释放，需要一个协程，否则还为播放粒子效果就执行释放了
        StartCoroutine(releaseObject(objPool, obj));
    }

    //释放对象回对象池
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
    /// 生成对象池
    /// </summary>
    private void CreatPool()
    {
        foreach(GameObject item in poolPrefabs)
        {
            Transform parent = new GameObject(item.name).transform;
            parent.SetParent(transform); //parent的父物体为poolManager

            //这里用lambda表达式简写
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

        //创建20个空对象
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
