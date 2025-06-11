using System;
using Core.Controller.Observer;
using Core.Entity;
using Core.Observer;
using Frameworks.Base;
using Frameworks.Scripts;
using Frameworks.UIPopup;
using InGame;
using Manager;
using Player;
using Runtime.Manager;
using Sirenix.OdinInspector;
using Tutorials;
using UnityEngine;

namespace MainMenu
{
    public class GamePlayView : MonoBehaviour
    {
        
        //scripts
        [SerializeField] private BaseUI view;
        [SerializeField] private PlayCardView playCardView;
        [SerializeField] private PlayerInfoBlindView playerInfoBlindView;
        [SerializeField] private ResultView resultViewScript;
        [SerializeField] private ShopView shopViewScript;
        [SerializeField] private OpenPackView openPackViewScript;
        [SerializeField] private TutorialsView tutorialsViewScript;
        
        // view
        [SerializeField] private GameObject playView;
        [SerializeField] private GameObject shopView;
        [SerializeField] private GameObject resultView;
        [SerializeField] private GameObject openPackView;
        [SerializeField] private GameObject tutorialView;
        
        [FoldoutGroup("Sound")]
        [SerializeField] private AudioName playViewSound;
        [FoldoutGroup("Sound")]
        [SerializeField] private AudioName shopViewSound;
        [FoldoutGroup("Sound")]
        [SerializeField] private AudioName resultViewSound;
        [FoldoutGroup("Sound")]
        [SerializeField] private AudioName openPackViewSound;
        [FoldoutGroup("Sound")]
        [SerializeField] private AudioName tutorialViewSound;
        private void OnEnable()
        {
            EventDispatcher.Instance?.RegisterListener(EventID.OnShowMainMenuView, OnShowMenu);
            EventDispatcher.Instance?.RegisterListener(EventID.OnShowGamePlayView, OnShowGamePlayView);
            EventDispatcher.Instance?.RegisterListener(EventID.OnShowShopView, OnShowShopView);
            EventDispatcher.Instance?.RegisterListener(EventID.OnShowResultGameView, OnShowResultGameView);
            EventDispatcher.Instance?.RegisterListener(EventID.OnShowOpenPackView, OnShowOpenPackView);
        }
        private void OnDisable()
        {
            EventDispatcher.Instance?.RemoveListener(EventID.OnShowMainMenuView, OnShowMenu);
            EventDispatcher.Instance?.RemoveListener(EventID.OnShowGamePlayView, OnShowGamePlayView);
            EventDispatcher.Instance?.RemoveListener(EventID.OnShowShopView, OnShowShopView);
            EventDispatcher.Instance?.RemoveListener(EventID.OnShowResultGameView, OnShowResultGameView);
            EventDispatcher.Instance?.RemoveListener(EventID.OnShowOpenPackView, OnShowOpenPackView);
        }
        public void OnClickSetting()
        {
            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupSettings);
        }
        private void Start()
        {
            Application.targetFrameRate = 999;
            playView.SetActive(false);
            shopView.SetActive(false);
            resultView.SetActive(false);
            openPackView.SetActive(false);
            playCardView.InitDataPlayView();
        }
        private void OnShow()
        {
            view.ShowView();
            AudioManager.Instance.OnViewHided(AudioName.BGM_Menu);
        }
        private void OnHide()
        {
            view.HideView();
        }
        private void OnShowGamePlayView(object obj)
        {
            MessageActiveUI msg = (MessageActiveUI)obj;
            if (msg.IsShow)
            {
               ActiveView(1);
               _ = playCardView.PlayGame();
               playerInfoBlindView.SetInfoPlayer();
               playerInfoBlindView.UpdateGameProgress();
               playerInfoBlindView.UpdateScore();
            }
            else
            {
                view.HideView();
            }
            // PlayerDataManager.Instance.Property.SetStatePlayerStay(StateScene.GameView);
        }
        private void OnShowShopView(object obj)
        {
            MessageActiveUI msg = (MessageActiveUI)obj;
           
            if (msg.IsShow)
            { 
                ActiveView(2);
            }
            if (msg.IsRefesh)
            {
                shopViewScript.InitDataShop();
            }
            PlayerDataManager.Instance.Property.SetStatePlayerStay(StateScene.ShopView);
        }
        private void OnShowResultGameView(object obj)
        {
            MessageResult msg = (MessageResult)obj;
            if (msg.IsShow)
            {
                ActiveView(3);
                PlayerDataManager.Instance.Property.SetStatePlayerStay(StateScene.ResultView);
                resultViewScript.SetInfoResult(msg.ResultGame);
            }
          
        }
        private void OnShowOpenPackView(object obj)
        {
            MessagePack msg = (MessagePack)obj;
            if (msg.IsShow)
            {
                ActiveView(4);
                openPackViewScript.OnInitPack(msg.DataPack);
            }
        }
        
        private void ActiveView(int idView)
        {
            playView.SetActive(false);
            shopView.SetActive(false);
            resultView.SetActive(false);
            openPackView.SetActive(false);
            OnShow();
            switch (idView)
            {
                case 1:
                    playView.SetActive(true);
                    AudioManager.Instance.OnViewLoaded(playViewSound);
                    playerInfoBlindView.SetActiveCurrentRound(true);
                    break;
                case 2:
                    shopView.SetActive(true);
                    AudioManager.Instance.OnViewLoaded(shopViewSound);
                    BackgroundController.Instance.OnChangeBackground(BackgroundEnum.BackgroundBlue);
                    playerInfoBlindView.SetActiveCurrentRound(false);
                    break;
                case 3:
                    resultView.SetActive(true);
                    AudioManager.Instance.OnViewLoaded(resultViewSound);
                    break;
                case 4:
                    openPackView.SetActive(true);
                    AudioManager.Instance.OnViewLoaded(openPackViewSound);
                    break;
            }
        }
        private void OnShowMenu(object obj)
        {
            MessageActiveUI msg = (MessageActiveUI)obj;
            if (msg.IsShow)
            {
                playView.SetActive(false);
            }
        }
        
    }
}