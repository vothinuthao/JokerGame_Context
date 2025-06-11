using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entity;
using Core.Manager;
using Core.Model;
using Enemy;
using Entity;
using Frameworks.Scripts;
using Frameworks.Utils;
using InGame;
using MainMenu;
using Manager;
using Model;
using Pathfinding.Serialization.JsonFx;
using Runtime.Manager;
using UnityEngine;

public class PlayCardController : Core.Utils.ManualSingletonMono<PlayCardController>
{
    public event Action<float> OnChipChanged;
    public event Action<float> OnMultChanged;
    public event Action<float> OnRoundScoreChanged;
    public event Action<int> OnDiscardsChanged;
    public event Action<int> OnHandsChanged;
    public event Action<bool> IsDoneCalHandDeck;
    public event Action<int> OnHealthChanged;
    public event Action<int> OnShieldChanged;
    public event Action<int> OnDollarInGameChanged;
    //check list 
    private float _totalDmgPerRound = 0f;
    private float _totalDmgPerTurn = 0f;
    private PlayerDataInGameModel _playerInGameProps = new PlayerDataInGameModel();
    private List<PokerCard> _lstFullDeck = new List<PokerCard>();
    private List<PokerCard> _lstCardOnHand = new List<PokerCard>();
    private List<PokerCardVo> _lstCardOnHandScripts = new List<PokerCardVo>();
    private List<PokerCard> _lstCardInDeck = new List<PokerCard>();
    private List<PokerCardVo> _lstScriptsCardOnSelect = new List<PokerCardVo>();
    private List<PokerCardVo> _listCardDroped = new List<PokerCardVo>();
    private List<PokerCardVo> _lstCardPlayed = new List<PokerCardVo>();
    private PokerHandValue _pokerHandValue;
    public float TotalDmgPerTurn => _totalDmgPerTurn;
    public List<PokerCard> ListCardFullDeck => _lstFullDeck;
    public PokerHandValue CurrentPokerHandValue => _pokerHandValue;
    public List<PokerCard> ListCardOnHand => _lstCardOnHand;
    public List<PokerCard> ListCardInDeck => _lstCardInDeck;
    public List<PokerCardVo> ListCardOnSelect => _lstScriptsCardOnSelect;
    public List<PokerCardVo> ListCardOnHandScripts => _lstCardOnHandScripts;

    public float Chip => _playerInGameProps.Chip;
    public float Mult => _playerInGameProps.Mult;
    public float TotalDamage => _playerInGameProps.TotalDamage;
    public int MaxHealth => _playerInGameProps.MaxHealth;
    public int Health => _playerInGameProps.Health;
    public int MaxShield => _playerInGameProps.MaxShield;
    public bool IsBossRound => _playerInGameProps.IsBossRound;
    public int IDBossInRound => _playerInGameProps.IDBossInRound;
    public int Shield => _playerInGameProps.Shield;
    public int Hands => _playerInGameProps.Hands;
    public int Discards => _playerInGameProps.Discards;
    
