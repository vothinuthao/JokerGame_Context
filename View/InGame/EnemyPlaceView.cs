using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Controller.Observer;
using Core.Entity;
using Core.Manager;
using Core.Utils;
using DG.Tweening;
using Effect;
using Entity;
using Frameworks.Effect.Scripts;
using Frameworks.Scripts;
using MainMenu;
//using Interface;
using Manager;
using Runtime.Manager;
using Tutorials;
using UnityEngine;
using UnityEngine.Events;

namespace InGame
{
    public class EnemyPlaceView : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent onCallActionPlay;
        [SerializeField] private VisualLeanTouch enemyTouch;
        [SerializeField]
        private Transform _content;
        [SerializeField]
        private GameObject _prefabCard;
        [SerializeField]
        private Transform _playContent;
        [SerializeField]
        private GameObject _prefabPoker;
        [SerializeField]
        private List<int> _listCurrentEnemy;
        [SerializeField]
        private Transform discardZone;
        [SerializeField] private RectTransform playerTransform;
        [SerializeField] private EnemyCardVo enemyAttacking;
        private List<PokerCard> _listCardEnemyDraw = new List<PokerCard>();
        public List<PokerCardVo> _lstPlayDeckVO = new List<PokerCardVo>();
        private Dictionary<int, GameObject> _dictionaryEnemyZone = new Dictionary<int, GameObject>();
        private int _selectedEnemy;
        private EnemyController _enemyController;
        private float _totalChip = 0f;
        private float _totalMult = 0f;
        public void StartEnemyTurn()
        {
            
            _enemyController = EnemyController.Instance;
            _enemyController.InitEnemyPerRound();
            _listCardEnemyDraw = _enemyController.LstDefaultCardEnemies;
            _listCurrentEnemy = _enemyController.ListEnemyId;
            _prefabCard.SetActive(false);
            _prefabPoker.SetActive(false);
        }
        public async Task EnemyPlay()
        {
            var listEnemy = _enemyController.ListEnemyScript;
            EnemyController.Instance.EnemyEffectStartGame();
            await Task.Delay(1500);
            
            foreach (EnemyCardVo enemyCardVo in listEnemy)
            {
                if (!enemyCardVo.IsDefeat)
                {
                    await enemyCardVo.EffectChooseForAttack();
                    enemyCardVo.DrawCardFromHand();
                    await OnPlayCardFromDeck(enemyCardVo);
                    enemyCardVo.EffectClose();
                }
            }
            ResetState();
        }
        public void SpawnEnemy()
        {
            var lst = EnemyController.Instance.ListEnemies;
            GameObjectUtils.Instance.ClearAllChild(_content.gameObject);
            _dictionaryEnemyZone.Clear();
            for (int i = 0; i < _listCurrentEnemy.Count; i++)
            {
                var e = _listCurrentEnemy[i];
                var getData = lst.FirstOrDefault(x=>x.id == e);
                var ob = GameObjectUtils.Instance.SpawnGameObject(_content, _prefabCard);
                var script = ob.GetComponent<EnemyCardVo>();
                //callback 
                script.OnInit(GetCallbackSelectEnemy);
                script.SetData(getData,i);
                EnemyController.Instance.InitEnemy(i,script);
                ob.SetActive(true);
                //spawn zone touch
                _dictionaryEnemyZone.Add(i,ob);
            }
            CreateZoneTouch();

        }
        public void SetDamageAndSend()
        {
            var listEnemyScript = EnemyController.Instance.ListEnemyScript;
            foreach (var ec in listEnemyScript)
            {
                if (_listCurrentEnemy.Any((x=>x == ec.EnemySO.id)))
                {
                    ec.UpdateStatusEnemyCard();
                }
            }
        }
        private async Task OnPlayCardFromDeck(EnemyCardVo enemyScript)
        {
            _totalChip = 0f;
            _totalMult = 0f;
            _lstPlayDeckVO.Clear();
            enemyAttacking = enemyScript;
            GameObjectUtils.Instance.ClearAllChild(_playContent.gameObject);
            var listCard = enemyScript.ListCardOnHand;
            List<PokerCard> bestHand =  EnemyController.Instance.GetBestHand(listCard);
            List<PokerCard> correctHand =  listCard.Count() <= 5 ? listCard : bestHand;
            
            for (var i = 0; i < correctHand.Count; i++)
            {
                var card = correctHand[i];
                GameObject newCard = GameObjectUtils.Instance.SpawnGameObject(_playContent, _prefabPoker);
                PokerCardVo cardScript = newCard.GetComponent<PokerCardVo>();
                cardScript.SetData(card, card.PokerRank, card.PokerSuit, false, index: i);
                _lstPlayDeckVO.Add(cardScript);
                cardScript.gameObject.SetActive(true);
                EnemyController.Instance.AddEnemyCardToStorage(enemyScript.GetIndexEnemy,card);
            }
            for (var i = 0; i < _lstPlayDeckVO.Count; i++)
            {
                var card = _lstPlayDeckVO[i];
                if (bestHand.Any(x=> x.Equals(card.PokerCard)))
                {
                    var delayCalPoint = (int)(DataDurationManager.Instance.DurationEnemyPlayPoint * 1000);
                    card.OnShowCardBeforePlay();
                    AudioManager.Instance.PlaySFXWithPitchChanger(AudioName.CardSlide1, i);
                    await Task.Delay(200);
                    _totalChip +=(card.PokerCard.ChipValue == 0 ? 1 :  card.PokerCard.ChipValue);
                    _totalMult += card.PokerCard.MultValue;
                    card.OnShakeEffect(card.PokerCard.ChipValue, TypeEffectEnum.Chip);
                    await Task.Delay(delayCalPoint);
                }
            }
            await Task.Delay(100);
            foreach (var card in _lstPlayDeckVO)
            {
                if (!bestHand.Any(x => x.Equals(card.PokerCard)))
                {
                    GameObjectUtils.Instance.MoveToNewLocation(discardZone, card.rectCardParent);
                    EffectContainerController.Instance.OnShakeContainer();
                    await Task.Delay(100);
                }
            }
            await Task.Delay(300);
            var _totalDmg =  EnemyController.Instance.CalculateDamage(_totalChip,_totalMult, enemyScript);
            EnemyController.Instance.TotalDmgSendToPlayer += (int)_totalDmg;
            for (var i = 0; i < _lstPlayDeckVO.Count; i++)
            {
                var card = _lstPlayDeckVO[i];
                var i1 = i;
                if (bestHand.Any(x => x.Equals(card.PokerCard)))
                {
                    card.transform.DOMove(playerTransform.position, 0.8f).SetEase(Ease.InOutElastic)
                        .OnUpdate(() =>
                        {
                            if (Vector3.Distance(card.transform.position, playerTransform.position) <= 0.4f)
                            {
                                EffectContainerController.Instance.OnShakeContainer();
                                card.OnHide();
                                card.transform.DOKill();
                                var calDamePerPoker = (_totalDmg / _totalChip) * card.PokerCard.ChipValue;
                                PlayCardController.Instance.PlayerTakeDame((int)calDamePerPoker);
                                EffectCanvasManager.Instance.ShowEffectTotalDame(playerTransform, -calDamePerPoker, calDamePerPoker < -400);
                                EffectManager.Instance.PlayEffect(playerTransform, EffectName.AttackPlayerVFX);
                              
                            }
                        })
                        .OnComplete(() =>
                        {
                            GameObjectUtils.Instance.MoveToNewLocation(playerTransform, card.rectCardParent);
                            
                        });
                   
                    await Task.Delay(300);
                    Vector3 direction = playerTransform.position - card.transform.position;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    card.transform.DORotate(new Vector3(0, 0, angle), 0.8f).SetEase(Ease.InOutElastic);
                    await Task.Delay(300);
                    EnemyController.Instance.RemoveCardOnHandEnemy(card.PokerCard, enemyScript);
                }
            }
        }
        public void ResetState()
        {
            EnemyController.Instance.ResetState();
            GameObjectUtils.Instance.ClearAllChild(_playContent.gameObject);
            _lstPlayDeckVO.Clear();
            StateMachineController.Instance.ChangeState(GameState.PlayerTurn);
            if(!PlayerDataManager.Instance.Property.IsCompletedTutorial)
                this.PostEvent(EventID.OnActiveTutorial, new MessageActiveIdTutorial() { IdTutorial = (int)TutorialState.Tutorial13,IsCompleted = true});
            CreateZoneTouch();
        }
        private void GetCallbackSelectEnemy(int index)
        {
            EnemyController.Instance.IdEnemySelected = index;
            onCallActionPlay?.Invoke();
        }

        private void CreateZoneTouch()
        {
            var listCount = _dictionaryEnemyZone.Count(x => x.Value.activeSelf);
            enemyTouch.OnInit(listCount,GetCallbackSelectEnemy);
            foreach (var obj in _dictionaryEnemyZone)
            {
                if(obj.Value.activeSelf)
                    enemyTouch.SpawnVisualZone(obj.Value, obj.Key);
            }
          
        }
    }
}