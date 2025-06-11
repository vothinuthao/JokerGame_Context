using System;
using System.Collections.Generic;
using Pathfinding.Serialization.JsonFx;
using Player;
using UnityEngine;

namespace Manager
{
    public class ShopPropertyManager
    {
        private ShopDataModel _shopProps = new ShopDataModel();
        
        public List<int> JokerOnShop => _shopProps.JokerOnShop;
        public List<int> BoosterPackOnShop => _shopProps.BoosterPackOnShop;
        public void InitShopPropertyData(string jsonShopData)
        {
            bool isCreateDefaultData = false;

            if (string.IsNullOrEmpty(jsonShopData))
            {
                _shopProps = CreateDefaultShopData();
                isCreateDefaultData = true;
            }
            else
            {
                try
                {
                    _shopProps = JsonReader.Deserialize<ShopDataModel>(jsonShopData);
                    DataManager.Instance.LogDebugSave("Shop data", jsonShopData);
                }
                catch (Exception e)
                {
                    Debug.LogError("Cannot parse player props:" + e.Message);
                    _shopProps = CreateDefaultShopData();
                    isCreateDefaultData = true;
                }
            }

            //save new token data after update
            if (isCreateDefaultData)
                SaveShopPlayerData();
        }
        public void ClearDataJokerOnShop()
        {
            _shopProps.JokerOnShop.Clear();
            SaveShopPlayerData();
        }
        public void SaveIdJokerShop(int idJoker)
        {
            if (!_shopProps.JokerOnShop.Contains(idJoker))
            {
                _shopProps.JokerOnShop.Add(idJoker);
            }
            SaveShopPlayerData();
        }
        public void RemoveIdJokerShop(int idJoker)
        {
            if (_shopProps.JokerOnShop.Contains(idJoker))
            {
                _shopProps.JokerOnShop.Remove(idJoker);
            }
            SaveShopPlayerData();
        }
        public void ClearDataBoosterPackOnShop()
        {
            _shopProps.BoosterPackOnShop.Clear();
            SaveShopPlayerData();
        }
        public void SaveIdBoosterPackShop(int idJoker)
        {
            if (!_shopProps.BoosterPackOnShop.Contains(idJoker))
            {
                _shopProps.BoosterPackOnShop.Add(idJoker);
            }
            SaveShopPlayerData();
        }
        public void RemoveIdBoosterPackShop(int idJoker)
        {
            if (_shopProps.BoosterPackOnShop.Contains(idJoker))
            {
                _shopProps.BoosterPackOnShop.Remove(idJoker);
            }
            SaveShopPlayerData();
        }
        private ShopDataModel CreateDefaultShopData()
        {
            return new ShopDataModel()
            {
                Uid = PlayerDataManager.Instance.Property.UserId,
                JokerOnShop = new List<int>(),
                BoosterPackOnShop = new List<int>(),
            };
        }
        private void SaveShopPlayerData()
        {
            string strShopData = JsonWriter.Serialize(_shopProps);
            DataManager.Instance.SaveShopData(strShopData);
        }
    }
}