    // level hand type 
    public HandTypePoker HandTypePokerClass => _playerInGameProps.HandTypePokerClass;
    public CountHandPlaying CountHandPlaying => _playerInGameProps.CountHandPlaying;
    public List<PokerCard> PokerCardInventory => _playerInGameProps.PokerCardInventory;
    public PlayerDataInGameModel Property
    {
        get
        {
            if (_playerInGameProps == null)
                _playerInGameProps = new PlayerDataInGameModel();
            return _playerInGameProps;
        }
    }
    // stat result game
    private float _bestScore;
    private List<int> _listValueHandPlayed = new List<int>();
    public List<int> ListValueHandPlayed => _listValueHandPlayed;
    public float BestScore => _bestScore;
    public void InitPlayerInGamePropertyData(string jsonPlayerInGameData)
    {
        bool isCreateDefaultData = false;

        if (string.IsNullOrEmpty(jsonPlayerInGameData))
        {
            _playerInGameProps = CreateDefaultPlayerInGameData();
            isCreateDefaultData = true;
        }
        else
        {
            try
            {
                _playerInGameProps = JsonReader.Deserialize<PlayerDataInGameModel>(jsonPlayerInGameData);
                DataManager.Instance.LogDebugPlayerInGameSave("Player InGame data", jsonPlayerInGameData);
            }
            catch (Exception e)
            {
                Debug.LogError("Cannot parse player props:" + e.Message);
                _playerInGameProps = CreateDefaultPlayerInGameData();
                isCreateDefaultData = true;
            }
        }
        if (isCreateDefaultData)
            SavePlayerInGameData();
    }
    public void SavePlayerInGameData()
    {
        string strPlayerInGameData = JsonWriter.Serialize(_playerInGameProps);
        DataManager.Instance.SavePlayerInGameData(strPlayerInGameData);
    }
    private PlayerDataInGameModel CreateDefaultPlayerInGameData()
    {
        return new PlayerDataInGameModel()
        {
            Uid = PlayerDataManager.Instance.Property.UserId,
            Chip = 0,
            Mult = 0,
            Discards = 4,
            Hands = 0,
            DollarInGame = 0,
            TotalDamage = 0,
            MaxHealth = 1000,
            Health = 1000,
            MaxShield = 500,
            Shield = 0,
            IsBossRound = false,
            IDBossInRound = 8888,
            ListPassiveActivated = new List<int>(),
            // level hand type 
            HandTypePokerClass = new HandTypePoker()
            {
                LevelHighCard = 1,
                LevelPair = 1,
                LevelTwoPair = 1,
                LevelThreeOfKind = 1,
                LevelFullHouse = 1,
                LevelStraight = 1,
                LevelFlush = 1,
                LevelFourOfKind = 1,
                LevelStraightFlush = 1,
                LevelRoyalFlush = 1,
                LevelFiveOfAKind = 1,
                LevelFlushHouse = 1,
                LevelFlushFive = 1,
            },
            CountHandPlaying = new CountHandPlaying()
            {
                CountHigh = 0,
                CountPair = 0,
                CountTwoPair = 0,
                CountThreeOfKind = 0,
                CountFullHouse = 0,
                CountStraight = 0,
                CountFlush = 0,
                CountFourOfKind = 0,
                CountRoyalFlush = 0,
                CountFiveOfAKind = 0,
                CountFlushHouse = 0,
                CountFlushFive = 0,
            },
            PokerCardInventory = new List<PokerCard>(),
          
        };
    }
    public override void Awake()
    {
        base.Awake();
        ConfigManager.Instance.LoadAllConfigLocal();
        DataManager.Instance.LoadAllData();

        OnInitCardDefault();
        EnemyController.Instance.OnInitEnemy();
        OnInitAllController();
    }
    private void OnInitCardDefault()
    {

        if (_playerInGameProps.PokerCardInventory.Count != 0)
        {
            foreach (var card in _lstCardOnHand)
            {
                card.isDisableCard = false;
            }

            foreach (var card in _lstFullDeck)
            {
                card.isDisableCard = false;
            }
            return;
        }
#if !UNITY_EDITOR
        if (PlayerDataManager.Instance.Property.Round != 1) return;
#endif
        
        // Init Card default
        foreach (SuitCard s in Enum.GetValues(typeof(SuitCard)))
        {
            if (s != SuitCard.None)
            {
                foreach (RankCard r in Enum.GetValues(typeof(RankCard)))
                {
                    if (r != RankCard.None)
                    {
                        PokerCard card = new PokerCard();
                        var data = ConfigManager.configValuePoints.GetValueByID((int)r);
                        card.PokerRank = r;
                        card.PokerSuit = s;
                        card.ChipValue = data.chip;
                        card.MultValue = data.mult;
                        _playerInGameProps.PokerCardInventory.Add(card);
                        card.isDisableCard = false;
                    }
                        
                }
            }
        }
    }
    public void RemoveCardInDeck(int index)
    {
        // if(_lstCardInDeck.Contains(card))
        _lstCardInDeck.RemoveAt(index);
    }
    public void RemoveCardInDeck(PokerCard pokerCard)
    {
        _lstCardInDeck.Remove(pokerCard);
    }
    #region global Method
    // ReSharper disable Unity.PerformanceAnalysis
    #region Poker Card Size
    public void InitCardToDeck()
    {
        _lstCardInDeck.Clear();
        _lstFullDeck.Clear();
        foreach (var card in  _playerInGameProps.PokerCardInventory)
        {
            _lstCardInDeck.Add(card);
            _lstFullDeck.Add(card);
        }
        Shuffle();
    }
    
