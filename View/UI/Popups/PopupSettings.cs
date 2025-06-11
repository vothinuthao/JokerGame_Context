using Core.Controller.Observer;
using Core.Entity;
using Frameworks.Base;
using Frameworks.Scripts;
using Frameworks.UIPopup;
using Manager;
using Runtime.Manager;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI.Popups
{
    public class PopupSettings : UIPopup
    {
        // Music 
        [FoldoutGroup("Music Toggle")]
        [SerializeField] GameObject musicActive;
        [FoldoutGroup("Music Toggle")]
        [SerializeField] GameObject musicDeActive;
        
        // Sound
        [FoldoutGroup("Sound Toggle")]
        [SerializeField] GameObject soundActive;
        [FoldoutGroup("Sound Toggle")]
        [SerializeField] GameObject soundDeActive;
        
        // Vibration 
        [FoldoutGroup("Vibration Toggle")]
        [SerializeField] GameObject vibrationActive;
        [FoldoutGroup("Vibration Toggle")]
        [SerializeField] GameObject vibrationDeActive;
        
        // Vibration 
        [FoldoutGroup("Language Toggle")]
        [SerializeField] GameObject englishButton;

        private void Start()
        {
            ActiveMusic();
            ActiveSfx();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        protected override void OnShown()
        {
            ActiveMusic();
            ActiveSfx();
            base.OnShown();
        }

        public void OnClickHidePopup()
        {
            Hide();
        }


        public void ToggleMusic()
        {
            AudioManager.Instance.ToggleMusic();
            ActiveMusic();
        }
        public void ToggleSfx()
        {
            AudioManager.Instance.ToggleSfx();
            ActiveSfx();
        }

        private void ActiveMusic()
        {
            bool check =  AudioManager.Instance.CheckIsMusicActive();
            musicActive.SetActive(!check);
            musicDeActive.SetActive(check);
        }
        private void ActiveSfx()
        {
            bool check =  AudioManager.Instance.CheckIsSfxActive();
            soundActive.SetActive(!check);
            soundDeActive.SetActive(check);
        }
        public void NewRun()
        {
            PlayerDataManager.Instance.Property.ResetDataPlayer();
            EnemyController.Instance.ClearDataEnemy();
            PlayCardController.Instance.ClearDataPlayInGame();
            Hide();
            this.PostEvent(EventID.OnShowGamePlayView,new MessageActiveUI { IsShow = true });
            PlayerDataManager.Instance.Property.SetStatePlayerStay(StateScene.GameView);
            if (GlobalSave.Instance.GetValuePlayerReviewed() == 0)
            {
                UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupInAppReview, false);
            }
            
        } 
        public void MainMenu()
        {
            Hide();
            this.PostEvent(EventID.OnShowMainMenuView, new MessageActiveUI{IsShow = true});
            EnemyController.Instance.ClearDataEnemy();
            AnalyticsManager.LogQuitGame();
        }

        public void OnClickChangeLanguage()
        {
            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupSettingLanguages, false);
        }
    }
}
