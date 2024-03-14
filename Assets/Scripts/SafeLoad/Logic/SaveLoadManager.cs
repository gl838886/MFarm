using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json; //���л�
using System.IO; //�ļ�д��
using System;

namespace Mfarm.Save
{
    public class SaveLoadManager : Singleton<SaveLoadManager>
    {
        public List<ISaveable> saveableList = new List<ISaveable>();

        //һ����3��slot
        public List<DataSlot> dataSlotList = new List<DataSlot>(new DataSlot[3]);

        private string jsonFolder;

        private int currentSlotIndex;

        protected override void Awake()
        {
            base.Awake();
            //�ļ���·��
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
                Debug.Log("����");
            }
            else if (Input.GetKeyDown(KeyCode.O))
            {
                Load(currentSlotIndex);
                Debug.Log("����");
            }
        }


        private void ReadSaveData()
        {
            //�մ���Ϸʱ
            //һ��ʼ����saveSlotUIʱ������Ҫ��ȥ���ļ�
            if(Directory.Exists(jsonFolder))
            {
                for(int i=0;i<dataSlotList.Count;i++)
                {
                    var resultPath = jsonFolder + "data" + i + ".json";
                    if(File.Exists(resultPath))
                    {
                        var stringData = File.ReadAllText(resultPath);
                        //�����л�
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
                //��saveableList�Ķ�����ӵ�һ��slot��
                dataSlot.slotDataDict.Add(saveable.GUID, saveable.GenerateGameData());
            }
            dataSlotList[index] = dataSlot;

            //�ļ�����·�� = �ļ���·�� + �ļ�
            var resultPath = jsonFolder + "data" + index + ".json";
            //���л�
            var jsonData = JsonConvert.SerializeObject(dataSlotList[index], Formatting.Indented);

            if(!File.Exists(resultPath))
            {
                //����Ŀ¼
                Directory.CreateDirectory(jsonFolder);
            }
            Debug.Log(resultPath);
            File.WriteAllText(resultPath, jsonData);
        }

        public void Load(int index)
        {
            currentSlotIndex = index;
            //�ļ�����·�� = �ļ���·�� + �ļ�
            var resultPath = jsonFolder + "data" + index + ".json";
            Debug.Log(resultPath);
            //�ȶ�ȡ����
            var stringData = File.ReadAllText(resultPath);
            //�����л�
            var jsonData = JsonConvert.DeserializeObject<DataSlot>(stringData);

            foreach(var saveable in saveableList)
            {
                saveable.RestoreGameData(jsonData.slotDataDict[saveable.GUID]);
            }
        }
    }
}