    public void AddMorePokerCardToInventory(PokerCard poker)
    {
        _playerInGameProps.PokerCardInventory.Add(poker);
        SavePlayerInGameData();
    }
    public void RemovePokerCardToInventory(PokerCard poker)
    {
        if( _playerInGameProps.PokerCardInventory.Contains(poker))
            _playerInGameProps.PokerCardInventory.Remove(poker);
        SavePlayerInGameData();
    }
    private void Shuffle()
    {
        System.Random rng = new System.Random();
        int n = _lstCardOnHandScripts.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (_lstCardOnHandScripts[k], _lstCardOnHandScripts[n]) = (_lstCardOnHandScripts[n], _lstCardOnHandScripts[k]);
        }
    }
    #endregion

    public void StartGamePerRound()
    {
        _playerInGameProps.Chip = 0;
        _playerInGameProps.Mult = 0;
        _playerInGameProps.Hands = 0;
        _playerInGameProps.Discards = 4;
        _playerInGameProps.DollarInGame = 0;
        _playerInGameProps.TotalDamage = 0;
        _playerInGameProps.Shield = 0;
        _playerInGameProps.ListPassiveActivated = new List<int>();
        OnHealthChanged?.Invoke(0);
        OnShieldChanged?.Invoke(0);
        //save data
        SavePlayerInGameData();
    }

    public void ResetPoint()
    {
        AddChip(-_playerInGameProps.Chip);
        AddMult(-_playerInGameProps.Mult);
        DamageAttack(-_playerInGameProps.TotalDamage);
    }
    public bool OnAddToSelectCard(PokerCardVo card)
    {
        if (!_lstScriptsCardOnSelect.Contains(card))
        {
            _lstScriptsCardOnSelect.Add(card);
            return true;
        }
        return false;
    }
    public bool OnRemoveToSelectCard(PokerCardVo card)
    {
        if (_lstScriptsCardOnSelect.Contains(card))
        {
            _lstScriptsCardOnSelect.Remove(card);
            return true;
        }

        return false;
    }
    public void ClearSelectCard()
    {
        _lstScriptsCardOnSelect.Clear();
    }
    // public void CalHandPoker(PokerHandValue value)
    // {
    //     _listValueHandPlayed.Add((int)value);
    //     _playerInGameProps.
    // }
    public void  CalHandPoker(PokerHandValue value)
    {
        _listValueHandPlayed.Add((int)value);
        switch (value)
        {
            case PokerHandValue.None:
                return;
            case PokerHandValue.High:
                _playerInGameProps.CountHandPlaying.CountHigh += 1;
                break;
            case PokerHandValue.Pair:
                _playerInGameProps.CountHandPlaying.CountPair += 1;
                break;
            case PokerHandValue.TwoPair:
                _playerInGameProps.CountHandPlaying.CountTwoPair += 1;
                break;
            case PokerHandValue.ThreeOfAKind:
                _playerInGameProps.CountHandPlaying.CountThreeOfKind += 1;
                break;
            case PokerHandValue.Flush:
                _playerInGameProps.CountHandPlaying.CountFlush += 1;
                break;
            case PokerHandValue.Straight:
                _playerInGameProps.CountHandPlaying.CountStraight += 1;
                break;
            case PokerHandValue.FourOfAKind:
                _playerInGameProps.CountHandPlaying.CountFourOfKind += 1;
                break;
            case PokerHandValue.FullHouse:
                _playerInGameProps.CountHandPlaying.CountFullHouse += 1;
                break;
            case PokerHandValue.StraightFlush:
                _playerInGameProps.CountHandPlaying.CountStraightFlush += 1;
                break;
            case PokerHandValue.RoyalFlush:
                _playerInGameProps.CountHandPlaying.CountRoyalFlush += 1;
                break;
            case PokerHandValue.FiveOfAKind:
                _playerInGameProps.CountHandPlaying.CountFiveOfAKind += 1;
                break;
            case PokerHandValue.FlushHouse:
                _playerInGameProps.CountHandPlaying.CountFlushHouse += 1;
                break; 
            case PokerHandValue.FlushFive:
                _playerInGameProps.CountHandPlaying.CountFlushFive += 1;
                break;
        }
        SavePlayerInGameData();
    }
    public int GetCountPlaying(PokerHandValue value)
    {
        if (value == PokerHandValue.None) return 0;
        int countValue;
        countValue = value switch
        {
            PokerHandValue.High =>   _playerInGameProps.CountHandPlaying.CountHigh,
            PokerHandValue.Pair => _playerInGameProps.CountHandPlaying.CountPair,
            PokerHandValue.TwoPair => _playerInGameProps.CountHandPlaying.CountTwoPair,
            PokerHandValue.ThreeOfAKind => _playerInGameProps.CountHandPlaying.CountThreeOfKind,
            PokerHandValue.FourOfAKind => _playerInGameProps.CountHandPlaying.CountFourOfKind,
            PokerHandValue.FullHouse => _playerInGameProps.CountHandPlaying.CountFullHouse,
            PokerHandValue.Straight => _playerInGameProps.CountHandPlaying.CountStraight,
            PokerHandValue.Flush => _playerInGameProps.CountHandPlaying.CountFlush,
            PokerHandValue.StraightFlush => _playerInGameProps.CountHandPlaying.CountStraightFlush,
            PokerHandValue.RoyalFlush => _playerInGameProps.CountHandPlaying.CountRoyalFlush,
            PokerHandValue.FiveOfAKind => _playerInGameProps.CountHandPlaying.CountFiveOfAKind,
            PokerHandValue.FlushHouse => _playerInGameProps.CountHandPlaying.CountFlushHouse,
            PokerHandValue.FlushFive => _playerInGameProps.CountHandPlaying.CountFlushFive,
            _ => 0
        };
        return countValue;
    }
    // ReSharper disable Unity.PerformanceAnalysis
    public bool AddChip(float amount)
    {
        _playerInGameProps.Chip += amount;
        OnChipChanged?.Invoke(amount);
        return true;
    }
    public bool AddMult(float amount)
    {
        _playerInGameProps.Mult += amount;
        //Callback\
        Debug.Log("Add Mult: " + amount);
        OnMultChanged?.Invoke(amount);
        return true;
    }
    public void ClearChipAndMult()
    {
        _playerInGameProps.Mult -= _playerInGameProps.Mult;
        _playerInGameProps.Chip -= _playerInGameProps.Chip;
        OnMultChanged?.Invoke(0);
        OnChipChanged?.Invoke(0);
    }
    public bool MultiplyMult(float amount)
    {
        _playerInGameProps.Mult *= amount != 0 ? amount : 1;
        //Callback
        Debug.Log("Multiplied Mult by " + amount);
        OnMultChanged?.Invoke(amount);
        return true;
    }
    public bool DamageAttack(float amount)
    {
        if (amount < 0) return false;
        _playerInGameProps.TotalDamage += amount;
        Debug.Log(" Total Score: " + amount);
        OnRoundScoreChanged?.Invoke(amount);
        return true;
    }
    public bool PlayerTakeDame(int dame)
    {
        var calShield = _playerInGameProps.Shield - dame;
        if (calShield>= 0)
        {
            PlayerTakeDamageShield(dame);
        }
        else
        {
            PlayerTakeDamageShield(_playerInGameProps.Shield);
            PlayerTakeDamageHealth(-calShield);
            
        }
        return true;
    }

    private bool PlayerTakeDamageHealth(int hp)
    {
        _playerInGameProps.Health -= Mathf.Min(_playerInGameProps.Health, hp);
        OnHealthChanged?.Invoke(hp);
        return true;
    } 
    public bool PlayerTakeDamageShield(int shield)
    {
        _playerInGameProps.Shield -= Mathf.Min(_playerInGameProps.Shield, shield);
        OnShieldChanged?.Invoke(shield);
        return true;
    }
    public bool AddPlayerHealth(int hp)
    {
        _playerInGameProps.Health += Mathf.Min(_playerInGameProps.MaxHealth, hp);
        OnHealthChanged?.Invoke(hp);
        return true;
    }
    public bool AddPlayerShield(int shield)
    {
        _playerInGameProps.Shield += Mathf.Min(_playerInGameProps.MaxShield, shield);
		OnShieldChanged?.Invoke(shield);
		return true;
    }
    public bool AddDollar(int dollar)
    {
        // if (hp < 0) return false;
        _playerInGameProps.DollarInGame += dollar;
        OnDollarInGameChanged?.Invoke(dollar);
        return true;
    }
    public bool SubDiscard(int amount)
    {
        if (amount < 0) return false;
        _playerInGameProps.Discards -= amount;
        //Callback
        OnDiscardsChanged?.Invoke(amount);
        return true;
    }
    public bool SubAllDiscard()
    {
        _playerInGameProps.Discards -= _playerInGameProps.Discards;
        //Callback
        OnDiscardsChanged?.Invoke(0);
        return true;
    }
    public bool AddHands(int amount)
    {
        if (amount < 0) return false;
        _playerInGameProps.Hands += amount;
        //Callback
        OnHandsChanged?.Invoke(amount);
        return true;
    }
    public bool SubHands(int amount)
    {
        if (amount < 0) return false;
        _playerInGameProps.Hands -= amount;
        //Callback
        OnHandsChanged?.Invoke(amount);
        return true;
    }
    
    public void PlayCardIsDone(bool isDone)
    {
        IsDoneCalHandDeck?.Invoke(isDone);
    }
    public void CardStorage(PokerCardVo card)
    {
        if (!_listCardDroped.Contains(card))
        {
            _listCardDroped.Add(card);
        }
    }
    public void ClearCardStorage()
    {
        if (_listCardDroped != null)
        {
            _listCardDroped.Clear();
        }
           
    }
    public void AddCardToHand(PokerCard card)
    {
        if (card != null && !_lstCardOnHand.Contains(card))
        {
            _lstCardOnHand.Add(card);
        }
    }
    public void RemoveCardFromHand(PokerCard card)
    {
        if (card != null && _lstCardOnHand.Contains(card))
        {
            _lstCardOnHand.Remove(card);
        }
    }
    public void RandomRemoveCard(int amount)
    {
        
        // var disableCard = _lstCardOnHand.Where(x=> x.)
        List<PokerCardVo> randomScripts = _lstCardOnHandScripts.Where(x=>!_lstScriptsCardOnSelect.Contains(x)).OrderBy(x => Guid.NewGuid()).Take(amount).ToList();
        foreach (var script in randomScripts)
        {
            script.DiscardEffect();
            _lstCardOnHandScripts.Remove(script);
        }
    }
    public void ClearCardOnHand()
    {
        _lstCardOnHand.Clear();
    }
    public void AddScriptCardOnHand(PokerCardVo pokerCardVo)
    {
        if (pokerCardVo != null && !_lstCardOnHandScripts.Contains(pokerCardVo))
        {
            _lstCardOnHandScripts.Add(pokerCardVo);
        }
    }
    public void ReNewListCardOnHand()
    {
        _lstCardOnHandScripts = new List<PokerCardVo>();
    }
    public void SortListScriptCardVoByRank()
    {
        //attack to list . Be careful
        _lstCardOnHandScripts = _lstCardOnHandScripts?.OrderBy(child => child.Rank).ToList();
    }
    public void SortListScriptCardVoBySuit()
    {
        //attack to list . Be careful
        _lstCardOnHandScripts = _lstCardOnHandScripts?
            .OrderBy(child => child.Suit)
            .ThenBy(child => child.Rank).ToList();
    }
    public void RemoveScriptCardOnHand(PokerCardVo pokerCardVo)
    {
        if (pokerCardVo != null && _lstCardOnHandScripts.Contains(pokerCardVo))
        {
            _lstCardOnHandScripts.Remove(pokerCardVo);
        }
    }
    public void ClearScriptCardOnHand()
    {
        _lstCardOnHandScripts.Clear();
    }
    public void SetValueHand(PokerHandValue value)
    {
        if (value == PokerHandValue.None) return;
        _pokerHandValue = value;
    }
    public PokerHandValue GetValueHand()
    {
        if (_pokerHandValue == PokerHandValue.None) return PokerHandValue.Pair;
        return _pokerHandValue ;
    }
    public void AddPassiveActivated(int id)
    {
        if (!_playerInGameProps.ListPassiveActivated.Contains(id))
        {
            _playerInGameProps.ListPassiveActivated.Add(id);
        }
    }
    public void RemovePassiveActivated(int id)
    {
        if (_playerInGameProps.ListPassiveActivated.Contains(id))
        {
            _playerInGameProps.ListPassiveActivated.Remove(id);
        }
    }
    public bool CheckPassiveSpecial(string nameEnemy)
    {
        var getListPassive = _playerInGameProps.ListPassiveActivated;
        var getJokerByName = JokerCardController.Instance.GetJokerFromPool(nameEnemy);
        if (getJokerByName == null) return false;
        return  getListPassive?.Contains(getJokerByName.ID) ?? false;
    }
    public void AddCardPlayedToList(PokerCardVo card)
    {
        if(!_lstCardPlayed.Contains(card))
            _lstCardPlayed.Add(card);
    }
    
    public void OnResetData()
    {
        _lstCardOnHand = new List<PokerCard>();
        _lstCardInDeck = new List<PokerCard>();
        _lstScriptsCardOnSelect = new List<PokerCardVo>();
        // Init Back
        OnInitCardDefault();
    }  

    #region make data for result game 
    public void SetBestScore(float score)
    {
        if (score > _bestScore || _bestScore == 0f)
        {
            _bestScore = score;
        }
    }
    public PokerHandValue GetMostValueHand()
    {
        if (_listValueHandPlayed.Count == 0) return PokerHandValue.None;
        int mostFrequentNumber = _listValueHandPlayed
            .GroupBy(x => x)
            .OrderByDescending(g => g.Count()) 
            .First()
            .Key;
        return (PokerHandValue)mostFrequentNumber;
    }
    public int GetMostValueHandCount()
    {
        int mostFrequentNumber = _listValueHandPlayed
            .GroupBy(x => x)
            .OrderByDescending(g => g.Count()) 
            .First()
            .Key;
        int frequency = _listValueHandPlayed.Count(x => x == mostFrequentNumber);
        return frequency;
    }
    #endregion
    public int GetQuantityValuePokerOnDeck(object param)
    {
        var count = 0;
        if (param is RankCard)
            count = _lstCardInDeck.Count(x => x.PokerRank == (RankCard)param);
        if (param is SuitCard)
            count = _lstCardInDeck.Count(x => x.PokerSuit == (SuitCard)param);
        if (param is EnhancementsModifyCardEnum)
            count = _lstCardInDeck.Count(x => x.pokerEnhancement == (EnhancementsModifyCardEnum)param);
        if (param is EditionsModifyCardEnum)
            count = _lstCardInDeck.Count(x => x.pokerEdition == (EditionsModifyCardEnum)param);
        if (param is SealsModifyCardEnum)
            count = _lstCardInDeck.Count(x => x.pokerSeal == (SealsModifyCardEnum)param);
            
        return count;
    }
    #endregion
    #region local Method
    private void OnInitAllController()
    {
        ResultController.Instance.OnInit();
    }
    public bool LevelUpPokerHand(int idHand)
    {
        PokerHandValue value = (PokerHandValue)idHand;
        switch (value)
        {
            case PokerHandValue.None:
                return false;
            case PokerHandValue.High:
                _playerInGameProps.HandTypePokerClass.LevelHighCard += 1;
                break;
            case PokerHandValue.Pair:
                _playerInGameProps.HandTypePokerClass.LevelPair += 1;
                break;
            case PokerHandValue.TwoPair:
                _playerInGameProps.HandTypePokerClass.LevelTwoPair += 1;
                break;
            case PokerHandValue.ThreeOfAKind:
                _playerInGameProps.HandTypePokerClass.LevelThreeOfKind += 1;
                break;
            case PokerHandValue.Flush:
                _playerInGameProps.HandTypePokerClass.LevelFlush += 1;
                break;
            case PokerHandValue.Straight:
                _playerInGameProps.HandTypePokerClass.LevelStraight += 1;
                break;
            case PokerHandValue.FourOfAKind:
                _playerInGameProps.HandTypePokerClass.LevelFourOfKind += 1;
                break;
            case PokerHandValue.FullHouse:
                _playerInGameProps.HandTypePokerClass.LevelFullHouse += 1;
                break;
            case PokerHandValue.StraightFlush:
                _playerInGameProps.HandTypePokerClass.LevelStraightFlush += 1;
                break;
            case PokerHandValue.RoyalFlush:
                _playerInGameProps.HandTypePokerClass.LevelRoyalFlush += 1;
                break;
            case PokerHandValue.FiveOfAKind:
                _playerInGameProps.HandTypePokerClass.LevelFiveOfAKind += 1;
                break;
            case PokerHandValue.FlushHouse:
                _playerInGameProps.HandTypePokerClass.LevelFlushHouse += 1;
                break; 
            case PokerHandValue.FlushFive:
                _playerInGameProps.HandTypePokerClass.LevelFlushFive += 1;
                break;
        }
        SavePlayerInGameData();
        return true;
    }
    public string GetNameByPokerValueHand(PokerHandValue value)
    {
        if (value == PokerHandValue.None) return "None";
        string nameValue;
        nameValue = value switch
        {
            PokerHandValue.High => "High",
            PokerHandValue.Pair => "Pair",
            PokerHandValue.TwoPair => "Two Pair",
            PokerHandValue.ThreeOfAKind => "Three Of A Kind",
            PokerHandValue.FourOfAKind => "Four Of A Kind",
            PokerHandValue.FullHouse => "Full House",
            PokerHandValue.Straight => "Straight",
            PokerHandValue.Flush => "Flush",
            PokerHandValue.StraightFlush => "Straight Flush",
            PokerHandValue.RoyalFlush => "Royal Flush",
            PokerHandValue.FiveOfAKind => "Five Of A Kind",
            PokerHandValue.FlushHouse => "Flush House",
            PokerHandValue.FlushFive => "Flush Five",
            _ => "None"
        };
        return nameValue;
    }
    public int GetLevelByPokerValueHand(PokerHandValue value)
    {
        if (value == PokerHandValue.None) return 0;
        int level = value switch
        {
            PokerHandValue.High => _playerInGameProps.HandTypePokerClass.LevelHighCard,
            PokerHandValue.Pair => _playerInGameProps.HandTypePokerClass.LevelPair,
            PokerHandValue.TwoPair => _playerInGameProps.HandTypePokerClass.LevelTwoPair,
            PokerHandValue.ThreeOfAKind => _playerInGameProps.HandTypePokerClass.LevelThreeOfKind,
            PokerHandValue.FourOfAKind => _playerInGameProps.HandTypePokerClass.LevelFourOfKind,
            PokerHandValue.FullHouse => _playerInGameProps.HandTypePokerClass.LevelFullHouse,
            PokerHandValue.Straight => _playerInGameProps.HandTypePokerClass.LevelStraight,
            PokerHandValue.Flush => _playerInGameProps.HandTypePokerClass.LevelFlush,
            PokerHandValue.StraightFlush => _playerInGameProps.HandTypePokerClass.LevelStraightFlush,
            PokerHandValue.RoyalFlush => _playerInGameProps.HandTypePokerClass.LevelRoyalFlush,
            PokerHandValue.FiveOfAKind => _playerInGameProps.HandTypePokerClass.LevelFiveOfAKind,
            PokerHandValue.FlushHouse => _playerInGameProps.HandTypePokerClass.LevelFlushHouse,
            PokerHandValue.FlushFive => _playerInGameProps.HandTypePokerClass.LevelFlushFive,
            _ => 0
        };
        return level;
    }
    public PointChipAndMult CalValueHandPoker(PokerHandValue value)
    {
        var data = ConfigManager.configValueHands.GetValueByID((int)value);
        PointChipAndMult points = new PointChipAndMult();
        // bonus level here
        var bonusChip = data.upPerLevelChip;
        var bonusMult = data.upPerLevelMult;
        if (value != PokerHandValue.None)
        {
            int currentLevel = GetLevelByPokerValueHand(value);
          
            bonusChip *= currentLevel == 1 ? 0 : currentLevel - 1;
            bonusMult *= currentLevel == 1 ? 0 : currentLevel - 1;
        }
            
        points.pokerChip = data.chip + bonusChip;
        points.pokerMult = data.mult + bonusMult;
        return points;
    }
    public void SaveDamePerTurn(float damage)
    {
        _totalDmgPerTurn = damage;
    }
    public void ResetDamePerTurn()
    {
        _totalDmgPerTurn = 0;
    }

    public void SaveIDBoss(int idBoss)
    {
        _playerInGameProps.IDBossInRound = idBoss;
        _playerInGameProps.IsBossRound = idBoss != 0;
        SavePlayerInGameData();
    }

    public void ClearDataPlayInGame()
    {
        _playerInGameProps = CreateDefaultPlayerInGameData();
        SavePlayerInGameData();
    }
    #endregion
        
        
}