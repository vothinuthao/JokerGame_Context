using System;
using System.Linq;
using Core.Manager;
using Core.Manager.Configs;
using Core.Utils;
using Manager;
using Manager.Configs;
using Runtime.Manager;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace InGame
{
    public class RoundInfoVo : MonoBehaviour
    {
        [FoldoutGroup("Info Round")]
        public RectTransform rectTransformParent;
        [FoldoutGroup("Info Round")]
        [SerializeField] private TextMeshProUGUI textNameRound;
        [FoldoutGroup("Info Round")]
        [SerializeField] private GameObject objectCurrentRound;
        [FoldoutGroup("Info Round")]
        [SerializeField] private GameObject objectBottomRound;
        [FoldoutGroup("Info Round")]
        [SerializeField] private GameObject objectFirstRound;
        [FoldoutGroup("Info Round")]
        [SerializeField] private GameObject objectTopRound;
        
        [FoldoutGroup("Info Enemy Round")]
        [SerializeField] private Transform transformParentEnemySpawn;
        [FoldoutGroup("Info Enemy Round")]
        [SerializeField] private GameObject objInfoEnemy;
        private ConfigPlayRoundsRecord _data;

        private void Start()
        {
            objInfoEnemy.SetActive(false);
        }

        public void SetDataRoundProgress(ConfigPlayRoundsRecord data)
        {
            _data = data;
            int currentRound = PlayerDataManager.Instance.Property.Round %
                               (ConfigManager.configPlayRound.Records.Count - 1);
            //cal round per threshold 20 
            var numberThresholdsExceeded = (PlayerDataManager.Instance.Property.Round - 1) / (ConfigManager.configPlayRound.Records.Count() - 1);
            var exactlyRound = data.round + (numberThresholdsExceeded *  (ConfigManager.configPlayRound.Records.Count() - 1));
            textNameRound.text = exactlyRound.ToString();
            objectCurrentRound.SetActive(currentRound == _data.round);
            objectBottomRound.SetActive(currentRound > _data.round);
            objectTopRound.SetActive(currentRound < _data.round);
            var lst = EnemyController.Instance.ListEnemies;
            GameObjectUtils.Instance.ClearAllChild(transformParentEnemySpawn.gameObject);
            var listEnemy = GameObjectUtils.Instance.ConvertStringToList(data.enemyID);
            foreach (var e in listEnemy)
            {
                if (e != 0)
                {
                    var getData = lst.FirstOrDefault(x=>x.id == e);
                    var ob = GameObjectUtils.Instance.SpawnGameObject(transformParentEnemySpawn, objInfoEnemy);
                    var script = ob.GetComponent<EnemyCardVo>();
                    script.SetDataInformation(getData);
                    ob.SetActive(true);
                };
            }
        }

        public void ChangeCurrent(bool isNextRound)
        {
            if (isNextRound)
            {
                objectCurrentRound.SetActive(true);
                objectTopRound.SetActive(false);
            }
            else
            {
                objectCurrentRound.SetActive(false);
                objectBottomRound.SetActive(true);
            }
        }
    }
}
