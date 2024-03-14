namespace Mfarm.Save
{
    public interface ISaveable
    {
        string GUID { get; }

        void RegisterSaveable()
        {
            SaveLoadManager.Instance.RegisterSaveable(this);
        }

        GameSaveData GenerateGameData();

        void RestoreGameData(GameSaveData data);
    }
}


