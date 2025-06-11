using System;
using System.Collections.Generic;
using Core.Entity;
using Core.Manager;
using Core.Observer;
using Core.Utils;
using Frameworks.Base;
using Frameworks.UIAlert;
using Pathfinding.Serialization.JsonFx;
using Player;
using UnityEngine;
using System.Linq;
using Frameworks.Scripts;
using Frameworks.UIPopup;
using UI.Popups.ClassParameter;

namespace Manager
{
    public class PlayerPropertyManager
    {
        public event Action<int> OnDollaChanged;
        public event Action<int> OnGemChanged;
        public event Action<int, int> OnExpChanged;
        public event Action<int> OnLevelChanged;
        public event Action<int> OnRoundChanged;
        public event Action<int> OnAnteChanged;
        public event Action<bool> OnUpdateJokerOwnChanged;
        public event Action<int> OnLoyaltyJokerCount;
    
        #region Properties
        public string UserId => _playerProps.Uid;
        public string Nickname => _playerProps.NickName;
        public int Dollar => _playerProps.Dollar;
        public int Gem => _playerProps.Gem;
        public int Exp => _playerProps.Exp;
        public int Level => _playerProps.Level;
        public int Ante => _playerProps.Ante;
        public int Round => _playerProps.Round;
        public int RoundWatchAds => _playerProps.RoundWatchAds;
        public List<int> TotalOwnerJoker => _playerProps.TotalOwnerJoker; 
        public int TotalHandsPlayed => _playerProps.TotalHandsPlayed;
        // method Stats
        
        public int MaxHandSlots => _playerProps.MaxHandSlots;
        public int MaxJokerSlots => _playerProps.MaxJokerSlots;
        public bool JokerSlotAPI => _playerProps.JokerSlotIAP;
        public bool JokerSlotAds => _playerProps.JokerSlotAds;
        public bool BeginnerPack => _playerProps.BeginnerPack;
        public int MaxDrawCardInHand => _playerProps.MaxDrawCardInHand;
        public int ExtraHandSlots => _playerProps.ExtraHandSlots;
        public int HandSinceLoyaltyJoker => _playerProps.HandSinceLoyaltyJoker;
        public int ConsecutiveFaceCardCount => _playerProps.ConsecutiveFaceCardCount;
        public int TwoPairPlayed => _playerProps.TwoPairPlayed;
        public int StraightPlayed => _playerProps.StraightPlayed;
        public bool IsCompletedTutorial => _playerProps.IsCompletedTutorial;
        public bool IsCompletedFirstPlay => _playerProps.IsCompletedFirstPlay;
        public int CurrentIdTutorial => _playerProps.CurrentIdTutorial;
        public bool RemoveAds => _playerProps.RemoveAds;
        public StateScene CurrentPlayerStay => _playerProps.CurrentPlayerStay;
        public PlayerLastResult PlayerLastResult => _playerProps.PlayerLastResult;
        private PlayerDataModel _playerProps = new PlayerDataModel();
        #endregion
        #region Property action 
        public void InitPlayerPropertyData(string jsonPlayerData)
        {
            bool isCreateDefaultData = false;

            if (string.IsNullOrEmpty(jsonPlayerData))
            {
                _playerProps = CreateDefaultPlayerData();
                isCreateDefaultData = true;
            }
            else
            {
                try
                {
                    _playerProps = JsonReader.Deserialize<PlayerDataModel>(jsonPlayerData);
                    DataManager.Instance.LogDebugSave("Player data", jsonPlayerData);
                }
                catch (Exception e)
                {
                    Debug.LogError("Cannot parse player props:" + e.Message);
                    _playerProps = CreateDefaultPlayerData();
                    isCreateDefaultData = true;
                }
            }

            //save new token data after update
            if (isCreateDefaultData)
                SavePlayerData();
        }
        private PlayerDataModel CreateDefaultPlayerData()
        {
            return new PlayerDataModel()
            {
                Uid = PlayerUtils.CreatShortUID(),
                NickName = "Joker's Gambit" + PlayerUtils.UtcDayNow(),
                Level = 1,
                Exp = 0,
                IsCompleteTutorial = false,
                HasInterstitialAds = true,
                TotalOwnerJoker = new List<int>(){},
                MaxHandSlots = 4,
                ExtraHandSlots = 0,
                Ante = 1,
                Round = 1,
                MaxJokerSlots = 5,
                JokerSlotAds = false,
                JokerSlotIAP = false,
                BeginnerPack = false,
                MaxConsumableOwner = 4,
                MaxDrawCardInHand = 8,
                Dollar = 0,
                Gem = 0,
                HandSinceLoyaltyJoker = 1,
                IsCompletedTutorial = false,
                IsCompletedFirstPlay = true,
                CurrentIdTutorial = 0,
                CurrentPlayerStay = StateScene.GameView,
                RoundWatchAds = 5,
                RemoveAds = false,
                PlayerLastResult = new PlayerLastResult()
                {
                    ResultLastGame = ResultGameEnum.WinPerRound,
                    PokerHandValue = PokerHandValue.None,
                    BestScoreLastGame = 0,
                    PokerHandValueCount = 0,
                    CountDiscardLastGame = 4,
                    CountHandLastGame = 0
                }
            };
        }
        public void SavePlayerData()
        {
            string strPlayerData = JsonWriter.Serialize(_playerProps);
            DataManager.Instance.SavePlayerData(strPlayerData);
        }
        #endregion
        public bool AddDollar(int amount, string source = "", bool save = true, bool showImage = false)
        {
            if (amount < 0) return false;
            _playerProps.Dollar += amount;
            //Callback
            AudioManager.Instance.PlaySFX(AudioName.Gold_Add);
            OnDollaChanged?.Invoke(Dollar);
            if (save) SavePlayerData();
            return true;
        }
        public bool SubDolla(int amount, string source = "", bool save = true, bool showImage = false)
        {
            if (amount < 0) return false;
            _playerProps.Dollar -= amount;
            //Callback
            OnDollaChanged?.Invoke(Dollar);
            if (save) SavePlayerData();
            return true;
        }
        public bool AddGem(int amount, string source = "", bool save = true, bool showImage = false)
        {
            if (amount < 0) return false;

            _playerProps.Gem += amount;

            //Callback
            OnGemChanged?.Invoke(Gem);

            if (save) SavePlayerData();

            return true;
        }
        public void UpdateStatsPlayer()
        {
            var configPlay = ConfigManager.configPlayRound.GetValueByRound(_playerProps.Round + 1 );
            // _playerProps.Ante = configPlay.ante;
            _playerProps.Round += 1;
            if (_playerProps.ExtraHandSlots > 0)
            {
                SubExtraHand(1);
            }
            SavePlayerData();
        }
        
