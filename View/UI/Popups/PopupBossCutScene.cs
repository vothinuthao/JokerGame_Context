using System;
using System.Collections.Generic;
using System.Linq;
using Enemy;
using Frameworks.UIPopup;
using Runtime.Manager;
using Sirenix.OdinInspector;
using TMPro;
using UI.BaseColor;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popups
{
    [Serializable]
    public enum StateBossCutSceneEnum
    {
        None = 0,
        Victory = 1,
        Defeat = 2,
        Encounter = 3,
    }
    
    public class MessageShowCutScene
    {
        public StateBossCutSceneEnum StateBossCutSceneEnum;
        public int IDBoss;
        public Action<bool> Callback = default;
    }
    [Serializable]
    public class ObjectOfState
    {
        public StateBossCutSceneEnum stateBossCutSceneEnum;
        public GameObject objectStateBoss;
        [MultiLineProperty(5)]
        public string strBossSpeech;
        

    }
    [Serializable]
    public class StateBossCutScene
    {
        public EnemyCardData.PassiveEffectEnemy enemyNameEnum;
        public GameObject objStateCutScene;
        public List<ObjectOfState> listObjectStateBoss;

    }
    public class PopupBossCutScene : UIPopup
    {
        [SerializeField] private TextMeshProUGUI textBossName;
        [SerializeField] private TextMeshProUGUI textBossSpeech;
        [SerializeField] private List<StateBossCutScene> listStateBossCutScenes;

        [FoldoutGroup("Victory Config")]
        public GameObject objectVictory;
        
        [FoldoutGroup("Defeated Config")]
        public GameObject objectDefeat;
        
        [FoldoutGroup("Encounter Config")]
        public GameObject objectEncounter;
        [FoldoutGroup("Encounter Config")]
        public TextMeshProUGUI textPassive;
        [FoldoutGroup("Encounter Config")]
        public Image imgIconPassive;
        private Action<bool> _callback;
        private MessageShowCutScene _messageBoss;
        // ReSharper disable Unity.PerformanceAnalysis
        protected override void OnShown()
        {
            base.OnShown();
            HideAllBoss();
            _messageBoss = (MessageShowCutScene)Parameter;
            ShowDataBossSpeech();
        }

        public void OnClickHide()
        {
            Hide();
            _callback?.Invoke(true);
        }

        private void ShowDataBossSpeech()
        {
            StateBossCutScene getStateClass = GetObjectState();
            var listState = getStateClass?.listObjectStateBoss.FirstOrDefault(x=> x.stateBossCutSceneEnum == _messageBoss.StateBossCutSceneEnum);
            // set text Object 
            objectVictory.SetActive(_messageBoss.StateBossCutSceneEnum == StateBossCutSceneEnum.Victory);
            objectDefeat.SetActive(_messageBoss.StateBossCutSceneEnum == StateBossCutSceneEnum.Defeat);
            objectEncounter.SetActive(_messageBoss.StateBossCutSceneEnum == StateBossCutSceneEnum.Encounter);
            if (_messageBoss.StateBossCutSceneEnum == StateBossCutSceneEnum.Encounter)
            {
                var enemySo = EnemyController.Instance.GetEnemyById(_messageBoss.IDBoss);
                textPassive.text = ColorSchemeManager.Instance.ConvertColorTextFromSymbol(enemySo.description);
                imgIconPassive.sprite = enemySo.iconBoss;
            }
            if (listState == null) return;
            listState.objectStateBoss.SetActive(true);
            // textBossSpeech.text = listState.strBossSpeech;
            textBossName.text = getStateClass.enemyNameEnum.ToString();
            getStateClass.objStateCutScene.SetActive(true);
            _callback = _messageBoss.Callback;
        }

        private StateBossCutScene GetObjectState()
        {
            var getEnum = (EnemyCardData.PassiveEffectEnemy)_messageBoss.IDBoss;
            var getStateClass = listStateBossCutScenes.FirstOrDefault(x => x.enemyNameEnum == getEnum);
           

            return getStateClass;
            
        }

        private void HideAllBoss()
        {
            foreach (var boss in listStateBossCutScenes)
            {
                boss.objStateCutScene.SetActive(false);
                boss.listObjectStateBoss.ForEach(x=>x.objectStateBoss.SetActive(false));
            }
        }
    }
}
