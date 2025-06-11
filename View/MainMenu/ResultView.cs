using System.Collections;
using System.Globalization;
using System.Threading.Tasks;
using Core.Controller.Observer;
using Core.Entity;
using Core.Observer;
using Core.Utils;
using Frameworks.Base;
using Frameworks.Scripts;
using Frameworks.UIButton;
using Frameworks.UIPopup;
using InGame;
using Manager;
using MoreMountains.Feedbacks;
using Runtime.Manager;
using Sirenix.OdinInspector;
using TMPro;
using UI.Popups.ClassParameter;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MainMenu
{
    public enum StateResultEffectEnum{
        None = 0,
        OnResult,
        OnWin,
        OnLose,
    }
    public class ResultView : MonoBehaviour
    {
        [SerializeField] private GameObject objResult;
        [SerializeField] private GameObject objWinFinal;
        [SerializeField] private GameObject objLose;
    
        // text 
        [SerializeField] private TextMeshProUGUI txtBestScore;
        [SerializeField] private TextMeshProUGUI txtMostPlayedHand;
        [SerializeField] private TextMeshProUGUI txtCountMostPlayedHand;
        [SerializeField] private TextMeshProUGUI txtAnte;
        [SerializeField] private TextMeshProUGUI txtRound;
        
        [FoldoutGroup("On Win PerRound")]
        [SerializeField] private TextMeshProUGUI txtCurrentCostWin;
        [FoldoutGroup("On Win PerRound")]
        [SerializeField] private GameObject objPrefabBonus;
        
        [FoldoutGroup("On Win PerRound")]
        [SerializeField] private Transform transContent;
        [FoldoutGroup("On Win PerRound")]
        [SerializeField] private TextMeshProUGUI currentDollar;
        [FoldoutGroup("On Win PerRound")]
        [SerializeField] private TextMeshProUGUI finallyDollar;
        [FoldoutGroup("On Win PerRound")]
        [SerializeField] private UIButton _btnClaimDollar;
        [FoldoutGroup("On Win PerRound")]
        [SerializeField] private TextMeshProUGUI txtDollarOnButton;
        [FoldoutGroup("On Win PerRound")]
        [SerializeField] private TextMeshProUGUI totalDamePerRound;
        
        [FoldoutGroup("On Win Game Effect")]
        [SerializeField] private GameObject objNewRun;
        [FoldoutGroup("On Win Game Effect")]
        [SerializeField] private GameObject objMainMenu;

        [FoldoutGroup("On Win Game Effect")]
        [SerializeField]private GameObject objInfoPanel;
        
        [FoldoutGroup("On Win Per Round Effect")]
        [SerializeField] private GameObject objShowTableWinPerRound;
        [FormerlySerializedAs("objShowTableWinPerRoundE")]
        [FoldoutGroup("On Win Per Round Effect")]
        [SerializeField] private GameObject objShowTableWinPerRoundEffect;
        [FoldoutGroup("On Win Per Round Effect")]
        [SerializeField] private GameObject objCloseTableWinPerRound;
        
        [FoldoutGroup("On Lose Per Round Effect")]
        [SerializeField] private GameObject objShowTableLosePerRound;
        [FoldoutGroup("On Lose Per Round Effect")]
        [SerializeField] private GameObject objCloseTableLosePerRound;
        
        [FoldoutGroup("On Table Effect")]
        [SerializeField] private GameObject objShowTable;
        [FoldoutGroup("On Table Effect")]
        [SerializeField] private GameObject objCloseTable;
        
        [SerializeField] private GameObject objShowTableStats;
        [SerializeField] private GameObject objShowTableWinPerAnte;
        [SerializeField] private GameObject objShowTableLose;
        
        [SerializeField] private UnityEvent onEventEffectDollar;
        private PlayCardController _playCardManager;
        private int _finallyReward = 0;
        private ResultGameEnum _rsEnum = ResultGameEnum.None;
        private void Start()
        {
            objPrefabBonus.SetActive(false);
            UpdateUIWinPerRoundResult();
        }
        private void OnEnable()
        {
            PlayerDataManager.Instance.Property.OnDollaChanged += OnDollarInGameChanged;
        }
        private void OnDisable()
        {
            PlayerDataManager.Instance.Property.OnDollaChanged -= OnDollarInGameChanged;
        }
        private void OnDollarInGameChanged(int amount)
        {
            UpdateUIWinPerRoundResult();
            AudioManager.Instance.PlaySFXWithPitchChanger(AudioName.Chip2, 2);
            onEventEffectDollar?.Invoke();
        }
        public void SetInfoResult(ResultGameEnum rsEnum)
        {
            if (rsEnum == ResultGameEnum.None) return;
            _rsEnum = rsEnum;
            EnemyController.Instance.ClearDataEnemy();
            OnAllHideComponent();
            switch(rsEnum)
            {
                case ResultGameEnum.WinPerRound:
                    OpenWinPerRound();
                    AudioManager.Instance.PlaySFX(AudioName.Win_Game_Per_Round);
                    break;
                case ResultGameEnum.Lose:
                    OpenLose();
                    AudioManager.Instance.PlaySFX((AudioName)Random.Range((int)AudioName.Player_Lose1, (int)AudioName.Player_Lose2));
                    break;
                case ResultGameEnum.WinGame:
                    OpenWinPerRound();
                    AudioManager.Instance.PlaySFX(AudioName.Win_Game);
                    break;
            }
            ShowAllStat();
        }
        private void OpenWinPerRound( )
        {
            EnemyController.Instance.ClearDataEnemy();
            ShopController.Instance.ResetPoolRandom();
            objResult.SetActive(true);
            _finallyReward = 0;
            _btnClaimDollar.Interactable = false;
            _btnClaimDollar.gameObject.SetActive(false);
            GameObjectUtils.Instance.ClearAllChild(transContent.gameObject);
            ShowStatResult();
            objShowTableWinPerRound.SetActive(true);
            objShowTableWinPerRoundEffect.SetActive(false);
            StartCoroutine(Effecting());
        }
        IEnumerator Effecting()
        {
            yield return new WaitForSeconds(0.2f);
            objShowTableWinPerRoundEffect.SetActive(true);
        }
        void OpenLose()
        {
            objShowTableLosePerRound.SetActive(true);
            objShowTable.SetActive(true);
            finallyDollar.text = "${0}";
        }
        public void OnClickChangeToResultView()
        {
            switch(_rsEnum)
            {
                case ResultGameEnum.WinPerRound:
                    CloseWinPerRound();
                    break;
                case ResultGameEnum.Lose:
                    CloseLosePerRound();
                    break;
                case ResultGameEnum.WinGame:
                    CloseWinPerRound();
                    break;
            }
        }
        void CloseWinPerRound()
        {
            objCloseTableWinPerRound.SetActive(true);
        }
        void CloseLosePerRound()
        {
          objCloseTableLosePerRound.SetActive(true);
          objCloseTable.SetActive(true);
         
        }
        public void OnClickChangeView()
        {
            PlayerDataManager.Instance.Property.AddDollar(_finallyReward);
            AnalyticsManager.LogEarnCurrency(TypeCurrency.Dollar,_finallyReward, "Earn_From_WinGame");
            StartCoroutine(AwaitChangeView());
        }
        // ReSharper disable Unity.PerformanceAnalysis
        IEnumerator AwaitChangeView()
        {
            OnHideAllEffect();
            yield return new WaitForSeconds(0.5f);
            this.PostEvent(EventID.OnShowShopView, new MessageActiveUI{IsShow = true, IsRefesh = true});
        }
        public void OnClickToMainView()
        {
            PlayerDataManager.Instance.Property.ResetDataPlayer();
            PlayCardController.Instance.ClearDataPlayInGame();
            CloseLosePerRound();
            this.PostEvent(EventID.OnShowMainMenuView,new MessageActiveUI { IsShow = true });
            PlayerDataManager.Instance.Property.SetStatePlayerStay(StateScene.GameView);
        }
        public void OnShowingMainView()
        {
            this.PostEvent(EventID.OnShowMainMenuView, new MessageActiveUI{IsShow = true});
            AnalyticsManager.LogQuitGame();
        }
        public void NewRun()
        {
            OnHideAllEffect();
            StartCoroutine(AwaitChangeViewNewRun());
        }
        public void WatchAdsForRevive()
        {
            if (IronSourceManager.Instance.IsRewardedAdsAvailable(RewardedAdsPlacement.Game_Over_Revive))
            {
                IronSourceManager.Instance.ShowRewardedVideo(RewardedAdsPlacement.Game_Over_Revive, (success) =>
                {
                    if (success)
                    {
                        UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupNotificationBox, new NotificationParameter()
                        {
                            TextNotification = "Bought back successfully",
                            IconNotificationEnum = IconNotificationEnum.Warning,
                        });
                    }
                });
            }
            else
            {
                Debug.LogError("Ads not available, please try again later!");
            }
        }
        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator AwaitChangeViewNewRun()
        {
            PlayerDataManager.Instance.Property.ResetDataPlayer();
            EnemyController.Instance.ClearDataEnemy();
            PlayCardController.Instance.ClearDataPlayInGame();
            yield return new WaitForSeconds(0.5f);
            this.PostEvent(EventID.OnShowGamePlayView,new MessageActiveUI { IsShow = true });
            if (GlobalSave.Instance.GetValuePlayerReviewed() == 0)
            {
                UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupInAppReview, false);
            }
        }
        public void MainMenu()
        {
            OnHideAllEffect();
            StartCoroutine(AwaitChangeViewMainMenu());
        }
        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator AwaitChangeViewMainMenu()
        {
            yield return new WaitForSeconds(0.5f);
            this.PostEvent(EventID.OnShowMainMenuView, new MessageActiveUI{IsShow = true});
            AnalyticsManager.LogQuitGame();
        }
        private void ShowAllStat()
        {
            txtBestScore.text = $"{PlayerDataManager.Instance.Property.PlayerLastResult.BestScoreLastGame}";
            txtMostPlayedHand.text = $"{PlayerDataManager.Instance.Property.PlayerLastResult.PokerHandValue}";
            txtCountMostPlayedHand.text = $"({PlayerDataManager.Instance.Property.PlayerLastResult.PokerHandValueCount})";
            txtAnte.text = $"{PlayerDataManager.Instance.Property.Ante - 1 }";
            txtRound.text = $"{PlayerDataManager.Instance.Property.Round -1 }";
            
        }
        private void OnAllHideComponent()
        {
            objResult.SetActive(false);
            objWinFinal.SetActive(false);
            objLose.SetActive(false);
        }
        private void OnHideAllEffect()
        {
            if (objResult.activeSelf)
            {
                // onEventHideResult?.Invoke();
            }
        }

        private void ShowStatResult()
        {
            // finalyDollar.text = plat
            _ = EffectAndUIResult();
            JokerCardController.Instance.OnUpdateUI();
        }
        private async Task EffectAndUIResult()
        {
            _btnClaimDollar.Interactable = true;
            _btnClaimDollar.gameObject.SetActive(true);
            objNewRun.SetActive(_rsEnum == ResultGameEnum.WinGame);
            objMainMenu.SetActive(_rsEnum == ResultGameEnum.WinGame);
            objInfoPanel.SetActive(_rsEnum == ResultGameEnum.WinGame);
            txtDollarOnButton.text = "$0";
            totalDamePerRound.text = PlayCardController.Instance.TotalDamage.ToString(CultureInfo.InvariantCulture);
            await Task.Delay(1000);
            var listBonus = ResultController.Instance.GenerateListBonus();
            foreach (var bonus in listBonus)
            {
                GameObject obj = GameObjectUtils.Instance.SpawnGameObject(transContent,objPrefabBonus);
                var sc = obj.GetComponent<BonusVO>();
                sc.OnInitData(bonus);
                obj.SetActive(true);
                await Task.Delay(200);
                _finallyReward += (int)bonus.Quantity;
                finallyDollar.text = $"${_finallyReward}";
            }
            txtDollarOnButton.text = _rsEnum == ResultGameEnum.WinPerRound ? $"Cash Out: ${_finallyReward}" :  $"EndLessMode And Get: ${_finallyReward}";
           
        }

        private void UpdateUIWinPerRoundResult()
        {
            currentDollar.text = PlayerDataManager.Instance.Property.Dollar.ToString();
            // finallyDollar.text = $"${_finallyReward}";
            
        }
    }
}