using System;
using System.Collections.Generic;
using System.Globalization;
using Core.Entity;
using Core.Manager;
using Core.Utils;
using System.Linq;
using System.Threading.Tasks;
using Core.Model;
using Enemy;
using Entity;
using InGame;
using MainMenu;
using Manager;
using UnityEngine;
using static UnityEngine.Mathf;

namespace Runtime.Manager
{
    public class EnemyController : Singleton<EnemyController>
    {
        private List<EnemyCardVo> _listEnemyScript = new List<EnemyCardVo>();
        private Dictionary<int, EnemyCardVo> _dictEnemyScript = new Dictionary<int, EnemyCardVo>();
        private Dictionary<int, List<PokerCard>> _dictEnemyUsedCard= new Dictionary<int, List<PokerCard>>();
        private List<PokerCard> _listCardInDeckEnemies = new List<PokerCard>();
        private List<PokerCard> _lstCardOnHandEnemy = new List<PokerCard>();
        private List<int> _lstEnemyId = new List<int>();
        private List<EnemySo> _lstEnemySo = new List<EnemySo>();
        private Dictionary<uint, EnemyModel> _dictAllEnemyModel = new Dictionary<uint, EnemyModel>();
        public int TotalDmgSendToPlayer { get; set; }
        public List<EnemyCardVo> ListEnemyScript => _listEnemyScript;
        public List<PokerCard> LstDefaultCardEnemies => _listCardInDeckEnemies;
        public List<PokerCard> LstCardOnHandEnemy => _lstCardOnHandEnemy;
        public Dictionary<int, EnemyCardVo> DictEnemyScript => _dictEnemyScript;
        
        public Dictionary<uint, EnemyModel> DictAllEnemyModel => _dictAllEnemyModel;
        