        public void AddJokerCardToInventory(int id)
        {
            _playerProps.TotalOwnerJoker.Add(id);
            OnUpdateJokerOwnChanged?.Invoke(true);
            SavePlayerData();
        }
        public void ResetIndexJoker(List<int> listIndex)
        {
            _playerProps.TotalOwnerJoker.Clear();
            foreach (var index in listIndex)
            {
                _playerProps.TotalOwnerJoker.Add(index);
            }
            // OnUpdateJokerOwnChanged?.Invoke(true);
            SavePlayerData();
        }

        public bool CheckSlotJokerInventory()
        {
            var countOwner = _playerProps.TotalOwnerJoker.Count;
            var maxSlot = _playerProps.MaxJokerSlots;
            return countOwner < maxSlot;
        }
        public bool CheckBeginnerPack()
        {
            if (_playerProps.BeginnerPack) return false;
            _playerProps.BeginnerPack = true;
            AddSlotJokerIAPInventory();
            SavePlayerData();
            return true;
        }
        public bool AddSlotJokerAdsInventory()
        {
            if (_playerProps.JokerSlotAds) return false;
            _playerProps.JokerSlotAds = true;
            _playerProps.MaxJokerSlots++;
            SavePlayerData();
            return true;
        }
        private bool AddSlotJokerIAPInventory()
        {
            if (_playerProps.JokerSlotIAP) return false;
            _playerProps.JokerSlotIAP = true;
            _playerProps.MaxJokerSlots++;
            SavePlayerData();
            return true;
        }
        public void SubJokerCardToInventory(int id)
        {
            var list = _playerProps.TotalOwnerJoker;
            if (list.Contains(id))
            {
                _playerProps.TotalOwnerJoker.Remove(id);
                OnUpdateJokerOwnChanged?.Invoke(true);
                SavePlayerData();
            }
        }
        
