using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Core.Controller.Observer;
using Core.Entity;
using Core.Utils;
using Entity;
using Manager;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Tutorials
{
    public class TutorialsView : ManualSingletonMono<TutorialsView>
    {
        // [SerializeField] private GameObject prefabTutorial;
        // [SerializeField] private Transform parentSpawn;
        
        [SerializeField]
        private List<TutorialsName> listTutorialsNames = new List<TutorialsName>();
        [FoldoutGroup("Tutorials")]
        [SerializeField] private List<PokerCard> listPokerCardTutorial;
        [FoldoutGroup("Tutorials")]
        [SerializeField] private List<PokerCard> listPokerCardAfterDiscard;

        public List<PokerCard> ListPokerCardTutorial => listPokerCardTutorial;
        public List<PokerCard> ListPokerCardAfterDiscard => listPokerCardAfterDiscard;
         public bool passTut = true;
        [SerializeField]
        private TutorialState _currentTutorialState = TutorialState.None;
        private int _currentIndex = 0;
        
        private DateTime _startTime;
        public void Start()
        {
#if UNITY_EDITOR
            if (passTut)
            {
                _currentTutorialState = TutorialState.Completed;
                PlayerDataManager.Instance.Property.OnCompleteTutorial();
            }
#endif
            foreach (var tutorials in listTutorialsNames)
            {
                if(tutorials!=null)
                    tutorials.tutorialVo.OnInitData(tutorials);
            }

            _currentIndex = PlayerDataManager.Instance.Property.CurrentIdTutorial;
            HideAllPanel();
            _startTime =  DateTime.Now;
            
        }
        private void OnEnable()
        {
            EventDispatcher.Instance?.RegisterListener(EventID.OnActiveTutorial, OnActiveTutorial);
        }
        private void OnDisable()
        {
            EventDispatcher.Instance?.RemoveListener(EventID.OnActiveTutorial, OnActiveTutorial);
        }

        public bool CheckIsCompleteTutorial()
        {
            _currentTutorialState = (TutorialState)_currentIndex;
            if (_currentTutorialState == TutorialState.None)
                return true;
            var currentTut = listTutorialsNames.Find(x => x.idTutorial == _currentTutorialState);
            return currentTut.IsComplete;
        }
        
        private void OnActiveTutorial(object obj)
        {
            DateTime endTime = DateTime.Now;
            TimeSpan duration = endTime - _startTime;
            AnalyticsManager.LogTutorialStep(_currentIndex, duration.TotalSeconds.ToString(CultureInfo.InvariantCulture));

            MessageActiveIdTutorial msg = (MessageActiveIdTutorial)obj;
            _currentIndex = msg.IdTutorial;
            ShowNextPanel(_currentIndex);
          
        }
        
        private void ShowNextPanel(int idTut)
        {
            HideAllPanel();
            _currentIndex = idTut == 0 ? 1 : idTut;
            _currentTutorialState = (TutorialState)_currentIndex;
            if (_currentIndex <= listTutorialsNames.Count)
            {
                var findObject = listTutorialsNames.Find(x => x.idTutorial == _currentTutorialState);
                PlayerDataManager.Instance.Property.SetCurrentTutorial(_currentIndex); 
                findObject.tutorialVo.ShowPanel();
             
                //analytics
                _startTime =  DateTime.Now;
              
            }
            else
            {
                _currentTutorialState = TutorialState.Completed;
                PlayerDataManager.Instance.Property.OnCompleteTutorial();
                Debug.Log("Tutorial Completed!");
            }
        }

        private void HideAllPanel()
        {
            if(!listTutorialsNames.Any()) return;
            foreach (var tutorials in listTutorialsNames)
            {
                tutorials.tutorialVo.HidePanel();
            }
        }

        public bool CheckStraightPoker()
        {
            var listCurrenSelect = PlayCardController.Instance.ListCardOnSelect;
            for (int i = 0; i < listCurrenSelect.Count - 1; i++)
            {
                if (listCurrenSelect[i + 1].PokerCard.PokerRank - listCurrenSelect[i].PokerCard.PokerRank != 1)
                {
                    return false;
                }
            }
            return true;
        }

        public void SetCanvasObjectTut(TutorialState state, bool setActive = false)
        {
            
            var tutorialsName = listTutorialsNames.FirstOrDefault(x => state.Equals(x.idTutorial));

            if (tutorialsName != null)
            {
                tutorialsName.canvasTarget.sortingOrder = 0;
                tutorialsName.canvasTarget.overrideSorting = false;
            }
              
        }
    }
}