        public int IdEnemySelected { get; set; }
        public List<int> ListEnemyId => _lstEnemyId;
        public List<EnemySo> ListEnemies => _lstEnemySo;
        private PokerHandValue _valueHandEnum = PokerHandValue.None;
        public void OnInitEnemy()
        {
            EnemySo[] cardSOs  = Resources.LoadAll<EnemySo>("SO/Enemy");
            cardSOs = cardSOs.Where(x => x.active).ToArray();
            foreach (var enemySo in cardSOs)
            {
                EnemyModel model = new EnemyModel
                {
                    ID = enemySo.id,
                    Name = enemySo.enemyName,
                    Health = enemySo.baseHealth,
                    MultiplierHealth = enemySo.multiplierHealth,
                    DrawCards = enemySo.drawCard,
                    TypeEnemyEnum = enemySo.typeEnemy,
                    IconBoss = enemySo.iconBoss,
                    ListValueHandLevelConfig = enemySo.listValueHandLevelConfig,
                    PassiveNameEnemy = enemySo.passiveNameEnemy,
                    TriggerPhaseEnemy = enemySo.triggerPhaseEnemy,
                    Active = enemySo.active
                };
                
                _dictAllEnemyModel.Add(enemySo.id, model);
                _lstEnemySo.Add(enemySo);
            }
        }
        public void InitEnemy(int index, EnemyCardVo enemy)
        {
            if(!_listEnemyScript.Contains(enemy))
            {
                _listEnemyScript.Add(enemy);
            }
            _dictEnemyScript.TryAdd(index, enemy);
            IdEnemySelected = -1;
        }
        public List<PokerCard> OnGenerateCardInDeck(EnemySo enemySo)
        {
            List<PokerCard> listCard = new List<PokerCard>();
            foreach (SuitCard suit in Enum.GetValues(typeof(SuitCard)))
            {
                if (suit == SuitCard.None) continue;
                foreach (RankCard rank in Enum.GetValues(typeof(RankCard)))
                {
                    if (rank == RankCard.None) continue;
                    PokerCard card = new PokerCard();
                    var data = ConfigManager.configValuePoints.GetValueByID((int)rank);
                    if (enemySo.enemyTagsEnum.Contains(EnemyTagsEnum.Modified))
                    {
                        if (enemySo.passiveNameEnemy == EnemyCardData.PassiveEffectEnemy.DiamondBeast)
                        {
                            card.PokerRank = rank;
                            card.PokerSuit = suit == SuitCard.Heart ||suit == SuitCard.Spade || suit == SuitCard.Club ? SuitCard.Diamond : suit;
                            card.ChipValue = data.chip;
                            card.MultValue = data.mult;
                            listCard.Add(card);
                        }
                        if (enemySo.passiveNameEnemy == EnemyCardData.PassiveEffectEnemy.RoyalGuard)
                        {
                            RankCard[] poolAvoid =
                            {
                                RankCard.Two, RankCard.Three, RankCard.Four,  RankCard.Five
                            };
                            if (!poolAvoid.Contains(rank))
                            {
                                card.PokerRank = rank;
                                card.PokerSuit = suit;
                                card.ChipValue = data.chip;
                                card.MultValue = data.mult;
                                listCard.Add(card);
                            }
                        }
						if (enemySo.passiveNameEnemy == EnemyCardData.PassiveEffectEnemy.RoyalKing)
						{
							RankCard[] poolAvoid =
							{
								RankCard.Two, RankCard.Three, RankCard.Four,  RankCard.Five, RankCard.Six, RankCard.Seven, RankCard.Eight, RankCard.Nine, RankCard.Ten
							};
							if (!poolAvoid.Contains(rank))
							{
								card.PokerRank = rank;
								card.PokerSuit = suit;
								card.ChipValue = data.chip;
								card.MultValue = data.mult;
								listCard.Add(card);
							}
						}
                        if (enemySo.passiveNameEnemy == EnemyCardData.PassiveEffectEnemy.HerMajesty)
                        {
                            RankCard[] poolAvoid =
                            {
                                RankCard.Two, RankCard.Three, RankCard.Four,  RankCard.Five, RankCard.Six, RankCard.Seven, RankCard.Eight, RankCard.Nine, RankCard.Ten
                            };
                            if (!poolAvoid.Contains(rank))
                            {
                                card.PokerRank = rank;
                                card.PokerSuit = suit;
                                card.ChipValue = data.chip;
                                card.MultValue = data.mult;
                                listCard.Add(card);
                            }
                        }
                        if (enemySo.passiveNameEnemy == EnemyCardData.PassiveEffectEnemy.TheBlackKnight)
                        {
                            card.PokerRank = rank;
                            card.PokerSuit = suit;
                            card.ChipValue = data.chip;
                            card.MultValue = data.mult;
                            listCard.Add(card);
                        }
                        if (enemySo.passiveNameEnemy == EnemyCardData.PassiveEffectEnemy.KingOfKings)
                        {
                            card.PokerRank = rank;
                            card.PokerSuit = suit;
                            card.ChipValue = data.chip;
                            card.MultValue = data.mult;
                            listCard.Add(card);
                        }
                        if (enemySo.passiveNameEnemy == EnemyCardData.PassiveEffectEnemy.TheCardManiac)
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                card.PokerRank = rank;
                                card.PokerSuit = suit;
                                card.ChipValue = data.chip;
                                card.MultValue = data.mult;
                                listCard.Add(card);
                            }
                            
                        }
                        if (enemySo.passiveNameEnemy == EnemyCardData.PassiveEffectEnemy.TheSlimeGirl)
                        {
                            card.PokerRank = rank;
                            card.PokerSuit = suit;
                            card.ChipValue = data.chip;
                            card.MultValue = data.mult;
                            listCard.Add(card);
                        }
                        if (enemySo.passiveNameEnemy == EnemyCardData.PassiveEffectEnemy.TheStrictMaid)
                        {
                            card.PokerRank = rank;
                            card.PokerSuit = suit;
                            card.ChipValue = data.chip;
                            card.MultValue = data.mult;
                            listCard.Add(card);
                        }
					}
                    else
                    {
                        card.PokerRank = rank;
                        card.PokerSuit = suit;
                        card.ChipValue = data.chip;
                        card.MultValue = data.mult;
                        listCard.Add(card);
                    }
                }
            }
            return listCard;
        }
        public void InitEnemyPerRound()
        {
            var round = PlayerDataManager.Instance.Property.Round % (ConfigManager.configPlayRound.Records.Count - 1) == 0 ?
                ConfigManager.configPlayRound.Records.Count() - 1 : PlayerDataManager.Instance.Property.Round % (ConfigManager.configPlayRound.Records.Count - 1) ;
            var configRound = ConfigManager.configPlayRound.GetValueByRound(round);
            if (PlayerDataManager.Instance.Property.IsCompletedTutorial)
            {
                var listEnemy = GameObjectUtils.Instance.ConvertStringToList(configRound.enemyID);
                var getRandomBoss = listEnemy.FirstOrDefault(x => x == EnemyCardData.RandomBossPool.PoolRandomBossAll);
                if (getRandomBoss != 0 && PlayerDataManager.Instance.Property.IsCompletedFirstPlay)
                {
                    int idBoss = 8888;
                    if (PlayCardController.Instance.Property.IsBossRound)
                    {
                        idBoss = PlayCardController.Instance.Property.IDBossInRound;
                    }
                    else
                    {
                        var randomBoss = _lstEnemySo.Where(x=>x.typeEnemy == EnemyCardData.TypeEnemyEnum.Boss).OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                        if (randomBoss != null) idBoss = (int)randomBoss.id;
                        PlayCardController.Instance.SaveIDBoss(idBoss);
                    }
                    int indexToReplace = listEnemy.IndexOf(getRandomBoss);
                    if (indexToReplace == -1) return;
                    listEnemy[indexToReplace] = idBoss;
                    _lstEnemyId = listEnemy;
                    
                }
                else
                {
                   var getEnemy  = GameObjectUtils.Instance.ConvertStringToList(configRound.enemyID);
                   if (getEnemy.Any(x => x == EnemyCardData.RandomBossPool.PoolRandomBossAll))
                   {
                       var config =  ConfigManager.configPlayRound.GetValueByRound(PlayerDataManager.Instance.Property.Round % (ConfigManager.configPlayRound.Records.Count - 1));
                       int indexToReplace = getEnemy.IndexOf(EnemyCardData.RandomBossPool.PoolRandomBossAll);
                       getEnemy[indexToReplace] = config.firstBoss;
                   }
                    _lstEnemyId = getEnemy;
                }
            }
            else
            {
                _lstEnemyId = GameObjectUtils.Instance.ConvertStringToList("9999");
            }
        }
        public List<PokerCard> GetBestHand(List<PokerCard>  listCard)
        {
            List<PokerCard> bestHand = new List<PokerCard>();
            List<PokerCard> sortedCards = listCard.OrderByDescending(x => x.PokerRank).ToList();
            
            if (IsStraightFlush(sortedCards))
            {
                bestHand = GetStraightFlush(sortedCards);
                _valueHandEnum = PokerHandValue.StraightFlush;
                return bestHand;
            }
            else if (IsFourOfAKind(sortedCards))
            {
                bestHand = GetFourOfAKind(sortedCards);
                _valueHandEnum = PokerHandValue.FourOfAKind;
                return bestHand;
            }
            else if (IsFullHouse(sortedCards))
            {
                bestHand = GetFullHouse(sortedCards);
                _valueHandEnum = PokerHandValue.FullHouse;
                return bestHand;
            }
            else if (IsFlush(sortedCards))
            {
                bestHand = GetFlush(sortedCards);
                _valueHandEnum = PokerHandValue.Flush;
                return bestHand;
            }
            else if (IsStraight(sortedCards))
            {
                bestHand = GetStraight(sortedCards);
                _valueHandEnum = PokerHandValue.Straight;
                return bestHand;
            }
            else if (IsThreeOfAKind(sortedCards))
            {
                bestHand = GetThreeOfAKind(sortedCards);
                _valueHandEnum = PokerHandValue.ThreeOfAKind;
                return bestHand;
            }
            else if (IsTwoPairs(sortedCards))
            {
                bestHand = GetTwoPairs(sortedCards);
                _valueHandEnum = PokerHandValue.TwoPair;
                return bestHand;
            }
            else if (IsOnePair(sortedCards))
            {
                bestHand = GetOnePair(sortedCards);
                _valueHandEnum = PokerHandValue.Pair;
                return bestHand;
            }
            else
            {
                bestHand.Add(sortedCards.FirstOrDefault());
                _valueHandEnum = PokerHandValue.High;
                return bestHand;
            }
        }
        public float CalculateDamage(float totalChip, float totalMult,EnemyCardVo enemyVo)
        {
            var cm = CalValueHandPoker(_valueHandEnum, GetLevelByPokerValueHand(_valueHandEnum, enemyVo));
            float chip = totalChip + cm.pokerChip;
            float mult = totalMult + cm.pokerMult;
            var total = chip * (mult != 0 ? mult : 1);
            return total;
        }
        public void EnemyEffectStartGame()
        {
            foreach (var enemy in _listEnemyScript)
            {
                if (enemy.EnemySO.passiveNameEnemy == EnemyCardData.PassiveEffectEnemy.CardSlicer)
                {
                    if(enemy.Property.Health > 0)
                        PlayCardController.Instance.RandomRemoveCard((int)enemy.EnemySO.value);
                }
            }
        }
        public void ResetState()
        {
            _valueHandEnum = PokerHandValue.None;
        }
        public bool IsAllEnemyDeath()
        {
            bool allLessThanZero = _listEnemyScript.All(x => x.Property.Health <= 0.99f && x.Property.IsUsePassive == true);
            return allLessThanZero;
        }
        public void ModifyStatsEnemy(int index, float damage, Action<bool> isDeath = default)
        {
            foreach (var enemy in _dictEnemyScript)
            {
                if (enemy.Key  == index)
                {
                    enemy.Value.Property.Health += damage;
                    if (enemy.Value.Property.Health <= 0.99f)
                    {
                        if (enemy.Value != null)
                        {
                          
                            enemy.Value.OnDefeat();
                            isDeath?.Invoke(true);
                        }
                    }
                    else
                    {
                        isDeath?.Invoke(false);
                    }
                  
                    EffectCanvasManager.Instance.ShowEffectTotalDame(enemy.Value.transform as RectTransform, damage, damage < -400);
                }
                
            }
        }
        public EnemyCardVo GetComponentByIdEnemy()
        {

            if (IdEnemySelected == -1 && _listEnemyScript.Count == 1)
            {
                return _listEnemyScript.FirstOrDefault();
            }
            return _listEnemyScript.FirstOrDefault(x => x.GetIndexEnemy == IdEnemySelected);
        }
        public void ClearDataEnemy()
        {
            _dictEnemyScript.Clear();
            _dictEnemyUsedCard.Clear();
            _lstEnemyId.Clear();
            _listEnemyScript.Clear();
            IdEnemySelected = - 1;
        }
        public void AddEnemyCardToStorage( int idEnemy,PokerCard card)
        {
            if (_dictEnemyUsedCard.ContainsKey(idEnemy))
            {
                _dictEnemyUsedCard[idEnemy].Add(card);
            }
            else
            {
                _dictEnemyUsedCard[idEnemy] = new List<PokerCard> { card };
            }
        }
        public EnemySo GetEnemyById(int idEnemy)
        {
            var enemySo = _lstEnemySo.FirstOrDefault(x => x.id == idEnemy);
            return enemySo;
        }
        public void RemoveCardOnHandEnemy(PokerCard card, EnemyCardVo enemy)
        {
            enemy.RemoveCardFromHand(card);
        }
        public string HaveBossInRoundAndReTurnNameRound()
        {
            foreach (var idE in _lstEnemyId)
            {
                var enemySo = _lstEnemySo.FirstOrDefault(x => x.id == idE);
                if (enemySo != null && enemySo.typeEnemy == EnemyCardData.TypeEnemyEnum.Boss)
                {
                    return enemySo.nameRoundBoss;
                }
            }
            return null;
        }
        public int ScaleMaxHealthEnemy(int baseHealth, int multiplier)
        {
            int health = baseHealth * multiplier;
            int threshold = 10;
            var round = PlayerDataManager.Instance.Property.Round % 10 == 0
                ? 10 : PlayerDataManager.Instance.Property.Round;
            // ReSharper disable once PossibleLossOfFraction
            int multiplierPerRound = (int)Pow(10, FloorToInt((round - 1) / threshold));
            return health * multiplierPerRound;
        }
        public List<EnemyCardData.EnemyLevelHandModify> ScaleHandLevelEnemy(List<EnemyCardData.EnemyLevelHandModify> listLevel)
        {
            List<EnemyCardData.EnemyLevelHandModify> listModified = new List<EnemyCardData.EnemyLevelHandModify>();
            int threshold = 10;
            foreach (var handValue in listLevel)
            {
                var handModified = new EnemyCardData.EnemyLevelHandModify();
                var round = PlayerDataManager.Instance.Property.Round % 10== 0
                    ? PlayerDataManager.Instance.Property.Round % 10 : PlayerDataManager.Instance.Property.Round;
                // ReSharper disable once PossibleLossOfFraction
                int multiplierPerRound = (int)Pow(1, FloorToInt((round - 1) / threshold));
                handModified.handValueLevel = handValue.handValueLevel + multiplierPerRound;
                handModified.scaleHandValue = handValue.scaleHandValue;
                handModified.pokerHandValue = handValue.pokerHandValue;
                
                listModified.Add(handModified);
            }
            
            return listModified;
        }
        public void UnSelectEnemyScript()
        {
            if (_listEnemyScript == null) return;
            foreach (var sCardVo in _listEnemyScript)
            {
                sCardVo.DeSelectData();
            }
        }
        private PointChipAndMult CalValueHandPoker(PokerHandValue value,int level)
        {
            var data = ConfigManager.configValueHands.GetValueByID((int)value);
            PointChipAndMult points = new PointChipAndMult();
            // bonus level here
            var bonusChip = data.upPerLevelChip;
            var bonusMult = data.upPerLevelMult;
            if (value != PokerHandValue.None)
            {
                bonusChip *= level == 1 ? 0 : level - 1;
                bonusMult *= level == 1 ? 0 : level - 1;
            }
            
            points.pokerChip = data.chip + bonusChip;
            points.pokerMult = data.mult + bonusMult;
            return points;
        }
        private int GetLevelByPokerValueHand(PokerHandValue handValue,EnemyCardVo enemyVo)
        {
            var data = enemyVo.EnemySO.listValueHandLevelConfig;
            var config = data.FirstOrDefault(x => x.pokerHandValue == handValue);
            if (config != null)
            {
                var extraLevel = PlayerDataManager.Instance.Property.Round / 10;
                return config.handValueLevel != 0 ? config.handValueLevel + extraLevel : 1;
            }
            return 1;
        }
        
