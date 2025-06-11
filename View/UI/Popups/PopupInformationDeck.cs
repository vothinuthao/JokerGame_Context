using System;
using Core.Entity;
using Core.Manager;
using Core.Utils;
using Frameworks.Scripts;
using Frameworks.UIPopup;
using InGame;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace UI.Popups
{
    public enum InformationTab
    {
        None = 0,
        HandValueTab = 1,
        ViewDeck = 2,
    }
    public class PopupInformationDeck : UIPopup
    {
        [FoldoutGroup("Hand Type")]
        [SerializeField] private GameObject objPrefabHandType;
        [FoldoutGroup("Hand Type")]
        [SerializeField] private Transform transParentSpawn;
        [FoldoutGroup("Hand Type")]
        [SerializeField] private GameObject objSampleType;
        [FoldoutGroup("Hand Type")]
        [SerializeField] private Transform transParentSampleTypeSpawn;
        [FoldoutGroup("View Deck")]
        [SerializeField] private ViewDeckView viewDeckView;
        
        
        [FoldoutGroup("Tab")]
        [SerializeField] private GameObject objArrowHandType;
        [FoldoutGroup("Tab")]
        [SerializeField] private GameObject objArrowViewDeck;
        [FoldoutGroup("Tab")]
        [SerializeField] private GameObject tabHandType;
        [FoldoutGroup("Tab")]
        [SerializeField] private GameObject tabViewDeck;

        private PokerHandValue _currentValue;
        private InformationTab _currentTab;
        private void Start()
        {
            objPrefabHandType.SetActive(false);
            objSampleType.SetActive(false);
            _currentValue = PokerHandValue.High;
            OnClickChangeTab(1);
        }
        // ReSharper disable Unity.PerformanceAnalysis
        protected override void OnShown()
        {
            base.OnShown();
            // hand type
            OnShowHandType();
            CreateSampleDeck();
            // view deck
            viewDeckView.OnShow();
        }
        public void OnClickChangeTab(int idTab)
        {
            tabHandType.SetActive(false);
            tabViewDeck.SetActive(false);
            objArrowHandType.SetActive(false);
            objArrowViewDeck.SetActive(false);
            
            if (idTab == 1)
            {
                tabHandType.SetActive(true);
                objArrowHandType.SetActive(true);
            }
            else 
            {
                tabViewDeck.SetActive(true);
                objArrowViewDeck.SetActive(true);
            }
            // AudioManager.Instance.PlaySFX(AudioName.SoundClick);
        }
        //hand type 
        private void OnShowHandType()
        {
            GameObjectUtils.Instance.ClearAllChild(transParentSpawn.gameObject);
            foreach (PokerHandValue pokerHand in Enum.GetValues(typeof(PokerHandValue)))
            {
                if (pokerHand != PokerHandValue.None)
                {
                    GameObject obj = GameObjectUtils.Instance.SpawnGameObject(transParentSpawn, objPrefabHandType);
                    var script = obj.GetComponent<HandTypeVo>();
                    script.OnInit(OnCallBackWhenClick);
                    script.SetDataHandType(pokerHand, pokerHand == _currentValue );
                    obj.SetActive(true);
                }
            }
        }
        private void CreateSampleDeck()
        {
            if(_currentValue == PokerHandValue.None) return;
            var configHand = ConfigManager.configValueHands.GetValueByID((int)_currentValue);
            var listRank = GameObjectUtils.Instance.ConvertStringToList(configHand.exampleRank);
            var listSuit = GameObjectUtils.Instance.ConvertStringToList(configHand.exampleSuit);
            GameObjectUtils.Instance.ClearAllChild(transParentSampleTypeSpawn.gameObject);
            for (var i = 0; i < listRank.Count; i++)
            {
                GameObject obj = GameObjectUtils.Instance.SpawnGameObject(transParentSampleTypeSpawn, objSampleType);
                var script = obj.GetComponent<InformationCardVo>();
                script.SetData((RankCard)listRank[i],(SuitCard)listSuit[i], i < configHand.availableCount);
                obj.SetActive(true);
            }
        }
        private void OnCallBackWhenClick(PokerHandValue value)
        {
            _currentValue = value;
            OnShowHandType();
            CreateSampleDeck();
        }
        public void OnClickHide()
        {
            Hide();
        }
    }
}
