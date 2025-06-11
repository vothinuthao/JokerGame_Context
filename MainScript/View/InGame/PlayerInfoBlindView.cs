using System.Collections;
using System.Globalization;
using System.Linq;
using Core.Controller.Observer;
using Core.Manager;
using DG.Tweening;
using Enemy;
using Frameworks.Base;
using Frameworks.Scripts;
using Frameworks.UIPopup;
using Frameworks.Utils;
using MainMenu;
using Manager;
using MoreMountains.Tools;
using Runtime.Manager;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace InGame
{
    public class PlayerInfoBlindView : MonoBehaviour
    {
        [SerializeField] private GameObject topPanel;
        [SerializeField] private MMProgressBar mmProgressBarHp;
        [SerializeField] private MMProgressBar mmProgressBarShield;
        [SerializeField] private GameObject objShieldSlide;
        [SerializeField] private GameObject objProgressBar;
        
        [FoldoutGroup("Level Group")]
        [SerializeField]
        private GameObject LevelSize;
        [FoldoutGroup("Level Group")]
        [SerializeField]
        private TextMeshProUGUI txtRoundName;
        [FoldoutGroup("Score Group")]
        [SerializeField]
        private GameObject _roundScores;
        [FormerlySerializedAs("_txtValueHand")]
        [FoldoutGroup("Score Group")]
        [SerializeField]
        private TextMeshProUGUI _txtDamageAttack;
        [FoldoutGroup("Score Group")]
        [SerializeField]
        private TextMeshProUGUI _txtChipScore;
        [FoldoutGroup("Score Group")]
        [SerializeField]
        private TextMeshProUGUI _txtMultScore;
        [FoldoutGroup("Score Group")]
        [SerializeField]
        private TextMeshProUGUI _txtCurrency;
        [FoldoutGroup("Score Group")]
        [SerializeField]
        private ParticleSystem _vfxFireChip;
        [FoldoutGroup("Score Group")]
        [SerializeField]
        private ParticleSystem _vfxFireMult;
        [FoldoutGroup("Point Group")]
        [SerializeField]
        private GameObject HandValueLevel;
        [FoldoutGroup("Point Group")]
        [SerializeField]
        private GameObject blindNameBox;
        [FoldoutGroup("Score Group")]
        [SerializeField]
        private Material _materialChip;
        [FoldoutGroup("Score Group")]
        [SerializeField]
        private Material _materialMult;
        [FoldoutGroup("Score Group")]
        [SerializeField]
        private Material _materialChipSub;
        [FoldoutGroup("Score Group")]
        [SerializeField]
        private Material _materialMultSub;
        [FoldoutGroup("Point Group")]
        [SerializeField]
        private TextMeshProUGUI _txtDiscards;
        [FoldoutGroup("Point Group")]
        [SerializeField]
        private TextMeshProUGUI _txtHands;
        [SerializeField]
        private TextMeshProUGUI _txtHealth;
        [SerializeField]
        private TextMeshProUGUI _txtShield;
        [SerializeField]
        private TextMeshProUGUI txtAnte;
        [SerializeField]
        private TextMeshProUGUI txtRound;
        [SerializeField]
        private GameObject InfomationPlayer;
        [SerializeField]
        private GameObject objDisableDisCard;
        [FoldoutGroup("Inventory")] [SerializeField]
        private InventoryView inventoryView;
        [SerializeField]
        private PlayCardView _playCardView;
        private PlayCardController _playCardManager;
        private PlayerDataManager _playerDataManager;
        private int _targetScore = 800;
        private float _currentHp;
        private float _currentShield;
        private bool _playingEffect= false;
        void Start()
        {
            _playerDataManager = PlayerDataManager.Instance;
            if (PlayCardController.IsInstanceValid())
            {
                _playCardManager = PlayCardController.Instance;
                _playCardManager.OnChipChanged += OnChipChanged;
                _playCardManager.OnMultChanged += OnMultChanged;
                _playCardManager.OnRoundScoreChanged += OnRoundScoreChanged;
                _playCardManager.OnDiscardsChanged += OnDiscardsChanged;
                _playCardManager.OnHandsChanged += OnHandsChanged;
                _playCardManager.OnHealthChanged += OnMaxHealthChanged;
                _playCardManager.OnShieldChanged += OnShieldChanged;
                _playerDataManager.Property.OnDollaChanged += OnDollarChanged;
                _playerDataManager.Property.OnRoundChanged += OnRoundChanged;
                _playerDataManager.Property.OnAnteChanged += OnAnteChanged;
            }
            EventDispatcher.Instance?.RegisterListener(EventID.OnShowShopView, OnShowShopView);
            EventDispatcher.Instance?.RegisterListener(EventID.OnShowGamePlayView, OnShowGamePlayView);
            EventDispatcher.Instance?.RegisterListener(EventID.OnShowOpenPackView, OnShowOpenPackView);
            inventoryView.OnInit();
            UpdateGameProgress();
            var nameShader = "_FadePower";
            _materialChip.SetFloat(nameShader, 2f);
            _materialChipSub.SetFloat(nameShader, 2f);
            _materialMult.SetFloat(nameShader, 2f);
            _materialMultSub.SetFloat(nameShader, 2f);
            _vfxFireChip.gameObject.SetActive(false);
            _vfxFireMult.gameObject.SetActive(false);
            _vfxFireChip.Stop();
            _vfxFireMult.Stop();
           
        }
        private void OnApplicationQuit()        
        {
            if (PlayCardController.IsInstanceValid())
            {
                _playCardManager = PlayCardController.Instance;
                _playCardManager.OnChipChanged -= OnChipChanged;
                _playCardManager.OnMultChanged -= OnMultChanged;
                _playCardManager.OnRoundScoreChanged -= OnRoundScoreChanged;
                _playCardManager.OnDiscardsChanged -= OnDiscardsChanged;
                _playCardManager.OnHandsChanged -= OnHandsChanged;
                _playCardManager.OnHealthChanged -= OnMaxHealthChanged;
                _playCardManager.OnShieldChanged -= OnShieldChanged;
                _playerDataManager.Property.OnDollaChanged -= OnDollarChanged;
                _playerDataManager.Property.OnRoundChanged -= OnRoundChanged;
                _playerDataManager.Property.OnAnteChanged -= OnAnteChanged;
            }
            EventDispatcher.Instance?.RemoveListener(EventID.OnShowGamePlayView, OnShowGamePlayView);
            EventDispatcher.Instance?.RemoveListener(EventID.OnShowShopView, OnShowShopView);
            EventDispatcher.Instance?.RemoveListener(EventID.OnShowOpenPackView, OnShowOpenPackView);
        }
        public void SetInfoPlayer()
        {
            _txtDiscards.text = PlayCardController.Instance.Property.Discards.ToString();
            _txtHands.text = PlayCardController.Instance.Property.Hands.ToString();
            _txtHealth.text = PlayCardController.Instance.Property.Health.ToString();
            _txtShield.text = PlayCardController.Instance.Property.Shield.ToString();
            _currentHp = PlayCardController.Instance.Property.Health;
            _currentShield = PlayCardController.Instance.Property.Shield;
            
            objShieldSlide.SetActive(PlayCardController.Instance.Property.Shield > 0);
            // mmProgressBarHp.SetBar01(1);
            // mmProgressBarShield.SetBar01(1);
        }
        public void OnClickSetting()
        {
            if (!PlayerDataManager.Instance.Property.IsCompletedTutorial) return;
            if(StateMachineController.Instance.CurrentState != GameState.PlayerTurn) return;
            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupSettings );
            
        }
        public void OnClickInformationPopup()
        {
            if(StateMachineController.Instance.CurrentState != GameState.PlayerTurn) return;
            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupMapProgression, false);
        }
        public void SetActiveCurrentRound(bool isShow)
        {
            HandValueLevel.SetActive(isShow);
            blindNameBox.SetActive(isShow);
        }
        public void UpdateGameProgress()
        {
            var chip = PlayCardController.Instance.Property.Chip;
            var mult = PlayCardController.Instance.Property.Mult;
            var configRound = ConfigManager.configPlayRound.GetValueByRound(PlayerDataManager.Instance.Property.Round % (ConfigManager.configPlayRound.Records.Count - 1));
            _txtChipScore.text = chip.ToString(CultureInfo.InvariantCulture);
            _txtMultScore.text = mult.ToString(CultureInfo.InvariantCulture);
            _txtCurrency.text = PlayerDataManager.Instance.Property.Dollar.ToString();
            txtAnte.text = $"{PlayerDataManager.Instance.Property.Ante.ToString()}";
            txtRound.text = $"{PlayerDataManager.Instance.Property.Round.ToString()}";
            txtRoundName.text = string.IsNullOrEmpty(EnemyController.Instance.HaveBossInRoundAndReTurnNameRound()) ? configRound.nameRound : EnemyController.Instance.HaveBossInRoundAndReTurnNameRound();
            // StartCoroutine(WaitAndStartEffect());
            var total = chip * mult;
            if (total >= 500 && !_playingEffect)
            {
                PlayEffect();
                _playingEffect = true;
            }
            else if (total == 0)
            {
                if (_playingEffect)
                {
                    _playingEffect = false;
                    StopEffect();
                }
                    
            }
        }
        public void UpdateScore()
        {
            float point = PlayCardController.Instance.Property.TotalDamage;
            _txtDamageAttack.text = $"Damage Attack: {point.ToString(CultureInfo.InvariantCulture)}";

        }
        private void UpdateHealth()
        {
            float maxHp = PlayCardController.Instance.Property.MaxHealth;
            float hp = PlayCardController.Instance.Property.Health;
            float perCentNumber = hp / maxHp;
            if(hp > 0)
              mmProgressBarHp.MinusNumberHp(hp != 0 ? perCentNumber : 0);
            else
                mmProgressBarHp.PlusNumberHp(hp != 0 ? perCentNumber : 0);
            
            _currentHp = hp;
            _txtHealth.text = hp.ToString(CultureInfo.InvariantCulture);
        }
        private void UpdateShield()
        {
            objShieldSlide.SetActive(PlayCardController.Instance.Property.Shield > 0);
            float maxShield = PlayCardController.Instance.Property.MaxShield;
            float shield = PlayCardController.Instance.Property.Shield;
            // float cal = _currentShield - shield; // 80 - 50
            float perCentNumber = shield / maxShield;
            if(shield > 0)
                mmProgressBarShield.MinusNumberHp(shield != 0 ? perCentNumber : 0);
            else
                mmProgressBarShield.PlusNumberHp(shield != 0 ? perCentNumber : 0);
            _currentShield = shield;
            _txtShield.text = shield.ToString(CultureInfo.InvariantCulture);
        }
        #region callback
        private void OnChipChanged(float chip)
        {
            if (!this.gameObject.activeInHierarchy) return;
            UpdateGameProgress();
        }
        private void OnMultChanged(float mult)
        {
            if (!this.gameObject.activeInHierarchy) return;
            UpdateGameProgress();
        }
        private void OnDiscardsChanged(int discard)
        {
            objDisableDisCard.gameObject.SetActive(true);
            var checkMaidBoss = EnemyController.Instance.ListEnemyScript.FirstOrDefault(x => x.EnemySO.passiveNameEnemy == EnemyCardData.PassiveEffectEnemy.TheStrictMaid);
            if (checkMaidBoss != null)
            {
                objDisableDisCard.gameObject.SetActive(!checkMaidBoss.IsDefeat);
            }
            else
            {
                objDisableDisCard.gameObject.SetActive(false);
            }
            if (!this.gameObject.activeInHierarchy) return;
            _txtDiscards.text = PlayCardController.Instance.Property.Discards.ToString();

        }
        private void OnHandsChanged(int hands)
        {
            if (!this.gameObject.activeInHierarchy) return;
            _txtHands.text = PlayCardController.Instance.Property.Hands.ToString();
        }
        private void OnRoundScoreChanged(float score)
        {
            if (!this.gameObject.activeInHierarchy) return;
            UpdateScore();
        }
        private void OnMaxHealthChanged(int hp)
        {
            if (!this.gameObject.activeInHierarchy) return;
            UpdateHealth();
        }
        private void OnShieldChanged(int hp)
        {
            if (!this.gameObject.activeInHierarchy) return;
            UpdateShield();
        }
        private void OnDollarChanged(int dollar)
        {
            if (!this.gameObject.activeInHierarchy) return;
            UpdateGameProgress();
        }
        private void OnRoundChanged(int round)
        {
            if (!this.gameObject.activeInHierarchy) return;
            UpdateGameProgress();
        }
        private void OnAnteChanged(int ante)
        {
            if (!this.gameObject.activeInHierarchy) return;
            UpdateGameProgress();
        }
        #endregion
        private void StopEffect()
        {
            var duration = 1f;
            var minVolumn = 0f;
            var maxVolumn = 100f;
            var nameShader = "_FadePower";
            DOTween.To(() => _materialChip.GetFloat(nameShader),
                x => _materialChip.SetFloat(nameShader, x),
                minVolumn, duration); 
            DOTween.To(() => _materialChipSub.GetFloat(nameShader),
                x => _materialChipSub.SetFloat(nameShader, x),
                minVolumn, duration); 
            DOTween.To(() => _materialMult.GetFloat(nameShader),
                x => _materialMult.SetFloat(nameShader, x),
                minVolumn, duration);
            DOTween.To(() => _materialMultSub.GetFloat(nameShader),
                x => _materialMultSub.SetFloat(nameShader, x),
                minVolumn, duration).OnComplete(() =>
            {
                _vfxFireChip.gameObject.SetActive(false);
                _vfxFireMult.gameObject.SetActive(false);
                
            });
            AudioManager.Instance.StopMusic(AudioName.Fire3);
        }
        private void PlayEffect()
        {
            var duration = 1f;
            var minVolumn = 2f;
            var maxVolumn = 100f;
            var nameShader = "_FadePower";
            _vfxFireChip.gameObject.SetActive(true);
            _vfxFireMult.gameObject.SetActive(true);
            AudioManager.Instance.PlaySFX(AudioName.Fire3);
            _vfxFireChip.Play();
            _vfxFireMult.Play();
            DOTween.To(() => _materialChip.GetFloat(nameShader),
                x => _materialChip.SetFloat(nameShader, x),
                maxVolumn, duration); 
            DOTween.To(() => _materialChipSub.GetFloat(nameShader),
                x => _materialChipSub.SetFloat(nameShader, x),
                maxVolumn, duration); 
            DOTween.To(() => _materialMult.GetFloat(nameShader),
                x => _materialMult.SetFloat(nameShader, x),
                maxVolumn, duration);
            DOTween.To(() => _materialMultSub.GetFloat(nameShader),
                x => _materialMultSub.SetFloat(nameShader, x),
                maxVolumn, duration);
            }
        private void OnShowGamePlayView(object obj)
        {
            MessageActiveUI msg = (MessageActiveUI)obj;
            objProgressBar.SetActive(msg.IsShow);
            topPanel.SetActive(true);
        }
        private void OnShowShopView(object obj)
        {
            MessageActiveUI msg = (MessageActiveUI)obj;
            objProgressBar.SetActive(!msg.IsShow);
            topPanel.SetActive(true);
        }
        private void OnShowOpenPackView(object obj)
        {
            MessagePack msg = (MessagePack)obj;
            if (msg.IsShow)
            {
                topPanel.SetActive(false);
            }
        }
    }
}