        public bool AddTotalHandsPlayed(int amount)
        {
            if (amount < 0) return false;
            _playerProps.TotalHandsPlayed += amount;
            SavePlayerData();
            return true;
        }
        public void ClearTotalHandsPlayed()
        {
            _playerProps.TotalHandsPlayed = 0;
            SavePlayerData();
        }
        public int GetTotalHandsPlayed()
        {
            return _playerProps.TotalHandsPlayed;
        }
        public bool AddExtraHand(int amount)
        {
            if(amount < 0 ) return false;
            _playerProps.ExtraHandSlots += amount;
            SavePlayerData();
            return true;
        }
        public bool SubExtraHand(int amount)
        {
            if(amount < 0 || _playerProps.ExtraHandSlots <= 0) return false;
            _playerProps.ExtraHandSlots -= amount;
            SavePlayerData();
            return true;
        }
        public void SubAllExtraHand()
        {
            _playerProps.ExtraHandSlots = 0;
            SavePlayerData();
        }
        public void ResetDataPlayer()
        {
            _playerProps.TotalOwnerJoker = new List<int>();
            _playerProps.MaxHandSlots = 4;
            _playerProps.ExtraHandSlots = 0;
            _playerProps.Dollar = 0;
            _playerProps.Gem = 0;
            _playerProps.HandSinceLoyaltyJoker = 0;
            _playerProps.ConsecutiveFaceCardCount = 0;
            _playerProps.TwoPairPlayed = 0;
            _playerProps.StraightPlayed = 0;
            _playerProps.Round = 1;
            _playerProps.TotalHandsPlayed = 0;
            PlayerDataManager.Instance.ShopProperty.ClearDataBoosterPackOnShop();
            PlayerDataManager.Instance.ShopProperty.ClearDataJokerOnShop();
            if (_playerProps.BeginnerPack)
            {
                var listRandom = ShopController.Instance.RandomJokerCard(2);
                foreach (var so in listRandom)
                {
                    AddJokerCardToInventory(so.ID);
                }
            }
            SavePlayerData();
        }

        public void SetStatePlayerStay(StateScene state)
        {
            if(_playerProps.CurrentPlayerStay == state)
                return;
            _playerProps.CurrentPlayerStay = state;
            SavePlayerData();
        }

        public void SetPlayerLastGameInfo(PlayerLastResult info)
        {
            // if(_playerProps.CurrentPlayerStay == StateScene.OnResult)
            _playerProps.PlayerLastResult = info;
            SavePlayerData();
        }

        public void CountHandPlayed()
        {
            _playerProps.HandSinceLoyaltyJoker += 1;
            OnLoyaltyJokerCount?.Invoke(1);
            SavePlayerData();
        }

        public void CountStraightPlayed(int value)
        {
            _playerProps.StraightPlayed += value;
            SavePlayerData();
        }

        public void CountConsecutiveHand( bool resetCount)
        {
            if(!resetCount)
                _playerProps.ConsecutiveFaceCardCount += 1;
            else
                _playerProps.ConsecutiveFaceCardCount = 0;
            SavePlayerData();
        }
        public void CountTwoPairPlayed() 
        {
            _playerProps.TwoPairPlayed += 1;
            SavePlayerData();
        }
        public void SetCurrentTutorial(int currentTut)
        {
            _playerProps.CurrentIdTutorial = currentTut;
            SavePlayerData();
        }
        public void OnCompleteTutorial()
        {
            _playerProps.IsCompletedTutorial = true;
            SavePlayerData();
        }
        public void IsCompleteFirstPlay()
        {
            _playerProps.IsCompletedFirstPlay = true;
            SavePlayerData();
        }
        public string ConvertListIDJokerToString()
        {
            string result = string.Join(", ", _playerProps.TotalOwnerJoker);
            return result;
        }
        public void AddRoundWatchAds()
        {
            _playerProps.RoundWatchAds++;
            SavePlayerData();
        }
        public void ResetWatchAds()
        {
            _playerProps.RoundWatchAds = 0;
            SavePlayerData();
        }
        public bool CheckSlot()
        {
            return  _playerProps.MaxJokerSlots > _playerProps.TotalOwnerJoker.Count();
        }

        public bool BuySuccessIAPRemoveAds()
        {
            _playerProps.RemoveAds = true;
            SavePlayerData();
            return true;
        }
    }
}

