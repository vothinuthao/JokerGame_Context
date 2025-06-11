using Core.Utils;
using UnityEngine;

namespace Manager
{
    public class PlayerDataManager : Singleton<PlayerDataManager>
    {


        #region Fields & Properties
        private PlayerPropertyManager _playerProperty;
        public PlayerPropertyManager Property
        {
            get
            {
                if (_playerProperty == null)
                    _playerProperty = new PlayerPropertyManager();
                return _playerProperty;
            }
        }
        private ShopPropertyManager _shopProperty;
        public ShopPropertyManager ShopProperty
        {
            get
            {
                if (_shopProperty == null)
                    _shopProperty = new ShopPropertyManager();
                return _shopProperty;
            }
        }
        #endregion

        public void SaveAllData()
        {
            _playerProperty.SavePlayerData();
            Debug.Log("SaveAllData");
        }
        
    
    }
}