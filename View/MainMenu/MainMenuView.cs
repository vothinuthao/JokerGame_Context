using System;
using Core.Controller.Observer;
using Core.Entity;
using Core.Observer;
using Frameworks.Base;
using Frameworks.UIAlert;
using Frameworks.UIPopup;
using Manager;
using MoreMountains.Feedbacks;
using Tutorials;
using UI.Popups.ClassParameter;
using UnityEngine;

namespace MainMenu
{
    public class MainMenuView : MonoBehaviour
    {
        [SerializeField]
        private BaseUI view;
        [SerializeField]
        private MMF_Player animation;
        private void OnEnable()
        {
            EventDispatcher.Instance?.RegisterListener(EventID.OnShowMainMenuView, OnShowMainMenuView);
            animation.PlayFeedbacks();
        }
        private void OnDisable()
        {
            EventDispatcher.Instance?.RemoveListener(EventID.OnShowMainMenuView, OnShowMainMenuView);
            animation.PlayFeedbacks();
        }
        private void Start()
        {
            BackgroundController.Instance.OnChangeBackground(BackgroundEnum.BackgroundMain);
        }
        private void OnApplicationQuit()
        {
            AnalyticsManager.LogQuitGame();
        }

        public void OnClickSetting()
        {
            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupSettings);
        }
        private void OnShowMainMenuView(object obj)
        {
            MessageActiveUI msg = (MessageActiveUI)obj;
            if (msg.IsShow)
            {
                view.ShowView();
                this.PostEvent(EventID.OnShowGamePlayView, new MessageActiveUI{IsShow = false});
            }
            else
            {
                OnHide();
            }
        }
        public void OnHide()
        {
            view.HideView();
        }
        public void OnClickPlay()
        {
            OnHide();
            // check player on which state? 
            switch (PlayerDataManager.Instance.Property.CurrentPlayerStay)
            {
                case StateScene.None:
                case StateScene.GameView:
                    this.PostEvent(EventID.OnShowGamePlayView,new MessageActiveUI { IsShow = true });
                    var checkTut = PlayerDataManager.Instance.Property.IsCompletedTutorial;
                    if (!checkTut)
                    {
                        this.PostEvent(EventID.OnActiveTutorial, new MessageActiveIdTutorial()
                        {
                            IdTutorial = PlayerDataManager.Instance.Property.CurrentIdTutorial,
                            IsCompleted = TutorialsView.Instance.CheckIsCompleteTutorial(),
                        });
                    }
                    break;
                case StateScene.ResultView:
                    this.PostEvent(EventID.OnShowResultGameView, new MessageResult { IsShow = true, ResultGame = PlayerDataManager.Instance.Property.PlayerLastResult.ResultLastGame });
                    break;
                case StateScene.ShopView:
                    this.PostEvent(EventID.OnShowShopView, new MessageActiveUI(){ IsShow = true, IsRefesh = true});
                    break;
            }
        }
        public void OnClickFakeGame()
        {
            OnHide();
            // check player on which state? 
            switch (PlayerDataManager.Instance.Property.CurrentPlayerStay)
            {
                case StateScene.None:
                case StateScene.GameView:
                    this.PostEvent(EventID.OnShowGamePlayView,new MessageActiveUI { IsShow = true });
                    break;
                case StateScene.ResultView:
                    this.PostEvent(EventID.OnShowResultGameView, new MessageResult { IsShow = true, ResultGame = PlayerDataManager.Instance.Property.PlayerLastResult.ResultLastGame });
                    break;
                case StateScene.ShopView:
                    this.PostEvent(EventID.OnShowShopView, new MessageActiveUI(){ IsShow = true, IsRefesh = true});
                    break;
            }
        }
        public void ComingSoon()
        {
            // UIManager.Instance.AlertManager.ShowAlertMessage("Coming soon !!!", AlertType.Error);
            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupNotificationBox, new NotificationParameter()
            {
                TextNotification = "Coming soon !!!",
                IconNotificationEnum = IconNotificationEnum.Warning,
            });
        }
        public void OnClickChangeLanguage()
        {
            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupSettingLanguages, false);
        }

        public void OnClickTest()
        {
            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupInAppReview, false);
        }
    }
}