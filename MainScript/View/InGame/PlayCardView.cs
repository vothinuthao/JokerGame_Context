 using System;
 using System.Collections.Generic;
 using System.Globalization;
 using System.Linq;
using System.Threading.Tasks;
using Core.Controller.Observer;
using Core.Entity;
using Core.Manager;
using Core.Observer;
using Core.Utils;
using DG.Tweening;
using Enemy;
using Entity;
using Frameworks.Base;
using Frameworks.Scripts;
using Frameworks.UIAlert;
using Frameworks.UIPopup;
using Frameworks.Utils;
using Manager;
using Player;
using Runtime.JokerCard;
using Runtime.Manager;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using Tutorials;
using UI.BaseColor;
using UI.Popups;
using UI.Popups.ClassParameter;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InGame
{
	public class PlayCardView : MonoBehaviour
	{
		[SerializeField] private PokerHandView pokerHandView;
		[SerializeField] private PokerPlayView pokerPlayView;
		#region variable 
		[FoldoutGroup("Spawn Project Group")]
		public GameObject cardPrefab;
		[FoldoutGroup("Spawn Project Group")]
		public Transform _content;
		[FoldoutGroup("Spawn Project Group")]
		public Transform _contentSpawn;
		public Transform transDisCard;
		[SerializeField]
		private TextMeshProUGUI _txtAmountInDeck;
		[SerializeField]
		private TextMeshProUGUI _txtValueHand;
		[SerializeField]
		private TextMeshProUGUI _txtLevelValueHand;
		[SerializeField] private EnemyPlaceView _enemyPlaceView;
		[FoldoutGroup("UI Information")]
		[SerializeField] private GameObject signPlayerTurn;
		[FoldoutGroup("UI Information")]
		[SerializeField] private GameObject signEnemyTurn;
		[FoldoutGroup("UI Information")]
		[SerializeField] private TextMeshProUGUI txtSortDeckOnHand;
		[FoldoutGroup("UI Information")]
		[SerializeField] private GameObject cardBackDeck;
		[FoldoutGroup("UI Information")]
		[SerializeField] private TextMeshProUGUI txtAmountCardOnHand;
		#endregion
		private List<PokerCardVo> _lstPlayDeckVo = new List<PokerCardVo>();
		private List<PokerCard> _defaultDeck = new List<PokerCard>();
		private List<PokerCardVo> _lstCardSelectingDeck = new List<PokerCardVo>();
		private List<PokerCard> _lstSetValue = new List<PokerCard>();
		private PlayCardController _playCardController;
		private DateTime _startTime;
		private float _startHpPlayer;
		private int _watchAdsForReviveTime;
		
		private void Start()
		{
			cardPrefab.SetActive(false);
			UInformation(PlayerPrefs.GetInt("SortType"));
		}
		public void Update()
		{
#if UNITY_EDITOR
			if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.A))
			{
				OnPlayCard();
			}
			if (Input.GetKeyDown(KeyCode.D))
			{
				OnDiscard();
			}
#endif
		}
		public static int SortType
		{
			get { return PlayerPrefs.GetInt("SortType"); }
			set { PlayerPrefs.SetInt("SortType", value); }
		}
		private void OnEnable()
		{
			EventDispatcher.Instance?.RegisterListener(EventID.OnChangeStateGamePlay, OnChangeStateGamePlay);
			EventDispatcher.Instance?.RegisterListener(EventID.OnActiveTutorial, OnActiveTutorial);
		}
		private void OnDisable()
		{
			EventDispatcher.Instance?.RemoveListener(EventID.OnChangeStateGamePlay, OnChangeStateGamePlay);
			EventDispatcher.Instance?.RemoveListener(EventID.OnActiveTutorial, OnActiveTutorial);
		}
		public void InitDataPlayView()
		{
			_playCardController = PlayCardController.Instance;
			SortType = 0;
			signEnemyTurn.SetActive(false);
			_txtValueHand.gameObject.SetActive(false);
			StateMachineController.Instance.ChangeState(GameState.PlayerTurn);
		}
		public async Task PlayGame()
		{
			StateMachineController.Instance.ChangeState(GameState.PlayerTurn);
			PlayCardController.Instance.ClearScriptCardOnHand();
			pokerHandView.ClearAllChildCard();
			_playCardController ??= PlayCardController.Instance;
			PlayCardController.Instance.OnResetData();
			PlayCardController.Instance.StartGamePerRound();
			_playCardController.ClearCardOnHand();
			_playCardController.InitCardToDeck();
			JokerCardController.Instance.OnUpdateUI();
			CreateNewEnemy();
			OpenBossCutScene();
			await ActiveJokerPassive();
			await OnShowDeck();
			_startTime = DateTime.Now;
			_startHpPlayer = PlayCardController.Instance.Property.Health;
			AnalyticsManager.LogRoundStart();
			EnemyController.Instance.TotalDmgSendToPlayer = 0;
            _watchAdsForReviveTime = 0;
		}
		private async Task OnShowDeck()
		{
			await GetRandomHand();
			ShowStatusDeck();
		}
		private void OpenBossCutScene()
		{
			var listEnemy = EnemyController.Instance.ListEnemyScript;
			var isBoss = listEnemy.FirstOrDefault(x => x.EnemySO.typeEnemy == EnemyCardData.TypeEnemyEnum.Boss);
			if (isBoss)
			{
				BackgroundController.Instance.OnChangeBackground(BackgroundEnum.BackgroundRed);
				var randomInt = Random.Range((int)AudioName.Boss_BGM1, (int)AudioName.Boss_BGM3);
				AudioManager.Instance.OnViewLoaded((AudioName)randomInt);
				UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupBossCutScene,  new MessageShowCutScene
				{
					StateBossCutSceneEnum = StateBossCutSceneEnum.Encounter,
					IDBoss = (int)isBoss.EnemySO.id,
					Callback = (isClick) =>
					{
						if (isClick)
						{
							var getListEnemy = EnemyController.Instance.ListEnemyScript;
							var getBoss = getListEnemy.FirstOrDefault(x =>
								x.EnemySO.typeEnemy == EnemyCardData.TypeEnemyEnum.Boss)
								?.EnemySO;
							if (getBoss != null && getBoss.enemyTagsEnum.Contains(EnemyTagsEnum.Debuff))
							{
								if (getBoss.passiveNameEnemy == EnemyCardData.PassiveEffectEnemy.TheBlackKnight)
								{
									JokerCardController.Instance.SubRandomJokerCardToInventoryButNotSave();
								}
							}
						}
					}
				});
			}
			else
			{
				BackgroundController.Instance.OnChangeBackground(BackgroundEnum.BackgroundGreen);
				if (!PlayerDataManager.Instance.Property.IsCompletedTutorial)
					AudioManager.Instance.OnViewLoaded(AudioName.BGM_Tutorial);
				else
				{
					var randomInt = Random.Range((int)AudioName.Mod_Round1, (int)AudioName.Mod_Round2);
					AudioManager.Instance.OnViewLoaded((AudioName)randomInt);
				}
			}
		}
		// ReSharper disable Unity.PerformanceAnalysis
		private async Task OnPlayCardFromHand()
		{
			_lstCardSelectingDeck = PlayCardController.Instance.ListCardOnSelect;
			pokerPlayView.SpawnBasePokerHand();
			var listFinal = new List<PokerCardVo>();
			if (_lstCardSelectingDeck.Count != 0)
			{
				StateMachineController.Instance.ChangeState(GameState.EnemyTurn);
				var listSc = PlayCardController.Instance.ListCardOnHandScripts;
				foreach (var item in listSc)
				{
					foreach (var pokerCard in _lstCardSelectingDeck)
					{
						if (item.Equals(pokerCard))
							listFinal.Add(item);
						pokerCard.OnEnableHighLight(true);
					}
				}
				_lstCardSelectingDeck.ForEach(x => x.TriggerEffectOneTime());
				for (var i = 0; i < listFinal.Count; i++)
				{
					var card = listFinal[i];
					_lstPlayDeckVo.Add(card);
                    pokerPlayView.AttachChildToParentByIndex(i, card.gameObject);
					await Task.Delay(100);
					AudioManager.Instance.PlaySFXWithPitchChanger(AudioName.CardSlide2, i);
					PlayCardController.Instance.AddCardPlayedToList(card);
				}

				if (PlayCardController.IsInstanceValid())
				{
					var getValue = EvaluateHand();
					PlayCardController.Instance.CalHandPoker(getValue);
				}
				_ = CalculateCardScore();
			}
			else
			{
				UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupNotificationBox, new NotificationParameter()
				{
					TextNotification = "Please Select Card",
					IconNotificationEnum = IconNotificationEnum.Warning,
				});
			}

		}
		private PokerHandValue OnShowValueHand()
		{
			_lstCardSelectingDeck = PlayCardController.Instance.ListCardOnSelect;
			_lstCardSelectingDeck.Sort((a, b) => a.PokerCard.PokerRank.CompareTo(b.PokerCard.PokerRank));
			PokerHandValue value = PokerHandValue.High;
			PlayCardController.Instance.ClearChipAndMult();
			int levelValue = PlayCardController.Instance.HandTypePokerClass.LevelHighCard;
			if (IsOnePair())
			{
				levelValue =  PlayCardController.Instance.HandTypePokerClass.LevelPair;
				value = PokerHandValue.Pair;
			}
			if (IsTwoPair())
			{
				levelValue =  PlayCardController.Instance.HandTypePokerClass.LevelTwoPair;
				value = PokerHandValue.TwoPair;
			}
			if (IsThreeOfAKind())
			{
				value = PokerHandValue.ThreeOfAKind;
				levelValue =  PlayCardController.Instance.HandTypePokerClass.LevelThreeOfKind;
			}
			if (IsStraight())
			{
				value = PokerHandValue.Straight;
				levelValue =  PlayCardController.Instance.HandTypePokerClass.LevelStraight;
			}
			if (IsFlush())
			{
				value = PokerHandValue.Flush;
				levelValue =  PlayCardController.Instance.HandTypePokerClass.LevelFlush;
			}
			if (IsFullHouse())
			{
				value = PokerHandValue.FullHouse;
				levelValue =  PlayCardController.Instance.HandTypePokerClass.LevelFullHouse;
			}
			if (IsFourOfAKind())
			{
				value = PokerHandValue.FourOfAKind;
				levelValue =  PlayCardController.Instance.HandTypePokerClass.LevelFourOfKind;
			}
			if (IsStraightFlush())
			{
				value = PokerHandValue.StraightFlush;
				levelValue =  PlayCardController.Instance.HandTypePokerClass.LevelStraightFlush;
			}
			if (IsRoyalFlush())
			{
				value = PokerHandValue.RoyalFlush;
				levelValue =  PlayCardController.Instance.HandTypePokerClass.LevelRoyalFlush;
			}
			if (IsFiveOfKind())
			{
				value = PokerHandValue.FiveOfAKind;
				levelValue =  PlayCardController.Instance.HandTypePokerClass.LevelFiveOfAKind;
			}
			if (IsFlushHouse())
			{
				value = PokerHandValue.FlushHouse;
				levelValue =  PlayCardController.Instance.HandTypePokerClass.LevelFlushHouse;
			}
			if (IsFlushFive())
			{
				value = PokerHandValue.FlushFive;
				levelValue =  PlayCardController.Instance.HandTypePokerClass.LevelFlushFive;
			}
			_txtValueHand.text = PlayCardController.Instance.GetNameByPokerValueHand(value);
			_txtLevelValueHand.text = string.Format("Level: " + levelValue.ToString());
			_txtValueHand.gameObject.SetActive(_lstCardSelectingDeck.Count != 0);
			PlayCardController.Instance.SetValueHand(value);
			var pc = PlayCardController.Instance.CalValueHandPoker(value);
			if (PlayCardController.Instance.ListCardOnSelect.Count != 0)
			{
				PlayCardController.Instance.AddChip(pc.pokerChip);
				PlayCardController.Instance.AddMult(pc.pokerMult);
			}
			return value;
		}
		private async Task OnDiscardFromDeck(bool isSubHands)
		{
			StateMachineController.Instance.ChangeState(GameState.EnemyTurn);
			_lstCardSelectingDeck = PlayCardController.Instance.ListCardOnSelect;
			pokerPlayView.SpawnBasePokerHand();
			var listDeckVo = _playCardController.ListCardOnHandScripts.Where(x => x).ToList();
			foreach (var card in _lstCardSelectingDeck)
			{
				for (int i = 0; i < listDeckVo.Count(); i++)
				{
					var cardVo = listDeckVo[i];
					if (cardVo.PokerCard.Equals(card.PokerCard))
					{
						PlayCardController.Instance.CardStorage(cardVo);
						_playCardController.RemoveScriptCardOnHand(cardVo);
						_playCardController.RemoveCardFromHand(card.PokerCard);
						if (isSubHands)
						{
							GameObjectUtils.Instance.MoveToNewLocation(transDisCard, cardVo.rectCardParent);
							await Task.Delay(100);
							AudioManager.Instance.PlaySFXWithPitchChanger(AudioName.CardSlide1, i);
							cardVo.gameObject.SetActive(false);
						}
					}
				}
			}
			PlayCardController.Instance.ClearSelectCard();
			_lstCardSelectingDeck = new List<PokerCardVo>();
			if (isSubHands)
				PlayCardController.Instance.SubDiscard(1);
			if (PlayerDataManager.Instance.Property.CurrentIdTutorial == 7)
			{
				for (int i = 0; i < listDeckVo.Count(); i++)
				{
					var cardVo = listDeckVo[i];
					PlayCardController.Instance.CardStorage(cardVo);
					_playCardController.RemoveScriptCardOnHand(cardVo);
					_playCardController.RemoveCardFromHand(cardVo.PokerCard);
					if (isSubHands)
					{
						GameObjectUtils.Instance.MoveToNewLocation(transDisCard, cardVo.rectCardParent);
						cardVo.gameObject.SetActive(false);
					}
				}
				this.PostEvent(EventID.OnActiveTutorial, new MessageActiveIdTutorial() { IdTutorial = (int)TutorialState.Tutorial08,IsCompleted = true});
			}
			await OnShowDeck();
		}
		private void OnUpdate(PokerCardVo data)
		{
			OperationListSelect(data);
			OnShowValueHand();
			// pokerHandView.OnShowTarget();
			Debug.Log("value hand: " + PlayCardController.Instance.CurrentPokerHandValue);
		}
		private void OperationListSelect(PokerCardVo data)
		{
			var listCardSelect = PlayCardController.Instance.ListCardOnSelect;
			int maxHand = 5;
			bool checkFullDeck = listCardSelect.Count == maxHand;
			if (!listCardSelect.Contains(data))
			{
				if (!checkFullDeck)
				{
					if(PlayCardController.Instance.OnAddToSelectCard(data))
						data.OnPlayEffectSelectCard();
				}
				else
				{
					UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupNotificationBox, new NotificationParameter()
					{
						TextNotification = $"Just Select <color=green>{maxHand}</color> Card",
						IconNotificationEnum = IconNotificationEnum.Warning,
					});
				}
			}
			else
			{
				if(PlayCardController.Instance.OnRemoveToSelectCard(data))
					data.OnPlayEffectUnSelectCard();
			}
		}
		private async Task GetRandomHand()
		{
			_defaultDeck = PlayCardController.Instance.ListCardInDeck;
			var count = _playCardController.ListCardOnHand.Count;
			var maxCard = PlayerDataManager.Instance.Property.MaxDrawCardInHand;
			int cardsSelected = 0;
			while (cardsSelected < (maxCard - count) && _defaultDeck.Count > 0)
			{
				if (PlayerDataManager.Instance.Property.IsCompletedTutorial)
				{
					int randomIndex = Random.Range(0, _defaultDeck.Count);
					var card = _defaultDeck[randomIndex];
					_playCardController.AddCardToHand(card);
					_playCardController.RemoveCardInDeck(randomIndex);
				}
				else
				{
					if (PlayerDataManager.Instance.Property.CurrentIdTutorial == 0)
					{
						foreach (var pokerCard in TutorialsView.Instance.ListPokerCardTutorial) 
						{
							_playCardController.AddCardToHand(pokerCard);
							_playCardController.RemoveCardInDeck(pokerCard);
						}
					}
					else if(PlayerDataManager.Instance.Property.CurrentIdTutorial == 8)
					{
						foreach (var pokerCard in  TutorialsView.Instance.ListPokerCardAfterDiscard)
						{
							_playCardController.AddCardToHand(pokerCard);
							_playCardController.RemoveCardInDeck(pokerCard);
						}
					}
					else
					{
						int randomIndex = Random.Range(0, _defaultDeck.Count);
						var card = _defaultDeck[randomIndex];
						_playCardController.AddCardToHand(card);
						_playCardController.RemoveCardInDeck(randomIndex);
					}
						
				}
				cardsSelected++;
			}
			if (PlayerPrefs.GetInt("SortType") == 0)
				PlayCardController.Instance.SortListScriptCardVoByRank();
			else
				PlayCardController.Instance.SortListScriptCardVoBySuit();
			
			var listScript = PlayCardController.Instance.ListCardOnHand;
			var listCardOnHandScriptVo = _playCardController.ListCardOnHandScripts;
			for (int i = 0; i < listScript.Count; i++)
			{
				var card = listScript[i];
				if (listCardOnHandScriptVo.All(x => x.PokerCard != card))
				{
					GameObject newCard = GameObjectUtils.Instance.SpawnGameObject(_contentSpawn, cardPrefab);
					PokerCardVo cardScript = newCard.GetComponent<PokerCardVo>();
					newCard.name = $"PokerCard {i}";
					cardScript.SetData(card, card.PokerRank, card.PokerSuit, index: i);
					cardScript.OnInit(OnUpdate);
					cardScript.OnInitCallBackSort((x) =>
					{
						OnSort(PlayerPrefs.GetInt("SortType"));
					});
					_playCardController.AddScriptCardOnHand(cardScript);
					cardScript.gameObject.SetActive(true);
				}
			}
			if (PlayerPrefs.GetInt("SortType") == 0)
				PlayCardController.Instance.SortListScriptCardVoByRank();
			else
				PlayCardController.Instance.SortListScriptCardVoBySuit();
			await pokerHandView.SwapPositionCard();
			var getAllChild = _contentSpawn.transform.GetAllChilds();
			foreach (var t in getAllChild)
			{
				var child = t.GetComponent<PokerCardVo>();
				var checkIndex = _playCardController.ListCardOnHandScripts.IndexOf(child);
				pokerHandView.AttachChildToParentByIndex(checkIndex, child.gameObject);
				AudioManager.Instance.PlaySFXWithPitchChanger(AudioName.CardSlide2, checkIndex);
				await Task.Delay(100);
			}

			ReSpawnListCardVisual();
		}
		private void ReSpawnListCardVisual()
		{
			pokerHandView.ClearListCardVisual();
			var getAllChild = _content.transform.GetAllChilds();
			for (int i = 0; i < getAllChild.Count(); i++)
			{
				var obj = getAllChild[i];
				pokerHandView.SpawnCardVisualDrag(obj,i);
			}
			
		}
		#region OnClick 
		// ReSharper disable Unity.PerformanceAnalysis
		public void OnDiscard()
		{
			var checkMaidBoss = EnemyController.Instance.ListEnemyScript.FirstOrDefault(x => x.EnemySO.passiveNameEnemy == EnemyCardData.PassiveEffectEnemy.TheStrictMaid);
			if (checkMaidBoss != null)
			{
				if (!checkMaidBoss.IsDefeat)
				{
					UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupNotificationBox, new NotificationParameter()
					{
						TextNotification = "Don't you dare litter on my watch (Cannot Discard)",
						IconNotificationEnum = IconNotificationEnum.Warning,
					});
					return;
				}
			}
			var discardCount = PlayCardController.Instance.Property.Discards;
			_lstCardSelectingDeck = PlayCardController.Instance.ListCardOnSelect;
			if (_lstCardSelectingDeck.Count != 0)
			{
				if (discardCount > 0)
					_ = OnDiscardFromDeck(true);
				else
				{
					UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupNotificationBox, new NotificationParameter()
					{
						TextNotification = "Out of Discard!",
						IconNotificationEnum = IconNotificationEnum.Warning,
					});
				}
			}
			

		}
		// ReSharper disable Unity.PerformanceAnalysis
		public void OnPlayCard()
		{
			if(StateMachineController.Instance.CurrentState != GameState.PlayerTurn) return;
			var countEnemy = EnemyController.Instance.ListEnemyScript.Count();
			if (countEnemy == 1)
			{
				_ = OnPlayCardFromHand();
			}
			else
			{
				if (EnemyController.Instance.IdEnemySelected == -1)
				{
					Debug.Log("Please select Enemy first");
					UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupNotificationBox, new NotificationParameter()
					{
						TextNotification = "Please select Enemy first",
						IconNotificationEnum = IconNotificationEnum.Warning,
					});
				}
				else
				{
					_ = OnPlayCardFromHand();

				}
			}

		}
		public void OnClickSort()
		{
			OnSort(SortType != 1 ? 1 : 0);
		} 
		private void OnSort(int sortID)
		{
			SortType = sortID;
			UInformation(sortID);
			if (PlayCardController.IsInstanceValid())
			{
				if (sortID == 0)
					PlayCardController.Instance.SortListScriptCardVoByRank();
				else
					PlayCardController.Instance.SortListScriptCardVoBySuit();
			}
			
			pokerHandView.SortPokerOnHand();

		}
		#endregion
		private void ShowStatusDeck()
		{
			var totalCard = _defaultDeck.Count();
			int maxDeck = _playCardController.ListCardFullDeck.Count;
			_txtAmountInDeck.text = $"{totalCard.ToString()}/{maxDeck}";
			// On Hand
			int maxHand = PlayerDataManager.Instance.Property.MaxDrawCardInHand;
			int currentHand = _playCardController.ListCardOnHand.Count;
			txtAmountCardOnHand.text = $"{currentHand}/{maxHand}";
			signPlayerTurn.SetActive(true);
			signEnemyTurn.SetActive(false);
			StateMachineController.Instance.ChangeState(GameState.PlayerTurn);
		}
		#region Set Hand'sValue
		private PokerHandValue EvaluateHand()
		{
			_lstCardSelectingDeck.Sort((a, b) => a.PokerCard.PokerRank.CompareTo(b.PokerCard.PokerRank));
			//_lstSetValue.Clear();
			if (IsFlushFive()) return PokerHandValue.FlushFive;
			if (IsFlushHouse()) return PokerHandValue.FlushHouse;
			if (IsFiveOfKind()) return PokerHandValue.FiveOfAKind;
			if (IsRoyalFlush()) return PokerHandValue.RoyalFlush;
			if (IsStraightFlush()) return PokerHandValue.StraightFlush;
			if (IsFourOfAKind()) return PokerHandValue.FourOfAKind;
			if (IsFullHouse()) return PokerHandValue.FullHouse;
			if (IsFlush()) return PokerHandValue.Flush;
			if (IsStraight()) return PokerHandValue.Straight;
			if (IsThreeOfAKind()) return PokerHandValue.ThreeOfAKind;
			if (IsTwoPair()) return PokerHandValue.TwoPair;
			if (IsOnePair()) return PokerHandValue.Pair;
			HighCard();
			return PokerHandValue.High;
		}
		private bool IsFlushFive()
		{
			return IsFiveOfKind() && IsFlush();
		}
		private bool IsFlushHouse()
		{
			if (_lstCardSelectingDeck.Count != 5)
			{
				_lstSetValue = new List<PokerCard>();
				return false;
			}
			return IsThreeOfAKind() && IsOnePair() && IsFlush();
		}
		private bool IsFiveOfKind()
		{
			var groups = _lstCardSelectingDeck.GroupBy(card => card.PokerCard.PokerRank);
			foreach (var group in groups)
			{
				if (group.Count() == 5)
				{
					group.ForEach(x =>
					{
						if (!_lstSetValue.Contains(x.PokerCard))
							_lstSetValue.Add(x.PokerCard);
					});
					return true;
				}
			}
			_lstSetValue = new List<PokerCard>();
			return false;
		}
		private bool IsRoyalFlush()
		{
			return IsHighStraight() && IsFlush();
		}
		private bool IsStraightFlush()
		{
			return IsStraight() && IsFlush();
		}

		private bool IsFourOfAKind()
		{
			var groups = _lstCardSelectingDeck.GroupBy(card => card.PokerCard.PokerRank);
			foreach (var group in groups)
			{
				if (group.Count() == 4)
				{
					group.ForEach(x =>
					{
						if (!_lstSetValue.Contains(x.PokerCard))
							_lstSetValue.Add(x.PokerCard);
					});
					return true;
				}
			}
			_lstSetValue = new List<PokerCard>();
			return false;
		}

		private bool IsFullHouse()
		{
			if (_lstCardSelectingDeck.Count != 5)
			{
				_lstSetValue = new List<PokerCard>();
				return false;
			}
			return IsThreeOfAKind() && IsOnePair();
		}

		private bool IsFlush()
		{
			var checkFourFinger = PlayCardController.Instance.CheckPassiveSpecial(DefineNameJokerCard.j_four_fingers);
			var gerJoker = JokerCardController.Instance.GetJokerFromPool(DefineNameJokerCard.j_four_fingers);
			var value = gerJoker.typeEffect.FirstOrDefault();
			if (value != null)
			{
				if (checkFourFinger)
				{
					if (_lstCardSelectingDeck.Count < 4)
					{
						_lstSetValue = new List<PokerCard>();
						return false;
					}
					var getList = FindFlushSuit(_lstCardSelectingDeck);
					_lstSetValue = getList.Select(x => x.PokerCard).ToList();
					return getList.Count() != 0;
				}
				else
				{
					if (_lstCardSelectingDeck.Count != 5)
					{
						_lstSetValue = new List<PokerCard>();
						return false;
					}
					var groups = _lstCardSelectingDeck.GroupBy(card => card.PokerCard.PokerSuit);
					_lstSetValue = _lstCardSelectingDeck.Select(x => x.PokerCard).ToList();
					return groups.Count() == 1 ;
				}
				
			}
			return false;
		}
		List<PokerCardVo> FindFlushSuit(List<PokerCardVo> hand)
		{
			Dictionary<SuitCard, int> suitCounts = new Dictionary<SuitCard, int>();
			List<PokerCardVo> finalList = new List<PokerCardVo>();
			foreach (PokerCardVo card in hand)
			{
				suitCounts.TryAdd(card.PokerCard.PokerSuit, 0);
				suitCounts[card.PokerCard.PokerSuit]++;
			}

			foreach (var kvp in suitCounts)
			{
				if (kvp.Value >= 4)
				{
					foreach (var card in hand)
					{
						if(card.PokerCard.PokerSuit == kvp.Key)
							finalList.Add(card);
					}

					return finalList;
				}
			}

			return new List<PokerCardVo>();
		}

		private bool IsHighStraight()
		{
			_lstCardSelectingDeck.Sort((a, b) => a.PokerCard.PokerRank.CompareTo(b.PokerCard.PokerRank));
			if (_lstCardSelectingDeck.Any(c => c.PokerCard.PokerRank == RankCard.Ace) &&
			    _lstCardSelectingDeck.Any(c => c.PokerCard.PokerRank == RankCard.King) &&
			    _lstCardSelectingDeck.Any(c => c.PokerCard.PokerRank == RankCard.Queen) &&
			    _lstCardSelectingDeck.Any(c => c.PokerCard.PokerRank == RankCard.Jack) &&
			    _lstCardSelectingDeck.Any(c => c.PokerCard.PokerRank == RankCard.Ten))
			{
				_lstSetValue = _lstCardSelectingDeck.Select(x => x.PokerCard).ToList();
				return true;
			}

			return false;
		}
		private bool IsStraight()
		{
			var checkFourFinger = PlayCardController.Instance.CheckPassiveSpecial(DefineNameJokerCard.j_four_fingers);
			var gerJoker = JokerCardController.Instance.GetJokerFromPool(DefineNameJokerCard.j_four_fingers);
			var value = gerJoker.typeEffect.FirstOrDefault();
			_lstCardSelectingDeck.Sort((a, b) => a.PokerCard.PokerRank.CompareTo(b.PokerCard.PokerRank));
			if (_lstCardSelectingDeck.Any(c => c.PokerCard.PokerRank == RankCard.Ace) &&
				_lstCardSelectingDeck.Any(c => c.PokerCard.PokerRank == RankCard.Two) &&
				_lstCardSelectingDeck.Any(c => c.PokerCard.PokerRank == RankCard.Three) &&
				_lstCardSelectingDeck.Any(c => c.PokerCard.PokerRank == RankCard.Four) &&
				_lstCardSelectingDeck.Any(c => c.PokerCard.PokerRank == RankCard.Five))
			{
				_lstSetValue = _lstCardSelectingDeck.Select(x => x.PokerCard).ToList();
				return true;
			}
			if (checkFourFinger)
			{
				if (value != null && _lstCardSelectingDeck.Count < value.Value)
				{
					_lstSetValue = new List<PokerCard>();
					return false;
				}
				var start = 0;
				var count = 1;
				for (int i = 1; i < _lstCardSelectingDeck.Count; i++)
				{
					if (_lstCardSelectingDeck[i].PokerCard.PokerRank - _lstCardSelectingDeck[i - 1].PokerCard.PokerRank == 1)
						count++;
					else
					{
						start = i;
						count = 1;
					}

					if (count >= 4)
					{
						var listFourFinger = _lstCardSelectingDeck.GetRange(start, count).ToList();
						_lstSetValue = listFourFinger.Select(x => x.PokerCard).ToList();
						return true;
					}
				}
				return false;
			}
			else
			{
				if (_lstCardSelectingDeck.Count != 5)
				{
					_lstSetValue = new List<PokerCard>();
					return false;
				}
				for (int i = 0; i < _lstCardSelectingDeck.Count - 1; i++)
				{
					if (_lstCardSelectingDeck[i + 1].PokerCard.PokerRank - _lstCardSelectingDeck[i].PokerCard.PokerRank != 1)
					{
						_lstSetValue = new List<PokerCard>();
						return false;
					}
				}
				_lstSetValue = _lstCardSelectingDeck.Select(x => x.PokerCard).ToList();
				return true;
			}
		}

		private bool IsThreeOfAKind()
		{
			var groups = _lstCardSelectingDeck.GroupBy(card => card.PokerCard.PokerRank);
			foreach (var group in groups)
			{
				if (group.Count() == 3)
				{
					group.ForEach(x =>
					{
						if (!_lstSetValue.Contains(x.PokerCard))
							_lstSetValue.Add(x.PokerCard);
					});
					return true;
				}
			}
			_lstSetValue = new List<PokerCard>();
			return false;
		}

		private bool IsTwoPair()
		{
			var groups = _lstCardSelectingDeck.GroupBy(card => card.PokerCard.PokerRank);
			int pairsCount = 0;
			foreach (var group in groups)
			{
				if (group.Count() == 2)
				{
					group.ForEach(x =>
					{
						if (!_lstSetValue.Contains(x.PokerCard))
							_lstSetValue.Add(x.PokerCard);

					});
					pairsCount++;
				}
			}
			if (!(pairsCount == 2)) _lstSetValue = new List<PokerCard>(); ;
			return pairsCount == 2;
		}

		private bool IsOnePair()
		{
			var groups = _lstCardSelectingDeck.GroupBy(card => card.PokerCard.PokerRank);
			foreach (var group in groups)
			{
				if (group.Count() == 2)
				{
					group.ForEach(x =>
					{
						if (!_lstSetValue.Contains(x.PokerCard))
							_lstSetValue.Add(x.PokerCard);
					});
					return true;
				}
			}
			_lstSetValue = new List<PokerCard>();
			return false;
		}
		private void HighCard()
		{
			var lst = _lstCardSelectingDeck;
			var lst2 = lst.OrderByDescending(card => card.PokerCard.PokerRank).ToList();
			if (lst2.Count != 0)
				_lstSetValue.Add(lst2[0].PokerCard);
		}
		#endregion
		#region Game Place Caculator Point
		// ReSharper disable Unity.PerformanceAnalysis
		private async Task CalculateCardScore()
		{
			PlayCardController.Instance.ResetDamePerTurn();
			await CalculateBaseCard();
			await CalculateCardOnHand();
			await CalculateIndependent();
			await FinalScores();
			await EnemyTurnPlay();
			CheckStatusPlayer();
			
		}
		private async Task CalculateBaseCard()
		{
			// effect here
			_lstCardSelectingDeck.ForEach(x => x.OnEnableHighLight(true));
			var checkSplash = JokerCardController.Instance.GetJokerFromInventory(DefineNameJokerCard.j_splash) != null; 
			for (var i = 0; i < _lstPlayDeckVo.Count; i++)
			{
				var pokerCard = _lstPlayDeckVo[i];
				if (!_lstSetValue.Contains(pokerCard.PokerCard) && !checkSplash) continue;
				await Task.Delay(100);
				pokerCard.OnShowCardBeforePlay();
				AudioManager.Instance.PlaySFXWithPitchChanger(AudioName.Card1, i);
				await Task.Delay(300);
			}

			for (var i = 0; i < _lstPlayDeckVo.Count; i++)
			{
				var pokerCard = _lstPlayDeckVo[i];
				if (_lstSetValue.Contains(pokerCard.PokerCard) || checkSplash)
				{
					var config = ConfigManager.configValuePoints.GetValueByID((int)pokerCard.Rank);
					bool checkDisable = pokerCard.PokerCard.isDisableCard;
					PlayCardController.Instance.AddChip(checkDisable ? 0 : pokerCard.PokerCard.ChipValue);
					PlayCardController.Instance.AddMult(checkDisable ? 0 :pokerCard.PokerCard.MultValue);
					Debug.Log("<color=green>Poker</color> Add <color=blue>Chip</color>: " + config.chip);
					Debug.Log("<color=green>Poker</color> Add <color=red>Mult</color>: " + config.mult);
					pokerCard.OnShakeEffect(checkDisable ? 0 : config.chip, TypeEffectEnum.Chip);
					AudioManager.Instance.PlaySFXWithPitchChanger(AudioName.Chip1, i);
					await Task.Delay(300);
					if(!checkDisable)
						await CalculateOnScore(pokerCard);
					ActiveOnTrigger(pokerCard);
				};
				
			}
		}
		private Task ActiveJokerPassive()
		{
			var listJokerCard = JokerCardController.Instance.ListJokerCardVO;
			var jokerCardVos = listJokerCard.Where(x => x.CanActive).ToList();
			foreach (JokerCardVO j in jokerCardVos)
			{
				if (j.JokerData.activationEnum == JokerActivationEnum.Passive)

				{
					var joker = JokerFactory.GetProduct(j.JokerData.JokerContext);
					joker.AsyncEffectCard(j);
				}
			}

			return Task.CompletedTask;
		}
		private async Task CalculateOnScore(PokerCardVo pokerCard)
		{
			var listJokerCard = JokerCardController.Instance.ListJokerCardVO;
			var jokerCardVos = listJokerCard.Where(x => x.CanActive).ToList();
			foreach (JokerCardVO j in jokerCardVos)
			{
				if (j.JokerData.activationEnum == JokerActivationEnum.OnScore)
				{
					var joker = JokerFactory.GetProduct(j.JokerData.JokerContext);
					await joker.AsyncEffectCard(j, pokerCard, b =>
					{
						if (b) pokerCard.AddJokerToPoolActivated(j);
					});
					await Task.Delay(300);
				}
			}

		}
		private async Task CalculateIndependent()
		{
			var listJokerCard = JokerCardController.Instance.ListJokerCardVO;
			var jokerCardVos = listJokerCard.Where(x => x.CanActive).ToList();
			foreach (JokerCardVO j in jokerCardVos)
			{
				if (j.JokerData.activationEnum == JokerActivationEnum.Independent)
				{
					var joker = JokerFactory.GetProduct(j.JokerData.JokerContext);
					await joker.AsyncEffectCard(j);
					await Task.Delay(DataDurationManager.Instance.ConvertFloatTimeToIntDurationChip());
				}
			}
		}
		private Task CalculateAfterScore()
		{
			var listJokerCard = JokerCardController.Instance.ListJokerCardVO;
			foreach (JokerCardVO j in listJokerCard)
			{
				if (j.JokerData.activationEnum == JokerActivationEnum.AfterScore)
				{
					var joker = JokerFactory.GetProduct(j.JokerData.JokerContext);
					joker.AsyncEffectCard(j);
				}
			}
			return Task.CompletedTask;
		}
		private Task CalTotalDMG()
		{
			float chip = PlayCardController.Instance.Property.Chip;
			float multi = PlayCardController.Instance.Property.Mult;
			float totalDmg = chip * (multi != 0 ? multi : 1);
			PlayCardController.Instance.SaveDamePerTurn(totalDmg);
			PlayCardController.Instance.DamageAttack(totalDmg);
			PlayCardController.Instance.SetBestScore(totalDmg);
			return Task.CompletedTask;
		}
		private Task CalculateAfterDealDame()
		{
			var listJokerCard = JokerCardController.Instance.ListJokerCardVO;
			var jokerCardVos = listJokerCard.Where(x => x.CanActive).ToList();
			foreach (JokerCardVO j in jokerCardVos)
			{
				if (j.JokerData.activationEnum == JokerActivationEnum.AfterDealDame)
				{
					var joker = JokerFactory.GetProduct(j.JokerData.JokerContext);
					joker.AsyncEffectCard(j);
				}
			}
			UpdateUiEnemy();
			return Task.CompletedTask;
		}
		private void ActiveOnTrigger(PokerCardVo pokerCard)
		{
			var listJokerCard = JokerCardController.Instance.ListJokerCardVO;
			foreach (JokerCardVO j in listJokerCard)
			{
				if (j.JokerData.activationEnum == JokerActivationEnum.OnTrigger)
				{
					var joker = JokerFactory.GetProduct(j.JokerData.JokerContext);
					joker.AsyncEffectCard(j, pokerCard);
				}
			}
		}
		private void ActiveJokerCardEndGame()
		{
			var listJokerCard = JokerCardController.Instance.ListJokerCardVO;
			foreach (JokerCardVO j in listJokerCard)
			{
				if (j.JokerData.activationEnum == JokerActivationEnum.EndGame)
				{
					var joker = JokerFactory.GetProduct(j.JokerData.JokerContext);
					joker.AsyncEffectCard(j);

				}
			}
		}
		private async Task CalculateCardOnHand()
		{
			var listJokerCard = JokerCardController.Instance.ListJokerCardVO;
			var jokerCardVos = listJokerCard.Where(x => x.CanActive).ToList();
			foreach (JokerCardVO j in jokerCardVos)
			{
				if (j.JokerData.activationEnum == JokerActivationEnum.TriggerOnHand)
				{
					var joker = JokerFactory.GetProduct(j.JokerData.JokerContext);
					var getListCardOnHand = _playCardController.ListCardOnHandScripts;
					foreach (var poker in getListCardOnHand)
					{
						if (!_lstCardSelectingDeck.Contains(poker))
							await joker.AsyncEffectCard(j, poker);
					}

				}
			}
		}
		private async Task FinalScores()
		{
			await CalculateAfterScore();
			await CalTotalDMG();
			await EffectAttackToEnemy();
			await CalculateAfterDealDame();
			PlayCardController.Instance.ResetPoint();
			_txtValueHand.gameObject.SetActive(false);
			PlayCardController.Instance.AddHands(1);
			EnemyController.Instance.IdEnemySelected = -1;
		}
		private async Task EnemyTurnPlay()
		{
			signPlayerTurn.SetActive(false);
			signEnemyTurn.SetActive(true);
			if (EnemyController.Instance.IsAllEnemyDeath())
			{
				var listDeckVo = _playCardController.ListCardOnHandScripts.Where(x => x).ToList();
				for (int i = 0; i < listDeckVo.Count(); i++)
				{
					var cardVo = listDeckVo[i];
					GameObjectUtils.Instance.MoveToNewLocation(transDisCard, cardVo.rectCardParent);
					await Task.Delay(100);
					cardVo.gameObject.SetActive(false);
				}
				await Task.Delay(500);
				SaveDataLastGame();
				PlayCardController.Instance.SaveIDBoss(0);
			}
			else
			{
				var listCardInDeck = PlayCardController.Instance.ListCardInDeck;
				bool checkCard = listCardInDeck.Count == 0;
				if (PlayerDataManager.Instance.Property.IsCompletedTutorial && !checkCard)
				{
					await _enemyPlaceView.EnemyPlay();
				}
				else if (PlayerDataManager.Instance.Property.IsCompletedTutorial)
				{
					PlayerLose();
				}
			}
			_ = OnDiscardFromDeck(false);
		}
		private void SaveDataLastGame()
		{
			ActiveJokerCardEndGame();
			PlayerDataManager.Instance.Property.UpdateStatsPlayer();
			PlayCardController.Instance.ClearCardStorage();
			_enemyPlaceView.ResetState();
			PlayerLastResult infoLastGame = new PlayerLastResult()
			{
				ResultLastGame = ResultGameEnum.WinPerRound,
				PokerHandValue = PlayCardController.Instance.GetMostValueHand(),
				PokerHandValueCount = PlayCardController.Instance.GetMostValueHandCount(), 
				BestScoreLastGame = PlayCardController.Instance.BestScore,
				CountHandLastGame = PlayCardController.Instance.Hands,
			};
			DateTime endTime = DateTime.Now; // Lấy thời gian kết thúc
			TimeSpan duration = endTime - _startTime;
			//analytics
			AnalyticsManager.LogGamePerRound(
				PlayerDataManager.Instance.Property.Round,
				duration.TotalSeconds.ToString(CultureInfo.InvariantCulture),
				1,
				PlayCardController.Instance.BestScore,
				PlayCardController.Instance.BestScore /  PlayCardController.Instance.Hands,
				EnemyController.Instance.TotalDmgSendToPlayer,
				_startHpPlayer - PlayCardController.Instance.Property.Health,
				PlayCardController.Instance.Hands,
				4 - PlayCardController.Instance.Discards,
				PlayerDataManager.Instance.Property.TotalOwnerJoker.Count(),
				"0",
				PlayCardController.Instance.Property.Health,
				PlayCardController.Instance.Property.Shield
				);
			//
			
			PlayerDataManager.Instance.Property.SetPlayerLastGameInfo(infoLastGame);
			if(!PlayerDataManager.Instance.Property.IsCompletedTutorial && PlayerDataManager.Instance.Property.CurrentIdTutorial == 14)
			{
				
			}
			else
			{
				var listEnemy = EnemyController.Instance.ListEnemyScript;
				var checkBoss = listEnemy.FirstOrDefault(x => x.EnemySO.typeEnemy == EnemyCardData.TypeEnemyEnum.Boss);
				if (checkBoss)
				{
					
					UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupBossCutScene,  new MessageShowCutScene
					{
						StateBossCutSceneEnum = StateBossCutSceneEnum.Victory,
						IDBoss = (int)checkBoss.EnemySO.id,
						Callback = (isClick) =>
						{
							if (isClick)
							{
								ShowPopupMapProgression();
							}
						}
					});
				}
				else
				{
					ShowPopupMapProgression();
				}
			}
		
		}
		private void ShowPopupMapProgression()
		{
			if (PlayerDataManager.Instance.Property.Round != 61)
			{
				UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupMapProgression, true);
			}
			this.PostEvent(EventID.OnShowResultGameView, new MessageResult { IsShow = true, ResultGame = ResultGameEnum.WinPerRound });
			
		
		}
		private void CheckStatusPlayer()
		{
			if (PlayCardController.Instance.Property.Health <= 0)
			{
				if (_watchAdsForReviveTime > 0)
				{
					PlayerLose();
					return;
				}
                PlayerLastResult infoLastGame = new PlayerLastResult()
                {
                	ResultLastGame = ResultGameEnum.Lose,
                	PokerHandValue = PlayCardController.Instance.GetMostValueHand(),
                	PokerHandValueCount = PlayCardController.Instance.GetMostValueHandCount(),
                	BestScoreLastGame = PlayCardController.Instance.BestScore,
                	CountHandLastGame = PlayCardController.Instance.Hands,
                };
                PlayerDataManager.Instance.Property.SetStatePlayerStay(StateScene.ResultView);
                PlayerDataManager.Instance.Property.SetPlayerLastGameInfo(infoLastGame);
				UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupReviveAds, new PopupReviveAds.MessageBoxParam()
				{
					MessageBoxType = PopupReviveAds.MessageBoxType.Yes_No,
					MessageTitle = "500 HP!",
					MessageBody = ColorSchemeManager.Instance.ConvertColorTextFromSymbol("Oops, looks like you took a bit of a beating there.  Want a do-over?  Watch an ad and we'll patch you up with/n %s1H 500 HP %s1H !"),
					OnMessageBoxActionCallback = (msg) =>
					{
						if (msg == PopupReviveAds.MessageBoxAction.Accept)
						{
							RevivePlayer();
							// _watchAdsForReviveTime++;
						}
						else
						{
							PlayerLose();
						}
						return true;
					}
				});
				
			}
		}
		private void RevivePlayer()
		{
			PlayCardController.Instance.AddPlayerHealth(500);
			_watchAdsForReviveTime ++;
			UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupNotificationBox, new NotificationParameter()
			{
				TextNotification = "Congratulations! you revive, let's try again!!!",
				IconNotificationEnum = IconNotificationEnum.Warning,
			});
		}
		private void PlayerLose()
		{
			PlayerLastResult infoLastGame = new PlayerLastResult()
			{
				ResultLastGame = ResultGameEnum.Lose,
				PokerHandValue = PlayCardController.Instance.GetMostValueHand(),
				PokerHandValueCount = PlayCardController.Instance.GetMostValueHandCount(),
				BestScoreLastGame = PlayCardController.Instance.BestScore,
				CountHandLastGame = PlayCardController.Instance.Hands,
			};
			PlayerDataManager.Instance.Property.SetPlayerLastGameInfo(infoLastGame);
			var listEnemy = EnemyController.Instance.ListEnemyScript;
			var checkBoss = listEnemy.FirstOrDefault(x => x.EnemySO.typeEnemy == EnemyCardData.TypeEnemyEnum.Boss);
			if (checkBoss)
			{
				UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupBossCutScene,  new MessageShowCutScene
				{
					StateBossCutSceneEnum = StateBossCutSceneEnum.Defeat,
					IDBoss = (int)checkBoss.EnemySO.id,
					Callback = (isClick) =>
					{
						if (isClick)
						{
							this.PostEvent(EventID.OnShowResultGameView, new MessageResult { IsShow = true, ResultGame = ResultGameEnum.Lose });
						}
					}
				});
			}
			else
			{
				this.PostEvent(EventID.OnShowResultGameView, new MessageResult { IsShow = true, ResultGame = ResultGameEnum.Lose });
			}
		}
		#endregion
		#region  Enemy
		public void UpdateUiEnemy()
		{
			_enemyPlaceView.SetDamageAndSend();
		}
		private void CreateNewEnemy()
		{
			_enemyPlaceView.StartEnemyTurn();
			_enemyPlaceView.SpawnEnemy();
		}
		private void OnChangeStateGamePlay(object obj)
		{
			MessageStateGameplay msg = (MessageStateGameplay)obj;
			GameState state = msg.GameState;
			if (state == GameState.None)
			{
				signPlayerTurn.SetActive(true);
				return;
			}
			
		}
		private void OnActiveTutorial(object obj)
		{
			MessageActiveIdTutorial msg = (MessageActiveIdTutorial)obj;
			if (msg.IdTutorial == 14)
			{
				_ = _enemyPlaceView.EnemyPlay();
			}
		}
		private void UInformation(int sort)
		{
			txtSortDeckOnHand.text = sort == 1 ? "Suit" : "Rank";
		}
		#endregion
		#region Effect Here
		private async Task EffectAttackToEnemy()
		{
			int totalCardActive = 0;
			for (var i = 0; i < _lstPlayDeckVo.Count; i++)
			{
				var card = _lstPlayDeckVo[i];
				card.OnShowCardBeforePlay();
				AudioManager.Instance.PlaySFXWithPitchChanger(AudioName.CardSlide1, i);
				await Task.Delay(150);
			}
			await Task.Delay(150);
			foreach (var card in _lstPlayDeckVo)
			{
				if (!_lstSetValue.Contains(card.PokerCard))
				{
					GameObjectUtils.Instance.MoveToNewLocation(transDisCard, card.rectCardParent);
					await Task.Delay(150);
				}
			}
			await Task.Delay(200);
			var enemyDeath = false;
			_lstPlayDeckVo.ForEach(x =>
			{
				if (_lstSetValue.Contains(x.PokerCard))
				{
					totalCardActive++;
				}
				
			});
				
			for (var i = 0; i < _lstPlayDeckVo.Count; i++)
			{
				var card = _lstPlayDeckVo[i];
				var enemySelected = EnemyController.Instance.GetComponentByIdEnemy();
				if(EnemyController.Instance.IsAllEnemyDeath())
				{
					GameObjectUtils.Instance.MoveToNewLocation(transDisCard, card.rectCardParent);
					break;
				}
				if (enemyDeath)
				{
					
					var ownedJoker = PlayerDataManager.Instance.Property.TotalOwnerJoker;
					if(ownedJoker.Contains(202))
					{
						var enemyIndex = EnemyController.Instance.ListEnemyScript.Where(x => !x.IsDefeat).LastOrDefault();
						EnemyController.Instance.IdEnemySelected = enemyIndex.GetIndexEnemy;
						enemySelected = enemyIndex;
					}
					else
					{
						GameObjectUtils.Instance.MoveToNewLocation(transDisCard, card.rectCardParent);
						continue;
					}
				}
				if (_lstSetValue.Contains(card.PokerCard))
				{
					card.transform.DOMove(enemySelected.transform.position, 0.8f).SetEase(Ease.InOutElastic)
						.OnUpdate(() =>
						{
							if (card.PokerCard.isDisableCard)
							{
								GameObjectUtils.Instance.MoveToNewLocation(transDisCard, card.rectCardParent);
								return;
							}
							if (Vector3.Distance(card.transform.position, enemySelected.transform.position) <= 0.4f)
							{
								card.OnHide();
								card.transform.DOKill();
								// AudioManager.Instance.PlaySFXWithPitchChanger(AudioName.Attack1,i1);
								enemySelected.EnableObjectFeel(2);
								var selectId = EnemyController.Instance.IdEnemySelected;
								float dmg = (PlayCardController.Instance.TotalDmgPerTurn / totalCardActive);
								EnemyController.Instance.ModifyStatsEnemy(selectId,-dmg, (isDeath) =>
								{
									enemyDeath = isDeath;
									
									//for boss 202
									PlayCardController.Instance.SubDiscard(0);
								});
								UpdateUiEnemy();
							}
						})
						.OnComplete(() =>
						{
								GameObjectUtils.Instance.MoveToNewLocation(transDisCard, card.rectCardParent);
						});
					await Task.Delay(500);
					Vector3 direction = enemySelected.transform.position - card.transform.position;
					float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
					card.transform.DORotate(new Vector3(0, 0, angle), 0.8f).SetEase(Ease.InOutElastic);
				}
			}

			await Task.Delay(300);
			_lstPlayDeckVo = new List<PokerCardVo>();
			if(!PlayerDataManager.Instance.Property.IsCompletedTutorial)
				this.PostEvent(EventID.OnActiveTutorial, new MessageActiveIdTutorial() { IdTutorial = (int)TutorialState.Tutorial11,IsCompleted = true});
		}
		
		#endregion
	}
}
