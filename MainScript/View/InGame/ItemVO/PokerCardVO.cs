using System;
using System.Collections;
using System.Collections.Generic;
using Core.Controller.Observer;
using Core.Entity;
using DG.Tweening;
using Entity;
using Frameworks.Scripts;
using Manager;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using TMPro;
using Tutorials;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace InGame
{
    public class PokerCardVo : MonoBehaviour
    {
        public RectTransform rectCardParent;
        [SerializeField]
        private Image _imgSuit;
        [SerializeField]
        private GameObject objHighLight;
        // UI 
        [SerializeField]
        private TextMeshProUGUI txtPointChip;
        [SerializeField]
        private TextMeshProUGUI txtPointMult;
        [SerializeField]
        private TextMeshProUGUI txtPointGold;
        [SerializeField]
        private GameObject objPointChip;
        [SerializeField]
        private GameObject objPointMult;
        [SerializeField]
        private GameObject objPointGold; 
        [SerializeField]
        private GameObject objDisableCard;
        [SerializeField]
        private RectTransform rectInitPosition;
        [Header("Effect")]
        [SerializeField] private MMF_Player mmPlayerEffectSelect;
        [SerializeField] private MMF_Player mmPlayerEffectUnSelect;
        [SerializeField] private MMF_Player mmPlayerEffectAddNewCard;
        [SerializeField] private MMF_Player mmPlayerEffectConvertCard;
        [FoldoutGroup("Effect For Point")] [SerializeField]
        private MMF_Player mmPointChip;
        [FoldoutGroup("Effect For Point")] [SerializeField]
        private MMF_Player mmPointMult;
        [FoldoutGroup("Effect For Point")] [SerializeField]
        private MMF_Player mmPointGold;
        
        [FoldoutGroup("Shop ")] [SerializeField]
        private CanvasGroup canvasSelect;
        
        private int _indexCard;
        public bool IsFaceCard { get; private set; }
        private RankCard rank;
        private SuitCard suit;
        private bool _isSelection = false;
        private PokerCard _cardBase;
        private List<JokerCardVO> _listJokerVo = new List<JokerCardVO>();
        private Action<PokerCardVo> _callback;
        private Action<bool> _callbackSort;
        private bool _canClick = false;
        public PokerCard PokerCard => _cardBase;
        public int IndexCard => _indexCard;
        public List<JokerCardVO> ListJokerCardVo => _listJokerVo;
        public RankCard Rank => rank;
        public SuitCard Suit => suit;

        // private float _poolChipOnCard;
        // private float _poolMultOnCard;
        // public float PoolChipOnCard => _poolChipOnCard;
        // public float PoolMultOnCard => _poolMultOnCard;
        
        [SerializeField] private UnityEvent onActionShowCard;
        [SerializeField] private UnityEvent actionPlay;
        [SerializeField] private UnityEvent onActionDissolve;
        private Action _isDoneCallback;
        private void Start()
        {
            objPointChip.SetActive(false);
            objPointMult.SetActive(false);
            objPointGold.SetActive(false);
            objHighLight.SetActive(false);
        }
        
        public void OnInit(Action<PokerCardVo> callback)
        {
            _callback = callback;
            objPointChip.SetActive(false);
            objPointMult.SetActive(false);
            objPointGold.SetActive(false);
        }
        public void CallBackDoneSelectCard(Action isDone)
        {
            _isDoneCallback = isDone;
        }
        public void OnInitCallBackSort(Action<bool> callback)
        {
            _callbackSort = callback;
        }
        // ReSharper disable Unity.PerformanceAnalysis
        public void SetData(PokerCard data = null, RankCard r = RankCard.None, SuitCard s = SuitCard.None, bool canClick = true, int index = 0)
        {
            _cardBase = data;
            rank = r;
            suit = s;
            _canClick = canClick;
            _indexCard = index;
            IsFaceCard = JokerCardController.Instance.CheckPassiveAndConsiderToFaceCard(r,s);
            Sprite sptSuit = SpritesManager.Instance.GetSpritesByRankAndSuit(r,s);
            _imgSuit.sprite = sptSuit;
            objDisableCard.SetActive(_cardBase is { isDisableCard: true });
            if(canvasSelect != null)
                canvasSelect.DOFade(0, 0f);
        }
        public void UpdateData()
        {
            StartCoroutine(PlayEffectConvert());
        }

        // ReSharper disable Unity.PerformanceAnalysis
        IEnumerator PlayEffectConvert()
        {
            mmPlayerEffectConvertCard.PlayFeedbacks();
            yield return new WaitForSeconds(0.2f);
            IsFaceCard = JokerCardController.Instance.CheckPassiveAndConsiderToFaceCard(_cardBase.PokerRank,_cardBase.PokerSuit);
            Sprite sptSuit = SpritesManager.Instance.GetSpritesByRankAndSuit(_cardBase.PokerRank,_cardBase.PokerSuit);
            _imgSuit.sprite = sptSuit;
            yield return new WaitForSeconds(0.2f);
            mmPlayerEffectConvertCard.PlayFeedbacks();
        }
        public void OnDestroyGameObject()
        {
            _callbackSort?.Invoke(true);
            this.gameObject.SetActive(false);
           Destroy(this.gameObject);
        }
        public void OnHide()
        {
            _callbackSort?.Invoke(true);
            this.gameObject.SetActive(false);
        }
        public void OnClickSelect()
        {
            if(StateMachineController.Instance.CurrentState != GameState.PlayerTurn) return;
            AudioManager.Instance.PlaySFX( _isSelection ? AudioName.CardSlide2 : AudioName.CardSlide1);
            if (!PlayCardController.IsInstanceValid()) return;
            _callback?.Invoke(this);
            
            #region  Tutorial 
            if (PlayCardController.Instance.ListCardOnSelect.Count != 0 && PlayerDataManager.Instance.Property.CurrentIdTutorial == 6)
            {
                this.PostEvent(EventID.OnActiveTutorial, new MessageActiveIdTutorial() { IdTutorial = (int)TutorialState.Tutorial07,IsCompleted = true});
            }
            if (PlayCardController.Instance.ListCardOnSelect.Count == 0 && PlayerDataManager.Instance.Property.CurrentIdTutorial == 7)
            {
                this.PostEvent(EventID.OnActiveTutorial, new MessageActiveIdTutorial() { IdTutorial = (int)TutorialState.Tutorial06,IsCompleted = true});
            }
            if (PlayCardController.Instance.ListCardOnSelect.Count == 5 && PlayerDataManager.Instance.Property.CurrentIdTutorial == 9)
            {
                if (TutorialsView.Instance.CheckStraightPoker())
                {
                    this.PostEvent(EventID.OnActiveTutorial, new MessageActiveIdTutorial() 
                    { 
                        IdTutorial = (int)TutorialState.Tutorial10,
                        IsCompleted = true 
                    });
                }
            }
            if(PlayCardController.Instance.ListCardOnSelect.Count != 5 && PlayerDataManager.Instance.Property.CurrentIdTutorial == 10)
            {
                this.PostEvent(EventID.OnActiveTutorial, new MessageActiveIdTutorial() { IdTutorial = (int)TutorialState.Tutorial09,IsCompleted = true});
            }
            

            #endregion
        }
        public void OnShakeEffect(int addPoint, TypeEffectEnum typePoint)
        {
           
            switch (typePoint)
            {
                case TypeEffectEnum.None:
                    objPointChip.SetActive(false);
                    objPointMult.SetActive(false);
                    objPointGold.SetActive(false);
                    break;
                case TypeEffectEnum.Chip:
                    txtPointChip.text = "+" + addPoint;
                    objPointChip.SetActive(true);
                   
                    break;
                case TypeEffectEnum.AddMult:
                    txtPointMult.text = "+" + addPoint;
                    objPointMult.SetActive(true);
                    break;
                case TypeEffectEnum.MultiplyMult:
                    txtPointMult.text = "x" + addPoint;
                    objPointMult.SetActive(true);
                    break;
                case TypeEffectEnum.Dollar:
                    txtPointGold.text = "+" + addPoint;
                    objPointGold.SetActive(true);
                    break;
            }
            AudioManager.Instance.PlaySFX(AudioName.Chip2);
            actionPlay?.Invoke();
            
        }
        public void AddJokerToPoolActivated(JokerCardVO joker)
        {
            if(!_listJokerVo.Contains(joker))
                _listJokerVo.Add(joker);
        }
        public void RemoveJokerInPool()
        {
            _listJokerVo.Clear();
        }
        public void DiscardEffect()
        {
            rectCardParent.DOAnchorPosY(rectCardParent.anchoredPosition.y + 100f, 1f)
                .SetEase(Ease.OutQuad)
                .OnComplete(()=>
                {
                    onActionDissolve?.Invoke();
                    
                });
        }
        public void OnShowCardBeforePlay()
        {
            onActionShowCard?.Invoke();
           
        }
        public void TriggerEffectOneTime()
        {
            this.mmPlayerEffectSelect.PlayFeedbacks();
            objHighLight.SetActive(true);
        }
        public void OnPlayEffectSelectCard()
        {
            mmPlayerEffectSelect.PlayFeedbacks();
            objHighLight.SetActive(true);
        }
        public void OnPlayEffectUnSelectCard()
        {
            mmPlayerEffectUnSelect.PlayFeedbacks();
            objHighLight.SetActive(false);
        }
        public void ShowSelectButton()
        {
            if (canvasSelect != null)
            {
                canvasSelect.DOFade(1, 0.2f);
            }
        }
        public void HideSelectButton()
        {
            if (canvasSelect != null)
            {
                canvasSelect.DOFade(0, 0.2f);
            }
        }
        public void OnClickChoosePokerCard()
        {
            PlayCardController.Instance.AddMorePokerCardToInventory(_cardBase);
            mmPlayerEffectAddNewCard.PlayFeedbacks();
        }
        public void OnCallBackChoosePokerCard()
        {
            _isDoneCallback?.Invoke();
        }

        public void OnEnableHighLight(bool isShow)
        {
            objHighLight.SetActive(isShow);
        }
    }
}