#region  set Hand Enemy
    private bool IsStraightFlush(List<PokerCard> cards)
    {
        return IsStraight(cards) && IsFlush(cards);
    }
    private List<PokerCard> GetStraightFlush(List<PokerCard> cards)
    {
        List<PokerCard> straightFlush = new List<PokerCard>();

        List<PokerCard> straight = GetStraight(cards);
        straightFlush.AddRange(straight);
        return straightFlush;
    }
    private bool IsFourOfAKind(List<PokerCard> cards)
    {
        return cards.GroupBy(card => card.PokerRank).Any(group => group.Count() == 4);
    }

    private List<PokerCard> GetFourOfAKind(List<PokerCard> cards)
    {
        return cards.GroupBy(card => card.PokerRank).Where(group => group.Count() == 4).SelectMany(group => group).ToList();
    }

    private bool IsFullHouse(List<PokerCard> cards)
    {
        return IsThreeOfAKind(cards) && IsOnePair(cards);
    }

    private List<PokerCard> GetFullHouse(List<PokerCard> cards)
    {
        List<PokerCard> fullHouse = new List<PokerCard>();

        List<PokerCard> threeOfAKind = GetThreeOfAKind(cards);
        fullHouse.AddRange(threeOfAKind);

        List<PokerCard> remainingCards = cards.Except(threeOfAKind).ToList();
        List<PokerCard> onePair = GetOnePair(remainingCards);
        fullHouse.AddRange(onePair);

        return fullHouse;
    }

    private bool IsFlush(List<PokerCard> cards)
    {
        return cards.GroupBy(card => card.PokerSuit).Any(group => group.Count() >= 5);
    }

    private List<PokerCard> GetFlush(List<PokerCard> cards)
    {
        return cards.GroupBy(card => card.PokerSuit).Where(group => group.Count() >= 5).SelectMany(group => group).Take(5).ToList();
    }

    private bool IsStraight(List<PokerCard> cards)
    {
        int distinctRanks = cards.Select(card => (int)card.PokerRank).Distinct().Count();
        return distinctRanks == 5 && (cards.Max(card => (int)card.PokerRank) - cards.Min(card => (int)card.PokerRank)) == 4;
    }

    private List<PokerCard> GetStraight(List<PokerCard> cards)
    {
        List<PokerCard> straight = new List<PokerCard>();

        // Nếu có Ace (Rank = 14) và làm Straight từ Ace đến 5
        if (cards.Any(card => card.PokerRank == RankCard.Ace) && cards.Any(card => card.PokerRank == RankCard.Five))
        {
            straight.AddRange(cards.Where(card => card.PokerRank == RankCard.Ace || card.PokerRank == RankCard.King || card.PokerRank == RankCard.Queen || card.PokerRank == RankCard.Jack || card.PokerRank == RankCard.Ten));
        }
        else
        {
            int minRank = cards.Min(card => (int)card.PokerRank);
            straight.AddRange(cards.Where(card => (int)card.PokerRank >= minRank && (int)card.PokerRank <= minRank + 4));
        }

        return straight;
    }

    private bool IsThreeOfAKind(List<PokerCard> cards)
    {
        return cards.GroupBy(card => card.PokerRank).Any(group => group.Count() == 3);
    }

    private List<PokerCard> GetThreeOfAKind(List<PokerCard> cards)
    {
        return cards.GroupBy(card => card.PokerRank).Where(group => group.Count() == 3).SelectMany(group => group).ToList();
    }

    private bool IsTwoPairs(List<PokerCard> cards)
    {
        return cards.GroupBy(card => card.PokerRank).Count(group => group.Count() == 2) == 2;
    }

    private List<PokerCard> GetTwoPairs(List<PokerCard> cards)
    {
        List<PokerCard> twoPairs = new List<PokerCard>();

        List<IGrouping<RankCard, PokerCard>> pairs = cards.GroupBy(card => card.PokerRank).Where(group => group.Count() == 2).ToList();
        foreach (var pair in pairs)
        {
            twoPairs.AddRange(pair);
        }

        return twoPairs;
    }

    private bool IsOnePair(List<PokerCard> cards)
    {
        return cards.GroupBy(card => card.PokerRank).Any(group => group.Count() == 2);
    }

    private List<PokerCard> GetOnePair(List<PokerCard> cards)
    {
        return cards.GroupBy(card => card.PokerRank).Where(group => group.Count() == 2).SelectMany(group => group).ToList();
    }

    #endregion

    }
}