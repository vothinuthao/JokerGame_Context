using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Core.Entity;
using Core.Model;
using DG.Tweening;
using Enemy;
using Entity;
using Frameworks.Effect.Scripts;
using Frameworks.Scripts;
using Frameworks.Utils;
using Manager;
using MoreMountains.Tools;
using Runtime.Manager;
using Sirenix.OdinInspector;
using TMPro;
using UI.Popups;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace InGame
{
    public class EnemyCardVo : MonoBehaviour
    { 
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private MMProgressBar mmProgressBar;
        #region  For UI
        [SerializeField] private GameObject _objCard;
        
        [SerializeField] private GameObject objInformationBoss;
        
        [SerializeField] private Image imgInformationBoss;
        
        [SerializeField] private Button _btnCard;

        [SerializeField] private Image _iconEnemy;

        [SerializeField] private TextMeshProUGUI _curDeck;

        [SerializeField] private TextMeshProUGUI _txtDescription;

        [SerializeField] private TextMeshProUGUI _txtEnemyName;

        [SerializeField] private TextMeshProUGUI _txtHealth;
        
        [SerializeField] private GameObject _objHighLight;
        
        [SerializeField] private GameObject _objHighLightForBoss;
        
        [SerializeField] private TextMeshProUGUI _txtDrawCard;
        
        [SerializeField] private GameObject objInFormation;
        [SerializeField] private PopupCardDescription scriptInFormation;
        
        #endregion
        #region Action Zone

        [FoldoutGroup("Action Click")] [SerializeField]
        private GameObject objDefeat;
        [FoldoutGroup("Action Click")] [SerializeField]
        private GameObject objGetHit;
        [FoldoutGroup("Action Click")] [SerializeField]
        private UnityEvent onActionDefeated;
        [FoldoutGroup("Action Click")] [SerializeField]
        private UnityEvent onActionGetHit;
        
        #endregion
        private List<PokerCard> _listCardInDeck = new List<PokerCard>();
        private List<PokerCard> _lstCardOnHand = new List<PokerCard>();
        private EnemyModel _enemyProps = new EnemyModel();
        public EnemyModel Property
        {
            get
            {
                if (_enemyProps == null)
                    _enemyProps = new EnemyModel();
                return _enemyProps;
            }
        }
        private EnemySo _data;
        private int _index; 
        private Action<int> _callback;
        private bool _isDefeat;
        private Camera _camera;
        public EnemySo EnemySO => _data;
        public List<PokerCard> ListCardInDeck => _listCardInDeck;
        public List<PokerCard> ListCardOnHand => _lstCardOnHand;
        public bool IsDefeat => _isDefeat;
        public int GetIndexEnemy => _index;

        private void Start()
        {
            _camera = Camera.main;
            if(objInFormation != null)
                objInFormation.SetActive(false);
        }
        public void OnInit(Action<int> callback)
        {
            _objHighLight.SetActive(false);
            _btnCard.interactable = true;
            _callback = callback;
        }
        public void OnHideAfterDefeat()
        {
            this.gameObject.SetActive(false);
        }
        public void SetData(EnemySo data, int index)
        {
            _data = data;
            _index = index;
            // init data 
            if (PlayCardController.IsInstanceValid())
            {
                var dict = EnemyController.Instance.DictAllEnemyModel;
                if (dict.ContainsKey(_data.id))
                {
                    var baseModel = dict.FirstOrDefault(x => x.Key == _data.id).Value;
                    _enemyProps = new EnemyModel()
                    {
                        ID = baseModel.ID,
                        Name = baseModel.Name,
                        Health = EnemyController.Instance.ScaleMaxHealthEnemy((int)baseModel.Health, (int)baseModel.MultiplierHealth),
                        MaxHealth = EnemyController.Instance.ScaleMaxHealthEnemy((int)baseModel.Health, (int)baseModel.MultiplierHealth),
                        EnemyDifficultyEnum = baseModel.EnemyDifficultyEnum,
                        ListValueHandLevelConfig = EnemyController.Instance.ScaleHandLevelEnemy(baseModel.ListValueHandLevelConfig),
                        EnemyRankEnum = baseModel.EnemyRankEnum,
                        IsUsePassive = baseModel.EnemyRankEnum == EnemyTagsEnum.None,
                    };
                }
            }
            _listCardInDeck = EnemyController.Instance.OnGenerateCardInDeck(_data);
            if (!_data.iconBoss)
            {
                _objHighLightForBoss.SetActive(_data.typeEnemy == EnemyCardData.TypeEnemyEnum.Boss);
                imgInformationBoss.sprite = _data.iconBoss;
            }
            _iconEnemy.sprite = _data.imgEnemy;
            _txtDescription.text = _data.description;
            _txtEnemyName.text = _data.enemyName;
            _txtHealth.text = UIMath.NiceNumber(_enemyProps.Health);
            _txtDrawCard.text = _data.drawCard.ToString();
            scriptInFormation.SetDataDefault(_data.enemyName, _data.description);
            if (_data.enemyTagsEnum.Contains(EnemyTagsEnum.Debuff))
            {
                if (_data.passiveNameEnemy == EnemyCardData.PassiveEffectEnemy.KingOfKings)
                {
                    var listCardPlayer = PlayCardController.Instance.ListCardInDeck;
                    var getKings = listCardPlayer.Where(x => x.PokerRank == RankCard.King).ToList();
                    foreach (var king in getKings)
                    {
                        _listCardInDeck.Add(king);
                        PlayCardController.Instance.RemoveCardInDeck(king);
                    }
                }
            }
            if (_data.enemyTagsEnum.Contains(EnemyTagsEnum.Debuff))
            {
                if (_data.passiveNameEnemy == EnemyCardData.PassiveEffectEnemy.TheSlimeGirl)
                {
                    var listCardPlayer = PlayCardController.Instance.ListCardInDeck;
                    int randomInt = Random.Range(1, 4);
                    var getRandomSuit = listCardPlayer.Where(x => x.PokerSuit == (SuitCard)randomInt).ToList();
                    foreach (var suit in getRandomSuit)
                    {
                        suit.isDisableCard = true;
                    }
                }
            }
            if (_data.passiveNameEnemy == EnemyCardData.PassiveEffectEnemy.TheStrictMaid)
            {
                PlayCardController.Instance.SubDiscard(0);
            }
            Shuffle();
            
        }
        private void Shuffle()
        {
            System.Random rng = new System.Random();
            int n = _listCardInDeck.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (_listCardInDeck[k], _listCardInDeck[n]) = (_listCardInDeck[n], _listCardInDeck[k]);
            }
        }
        public void SetDataInformation(EnemySo data)
        {
            _data = data;
            if (_data != null)
            {
                _objHighLightForBoss.SetActive(_data.typeEnemy == EnemyCardData.TypeEnemyEnum.Boss);
                _iconEnemy.sprite = _data.imgEnemy;
                _txtEnemyName.text = _data.enemyName;
                _txtHealth.text = $"{EnemyController.Instance.ScaleMaxHealthEnemy((int)_data.baseHealth, (int)_data.multiplierHealth):0}";
                _txtDrawCard.text = _data.drawCard.ToString("F0");
                scriptInFormation.SetDataDefault(_data.enemyName, _data.description);
            }
            else
            {
                _objHighLightForBoss.SetActive(true);
                _iconEnemy.sprite = SpritesManager.Instance.GetSpritesCommonByName("RandomBoss_Icon");
                _txtEnemyName.text = "???";
                _txtHealth.text = "???";
                _txtDrawCard.text = "?";
                scriptInFormation.SetDataDefault("???", "???");
            }
           
        }
        #region  Effect 
        public void UpdateStatusEnemyCard()
        {
            if (_enemyProps == null)
            {
                Debug.LogError("Can not find Enemy right now");
                return;
            }

            var minus =  _enemyProps.Health / _enemyProps.MaxHealth;
            if (_enemyProps.Health > 0)
            {
                mmProgressBar.MinusNumberHp(minus);
                _txtHealth.text = $"{_enemyProps.Health:0}";
            }
            else
            {
                mmProgressBar.UpdateBar01(0);
                _txtHealth.text = string.Format("0");
            }

        }
        public void OnDefeat()
        {
            _isDefeat = true;
            _btnCard.interactable = false;
            EnableObjectFeel(1);
            AudioManager.Instance.PlaySFX(AudioName.Card_Destroyed);
            if (_data.passiveNameEnemy == EnemyCardData.PassiveEffectEnemy.TheSlimeGirl)
            {
                var listCardPlayer = PlayCardController.Instance.ListCardFullDeck;
                foreach (var suit in listCardPlayer)
                {
                    suit.isDisableCard = false;
                }
            }
        }
        public void OnClick()
        {
            _callback?.Invoke(_index);
        }
        public void OnSelectData(int index)
        {
            if (_data.typeEnemy == EnemyCardData.TypeEnemyEnum.Minion)
            {
                if(_objHighLight)
                    _objHighLight.SetActive(index != -1 && _index == index);
                if(_objHighLightForBoss)
                    _objHighLightForBoss.SetActive(false);
            }
            else if(_data.typeEnemy == EnemyCardData.TypeEnemyEnum.Boss)
            {
                if(_objHighLightForBoss)
                    _objHighLightForBoss.SetActive(index != -1 && _index == index);
                if(_objHighLight)
                    _objHighLight.SetActive(false);
            }
            else
            {
                if(_objHighLightForBoss)
                    _objHighLightForBoss.SetActive(false);
                if(_objHighLight) _objHighLight.SetActive(false);
            }
            // AudioManager.Instance.PlaySFX(AudioName.SoundClick);
        }
        public void DeSelectData()
        {
            if(_objHighLightForBoss)
                _objHighLightForBoss.SetActive(false);
            if(_objHighLight)
                _objHighLight.SetActive(false);
            // AudioManager.Instance.PlaySFX(AudioName.BackOrCancel);
        }
        public async Task EffectChooseForAttack()
        {
            _objCard.transform.DOScale(1.1f, 0.5f).SetEase(Ease.OutBack);
            await Task.Delay(200);
        }
        public void EffectClose()
        {
            _objCard.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
        }
        public void EnableObjectFeel(int id)
        {
            switch (id)
            {
                case 0:
                case 1:
                    objDefeat.SetActive(true);
                    onActionDefeated?.Invoke();
                    break;
                case 2:
                    objGetHit.SetActive(true);
                    var getId = (int)PlayCardController.Instance.GetValueHand();
                    EffectManager.Instance.PlayEffect(rectTransform, (EffectName)getId);
                    break;
            }
        }
        #endregion
        // controller 
        public void DrawCardFromHand()
        {
            var count = _lstCardOnHand.Count();
            int cardsSelected = 0;
            while (cardsSelected < (_data.drawCard - count) && _listCardInDeck.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, _listCardInDeck.Count);
                _lstCardOnHand.Add(_listCardInDeck[randomIndex]);
                _listCardInDeck.RemoveAt(randomIndex);
                cardsSelected++;
            }
        }

        public void RemoveCardFromHand(PokerCard card)
        {
            _lstCardOnHand.Remove(card);
        }

        public void OnClickShowInformationEnemy()
        {
            if(StateMachineController.Instance.CurrentState != GameState.PlayerTurn) return;
            if (!objInFormation.activeSelf)
            {
                objInFormation.SetActive(true);
                AudioManager.Instance.PlaySFX(AudioName.SoundClick);
            }
                
        }
    }
    
}
