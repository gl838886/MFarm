using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json; //序列化
using System.IO; //文件写入
using System;

namespace Mfarm.Save
{
    public class SaveLoadManager : Singleton<SaveLoadManager>
    {
        public List<ISaveable> saveableList = new List<ISaveable>();

        //一共有3个slot
        public List<DataSlot> dataSlotList = new List<DataSlot>(new DataSlot[3]);

        private string jsonFolder;

        private int currentSlotIndex;

        protected override void Awake()
        {
            base.Awake();
            //文件夹路径
            jsonFolder = Application.persistentDataPath + "/SAVE DATA/";
            ReadSaveData();
        }

        private void OnEnable()
        {
            EventHandler.StartNewGame += OnStartNewGame;
            EventHandler.EndCurrentGame += OnEndCurrentGame;
        }

        private void OnDisable()
        {
            EventHandler.StartNewGame -= OnStartNewGame;
            EventHandler.EndCurrentGame -= OnEndCurrentGame;
        }

        private void OnStartNewGame(int index)
        {
            currentSlotIndex = index;
        }

        private void OnEndCurrentGame()
        {
            Debug.Log(currentSlotIndex);
            Save(currentSlotIndex);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                Save(currentSlotIndex);
                Debug.Log("保存");
            }
            else if (Input.GetKeyDown(KeyCode.O))
            {
                Load(currentSlotIndex);
                Debug.Log("加载");
            }
        }


        private void ReadSaveData()
        {
            //刚打开游戏时
            //一开始加载saveSlotUI时，我需要先去读文件
            if(Directory.Exists(jsonFolder))
            {
                for(int i=0;i<dataSlotList.Count;i++)
                {
                    var resultPath = jsonFolder + "data" + i + ".json";
                    if(File.Exists(resultPath))
                    {
                        var stringData = File.ReadAllText(resultPath);
                        //反序列化
                        var jsonData = JsonConvert.DeserializeObject<DataSlot>(stringData);
                        dataSlotList[i] = jsonData;
                    }
                }
            }
            else
            {
                Debug.Log("Fail to read save data");
            }
        }

        public void RegisterSaveable(ISaveable saveable)
        {
            if (!saveableList.Contains(saveable))
                saveableList.Add(saveable);
        }

        public void Save(int index)
        {
            DataSlot dataSlot = new DataSlot();
            foreach(var saveable in saveableList)
            {
                //将saveableList的东西添加到一个slot上
                dataSlot.slotDataDict.Add(saveable.GUID, saveable.GenerateGameData());
            }
            dataSlotList[index] = dataSlot;

            //文件最终路径 = 文件夹路径 + 文件
            var resultPath = jsonFolder + "data" + index + ".json";
            //序列化
            var jsonData = JsonConvert.SerializeObject(dataSlotList[index], Formatting.Indented);

            if(!File.Exists(resultPath))
            {
                //创建目录
                Directory.CreateDirectory(jsonFolder);
            }
            Debug.Log(resultPath);
            File.WriteAllText(resultPath, jsonData);
        }

        public void Load(int index)
        {
            currentSlotIndex = index;
            //文件最终路径 = 文件夹路径 + 文件
            var resultPath = jsonFolder + "data" + index + ".json";
            Debug.Log(resultPath);
            //先读取出来
            var stringData = File.ReadAllText(resultPath);
            //反序列化
            var jsonData = JsonConvert.DeserializeObject<DataSlot>(stringData);

            foreach(var saveable in saveableList)
            {
                saveable.RestoreGameData(jsonData.slotDataDict[saveable.GUID]);
            }
        }
    }
}
