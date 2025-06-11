using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Coffee.UIExtensions;
using Core.Entity;
using Core.Manager.Configs;
using DG.Tweening;
using Entity;
using Manager;
using Model;
using MoreMountains.Feedbacks;
using Runtime.Manager;
using TMPro;
using UI.Popups;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace InGame
{
    public class PlanetPackVo : MonoBehaviour
    {
        [SerializeField] private GameObject objPack;
        [SerializeField] private GameObject objShadow;
        [SerializeField] private GameObject objOpenZone;
        [SerializeField] private GameObject objBuy;
        [SerializeField] private Image imgPlanetPack;
        [SerializeField] private Button btnBuy;
        [SerializeField] private GameObject objNextLevel;
        [SerializeField] private TextMeshProUGUI textCurLevel;
        [SerializeField] private TextMeshProUGUI textNextLevel;
        [SerializeField] private TextMeshProUGUI textValueHand;
        [SerializeField] private MMF_Player mmPlayerEffectSelect;
        [SerializeField] private MMF_Player mmPlayerEffectUnSelect;
        [SerializeField] private MMF_Player mmPlayerEffectShake;
        
        private ConfigPlanetCardRecord _data;
        private Vector3 _originalPosition;
        private bool _isSelect;
        private ShopController _shopController;
        private Action<PlanetPackVo> _callback;
        private Action _isDoneCallback;

        public ConfigPlanetCardRecord Data => _data;
        public void OnInit(Action<PlanetPackVo> callback)
        {
            _callback = callback;
        }
        public void SetData(ConfigPlanetCardRecord data)
        {
            _data = data;
            var image = SpritesManager.Instance.GetSpritePlanetPackById(data.indexSprite);
            imgPlanetPack.sprite = image;
            objBuy.SetActive(false);
            btnBuy.interactable = false;
            objNextLevel.SetActive(false);
        }

        public void OnClickUse()
        {
            _callback?.Invoke(this);
            Vector3 targetPosition = objOpenZone.transform.position;
            objShadow.SetActive(false);
            ShowTextEffect();
            // GetComponent<PopupCardDescription>().gameObject.SetActive(false);
            var allHandLevel = PlayCardController.Instance.HandTypePokerClass;
            Type handTypeType = typeof(HandTypePoker);
            PropertyInfo[] properties = handTypeType.GetProperties();
            string result = string.Join(", ", properties
                .Where(p => p.PropertyType == typeof(int))
                .Select(p => p.Name + ": " + p.GetValue(allHandLevel).ToString())); 
            
            AnalyticsManager.LogSelectPlanetPerRound(_data.id,result);
            
            objPack.transform.DOMove(targetPosition, 0.2f)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    if(objPack.activeSelf)
                        PlayCardController.Instance.LevelUpPokerHand(_data.id);
                    mmPlayerEffectShake.PlayFeedbacks();
                });
            if(gameObject.activeSelf)
            {
                StartCoroutine(TransitionCoroutine());
            }
        }

        public void OnCallBackWhenDoneEffect(Action isDone)
        {
            _isDoneCallback = isDone;
        }
        public void OnClick()
        {
            _callback?.Invoke(this);
            
        }
        public void OnPlayEffectSelectCard()
        {
            mmPlayerEffectSelect.PlayFeedbacks();
            objBuy.SetActive(true);
            btnBuy.interactable = true;
        }
        public void OnPlayEffectUnSelectCard()
        {
            mmPlayerEffectUnSelect.PlayFeedbacks();
            objBuy.SetActive(false);
            btnBuy.interactable = false;
        }
        public void IsDoneEffect()
        {
            _isDoneCallback?.Invoke();
            this.gameObject.SetActive(false);
        }
        private void ShowTextEffect()
        {
            objBuy.SetActive(false);
            btnBuy.interactable = false;
            objNextLevel.SetActive(true);
            PokerHandValue value = (PokerHandValue)_data.pokerHand;
            int curLevel = 0;
            if (value != PokerHandValue.None)
            {
                int multiplier = PlayCardController.Instance.GetLevelByPokerValueHand(value);
                curLevel = multiplier;
                textCurLevel.text = string.Format("Level: " + curLevel.ToString());
                textNextLevel.text = string.Format("Level: " + (curLevel + 1));
                textValueHand.text = string.Format(value.ToString());
            }
        }
        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator TransitionCoroutine()
        {
            yield return new WaitForSeconds(1f);
            textNextLevel.transform.DOLocalMoveY(0f, 0.5f).SetEase(Ease.InOutSine);
            textCurLevel.transform.DOLocalMoveY(-120f, 0.5f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
               
            });
        }
    }
}