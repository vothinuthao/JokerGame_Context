using System.Threading.Tasks;
using Bayat.SaveSystem;
using Core.Entity;
using Core.Utils;
using UnityEngine;

namespace Manager
{
    public class DataManager : Singleton<DataManager>
    {
        private bool _isDataLoaded = false;

        public bool IsDataLoaded => _isDataLoaded;
        public void LoadAllData()
        {
            string jsonPlayerData = LoadDataFromFile(FileDataName.PlayerData).Result;
            string jsonShopData = LoadDataFromFile(FileDataName.ShopData).Result;
            string jsonPlayerInGameData = LoadDataFromFile(FileDataName.PlayerInGameData).Result;
            PlayerDataManager.Instance.Property.InitPlayerPropertyData(jsonPlayerData);
            PlayerDataManager.Instance.ShopProperty.InitShopPropertyData(jsonShopData);
            PlayCardController.Instance.InitPlayerInGamePropertyData(jsonPlayerInGameData);
        }
        public void SavePlayerData(string jsonPlayerData)
        {
            SaveSystemAPI.SaveAsync(FileDataName.PlayerData, jsonPlayerData);
            LogDebugSave("Player data", jsonPlayerData);
        }
        public void SaveShopData(string jsonShopData)
        {
            SaveSystemAPI.SaveAsync(FileDataName.ShopData, jsonShopData);
            LogDebugShopSave("Shop data", jsonShopData);
        }
        public void SavePlayerInGameData(string jsonPlayerInGameData)
        {
            SaveSystemAPI.SaveAsync(FileDataName.PlayerInGameData, jsonPlayerInGameData);
            LogDebugPlayerInGameSave("PlayerInGame data", jsonPlayerInGameData);
        }


        private async Task<string> LoadDataFromFile(string identifierName)
        {
            string ret = "";
            if (!await SaveSystemAPI.ExistsAsync(identifierName))
            {
                return ret;
            }
            ret = await SaveSystemAPI.LoadAsync<string>(identifierName);
            return ret;
        }


        public void LogDebugSave(string context, string value)
        {
            Debug.LogFormat("<color=Yellow>SAVE {0}: {1} </color>", context, value);
        }
        private void LogDebugShopSave(string context, string value)
        {
            Debug.LogFormat("<color=Cyan>SAVE {0}: {1} </color>", context, value);
        }
        
        public void LogDebugPlayerInGameSave(string context, string value)
        {
            Debug.LogFormat("<color=Blue>SAVE {0}: {1} </color>", context, value);
        }


    }
}